using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Core;
using Core.Character;
using Militia;
using AudioManagement;
using Sirenix.OdinInspector;

namespace Towers
{
    /// <summary>
    /// A tower that deploys militia units onto a unit garrison point
    /// </summary>
    public class MilitiaTower : Tower
    {
        [SerializeField] GameObject militiaUnitPrefab;

        [Header("Tower Unit Sprite Libraries")]
        [SerializeField] SpriteLibraryAsset towerUnitLvl1Lib;
        [SerializeField] SpriteLibraryAsset towerUnitLvl2Lib;
        [SerializeField] SpriteLibraryAsset towerUnitLvl3Lib;

        [Header("The upgrade levels where the sprite sheets will be replaced")]
        [SerializeField] private int level2Upgrade, level3Upgrade;

        [Header("Deployable Range Collider")]
        [SerializeField] private CircleCollider2D deployRangeCollider;

        public float DeployRange { get; private set; }

        public int unitRespawnTime;
        [FoldoutGroup("Tower Settings"), ReadOnly] public float UnitPercentHealPerSecond;
        [FoldoutGroup("Tower Settings"), ReadOnly] public float StartingUnitDamage;
        [FoldoutGroup("Tower Settings"), ReadOnly] public float UnitDamageIncreasePerUpgrade;
        [FoldoutGroup("Tower Settings"), ReadOnly] public float StartingUnitHealth;
        [FoldoutGroup("Tower Settings"), ReadOnly] public float UnitHealthIncreasePerUpgrade;
        [FoldoutGroup("Tower Settings"), ReadOnly] public float UnitAttackSpeed;

        private float unitDamage;
        private float unitHealth;

        [SerializeField] private Transform positionMarkGroup;
        private RallyPoint rallyPoint;

        [SerializeField] private Transform unitSpawnPoint;

        // Marker that will activate when the unit waypoint is moved. 
        [SerializeField] private GameObject garrisonFlagPrefab;

        // the level at which the tower door opens when a unit is deployed
        private const int towerDoorOpenLevel = 3;

        private int towerLevel;

        private Check2DNavMesh check2DNavMesh;

        // Animation Keys

        private const string DOOROPEN3 = "MilitiaTower_DoorOpenLvl3", DOOROPEN4 = "MilitiaTower_DoorOpenLvl4", DOOROPEN5 = "MilitiaTower_DoorOpenLvl5", DOOROPEN6 = "MilitiaTower_DoorOpenLvl6", DOOROPEN7 = "MilitiaTower_DoorOpenLvl7";

        protected override void Start()
        {
            // Singleton Assignments
            audioManager = AudioManager.Instance;
            fmodEvents = FMODEvents.Instance;

            if (towerVisual == null)
            {
                Debug.LogError("Tower visual has not been assigned on tower!");
                return;
            }

            rallyPoint = positionMarkGroup.GetComponent<RallyPoint>();

            if (rallyPoint == null)
            {
                Debug.LogError("Rally point could not be found within the position mark group!");
                return;
            }

            enemyLayer = GamePrefs.Instance.EnemyLayer;

            AttackRange = StartingRange;

            animator = towerVisual.GetComponent<Animator>();

            if (animator == null)
            {
                Debug.LogError("Animator could not be found on tower!");
            }

            check2DNavMesh = FindFirstObjectByType<Check2DNavMesh>();

            if (check2DNavMesh == null)
            {
                Debug.LogError("Check2DNavMesh could not be found in the scene!");
            }

            StartCoroutine(StartRoutine());
        }

        protected override void CreateUnitsForLevel(int towerLevel)
        {
            SpriteLibraryAsset spriteLibrary = towerUnitLvl1Lib;

            // If the tower level is at one of the upgrade levels, change the sprite library to the corresponding level's library, otherwise set to the lvl 1 library
            if (towerLevel >= level3Upgrade)
            {
                spriteLibrary = towerUnitLvl3Lib;
            }
            else if (towerLevel >= level2Upgrade)
            {
                spriteLibrary = towerUnitLvl2Lib;
            }
     
            foreach (var unit in towerUnits)
            {
                SpriteLibrary spriteRenderer = unit.GetComponentInChildren<SpriteLibrary>();

                spriteRenderer.spriteLibraryAsset = spriteLibrary;
            }

            ShowTowerUnits();       
        }

        protected void UpgradeUnitStats()
        {
            foreach (var unit in towerUnits)
            {
                MilitiaUnit militiaUnit = unit.GetComponent<MilitiaUnit>();

                if (militiaUnit == null)
                {
                    Debug.LogError("Militia unit script not found!");
                    return;
                }   

                // Add the damage and health addition per upgrade to the unit's stats
                militiaUnit.damage += UnitDamageIncreasePerUpgrade;
                militiaUnit.maxHealth += UnitHealthIncreasePerUpgrade;

                // If the militia unit is not currently in combat, heal it to full health
                if (militiaUnit.State != CharacterState.Attacking)
                {
                    militiaUnit.CurrentHealth = militiaUnit.maxHealth;
                }

                // Reload the health bar of the unit
                militiaUnit.ReloadHealthBar();
            }
        }

        protected override IEnumerator StartRoutine()
        {
            SetMilitiaWaypoint(check2DNavMesh.GetRoadSampleFromPosition(transform.position + new Vector3(0, 0.32f, 0)), false);

            // Play the construction sound effect
            audioManager.PlayTowerConstructionSound(fmodEvents.militiaTowerConstructionSound, 0, transform.position);

            yield return new WaitForSeconds(buildTime);

            isUpgrading = false;

            StartCoroutine(InstantiateUnits());
            CreateUnitsForLevel(1);


            yield break;
        }

        public void SetMilitiaWaypoint(Vector3 position, bool createFlag = true)
        {     
            Vector3 oldGarrisonPos = positionMarkGroup.position;

            if (Vector3.Distance(towerCenter.transform.position, position) < AttackRange)
            {
                if (!check2DNavMesh.PointOnNavMesh(position))
                {
                    //Debug.Log("Position is not on the navmesh!");
                    position = check2DNavMesh.GetNearestPointOnNavMesh(position, AttackRange);

                    // If the position is still not on the navmesh, return
                    if (position == Vector3.zero)
                    {
                        return;
                    }   
                }
                
                rallyPoint.ChangePosition(position);

                if (createFlag)
                {
                    GameObject garrisonFlag;
                    garrisonFlag = Instantiate(garrisonFlagPrefab, position, Quaternion.identity);
                    StartCoroutine(DestroyGarrisonFlag(garrisonFlag));
                }
            }

            // If the targeted position is too far away from the tower or not on the navmesh, move the waypoint as close as possible to the target position while still on the navmesh
            else
            {
                Vector3 direction = position - (towerCenter.transform.position);
                direction.Normalize();

                Vector3 newPosition = (towerCenter.transform.position) + (direction * AttackRange);

                if (!check2DNavMesh.PointOnNavMesh(newPosition, 0.5f))
                {        
                    return;
                }

                rallyPoint.ChangePosition(newPosition);

                if (createFlag)
                {
                    GameObject garrisonFlag;
                    garrisonFlag = Instantiate(garrisonFlagPrefab, newPosition, Quaternion.identity);
                    StartCoroutine(DestroyGarrisonFlag(garrisonFlag));
                }
            }     
        } 

        private IEnumerator DestroyGarrisonFlag(GameObject flagObject)
        {
            yield return new WaitForSeconds(2);

            if (flagObject != null)
                Destroy(flagObject);

            yield break;
        }

        private IEnumerator InstantiateUnits()
        {
            // First, create the militia units and assign them to the rally point
            foreach (Transform child in positionMarkGroup)
            {
                if (!child.gameObject.activeInHierarchy)
                    continue;

                GameObject newMilitiaUnit = Instantiate(militiaUnitPrefab, unitSpawnPoint.position, Quaternion.identity, parent: this.transform);

                // Set the X rotation to -90 so that the unit faces upwards
                towerUnits.Add(newMilitiaUnit);

                MilitiaUnit militiaUnit = newMilitiaUnit.GetComponent<MilitiaUnit>();

                rallyPoint.rallyPointUnits.Add(militiaUnit);

                militiaUnit.attackSpeed = UnitAttackSpeed;
                militiaUnit.percentHealPerSecond = UnitPercentHealPerSecond;
                militiaUnit.damage = StartingUnitDamage;
                militiaUnit.maxHealth = StartingUnitHealth;

                militiaUnit.ReloadHealthBar();

                if (militiaUnit == null)
                {
                    Debug.LogError("Militia unit script not found!");
                    yield break;
                }

                militiaUnit.SetPositionMark(child);
                militiaUnit.UnitDeathEvent.AddListener(RespawnUnit);

                // Initially, disable the unit
                militiaUnit.gameObject.SetActive(false);
            }
          
            rallyPoint.Initialize();

            // Enable all the militia units after they have been created and the rally point has been initialized
            foreach (var unit in towerUnits)
            {
                MilitiaUnit militiaUnit = unit.GetComponent<MilitiaUnit>();

                militiaUnit.gameObject.SetActive(true);
                militiaUnit.ShowUnit();

                // The delay before the next unit is instantiated
                yield return new WaitForSeconds(0.3f);
            }

            yield return null;
        }

        private void RespawnUnit(MilitiaUnit unit)
        {
            StartCoroutine(RespawnUnitRoutine(unit));
        }

        private IEnumerator RespawnUnitRoutine(MilitiaUnit militiaUnit)
        {
            yield return new WaitForSeconds(unitRespawnTime);

            if (isAttackDisabled)
            {
                Debug.LogWarning("Attack is disabled, cannot respawn unit!");
                yield break;
            }

            OpenDoorAnimation();

            militiaUnit.transform.position = unitSpawnPoint.position;

            militiaUnit.gameObject.SetActive(true);

            militiaUnit.ResetUnit();

            ShowMilitiaUnit(militiaUnit);

            yield return null;
        }

        private void OpenDoorAnimation()
        {
            if (towerLevel < towerDoorOpenLevel)
                return;

            switch (towerLevel)
            {
                case 3:
                    SetAnimationState(DOOROPEN3);
                    break;

                case 4:
                    SetAnimationState(DOOROPEN4);
                    break;

                case 5:
                    SetAnimationState(DOOROPEN5);
                    break;

                case 6:
                    SetAnimationState(DOOROPEN6);
                    break;

                case 7:
                    SetAnimationState(DOOROPEN7);
                    break;
            }
        }

        public override void DestroyTower()
        {
            foreach (var unit in towerUnits)
            {
                Destroy(unit.gameObject);
            }

            base.DestroyTower();
        }

        public override void DisableTowerAttacks()
        {
            base.DisableTowerAttacks();

            foreach (var unit in towerUnits)
            {
                unit.GetComponent<MilitiaUnit>().KillUnit();
            }
        }

        public override void UpgradeTower(int level)
        {
            isUpgrading = true;

            towerLevel = level;

            audioManager.PlayTowerConstructionSound(fmodEvents.militiaTowerConstructionSound, level, transform.position);

            SetAnimationState("Upgrade" + level);

            CreateUnitsForLevel(level);

            UpgradeUnitStats();

            StartCoroutine(WaitTime(buildTime));
        }


        private IEnumerator WaitTime(float time)
        {
            yield return new WaitForSeconds(time);

            isUpgrading = false;

            yield return null;
        }

        private void SetAnimationState(string newState)
        {
            animator.Play(newState);
        }

        protected override void HideTowerUnits()
        {
            foreach (var unit in towerUnits)
            {
                TowerUnit towerUnit = unit.GetComponent<TowerUnit>();

                if (towerUnit == null)
                {
                    Debug.LogError("Tower unit is null, cannot hide!");
                    continue;
                }

                towerUnit.HideUnit();
            }
        }

        protected override void ShowTowerUnits()
        {
            foreach (var unit in towerUnits)
            {
                MilitiaUnit militiaUnit = unit.GetComponent<MilitiaUnit>();

                if (militiaUnit == null)
                {
                    Debug.LogError("Tower unit is null, cannot show!");
                    continue;
                }

                if (unit.activeInHierarchy)
                    militiaUnit.ShowUnit();
            }
        }

        protected void ShowMilitiaUnit(MilitiaUnit militiaUnit)
        {
            if (militiaUnit == null)
            {
                Debug.LogError("Militia unit is null, cannot show!");
                return;
            }

            militiaUnit.ShowUnit();
        }
    }
}
