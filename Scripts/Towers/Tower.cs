using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Enemies;
using Core.Character;
using AudioManagement;
using Sirenix.OdinInspector;

namespace Towers
{
    public enum TowerType
    {
        Archer,
        Mage,
        Bomber,
        MenAtArms
    }

    /// <summary>
    /// Base class for all towers, contains the basic attributes and methods that all towers share
    /// </summary>
    public abstract class Tower : MonoBehaviour
    {
        public TowerType TowerType;
        [SerializeField] ProjectileType projectileType;

        [SerializeField] Transform unitMarksParent;
        [SerializeField] protected Transform towerVisual;
        [SerializeField] protected SpriteRenderer backgroundVisualRenderer;

        [Header("Tower background sprites per level")]
        [SerializeField] protected List<Sprite> backgroundSprites;

        [Header("Tower Units Prefabs")]
        [SerializeField] GameObject towerUnitLvl1Prefab;
        [SerializeField] GameObject towerUnitLvl2Prefab;
        [SerializeField] GameObject towerUnitLvl3Prefab;

        [HideInInspector] public int TotalUpgradeCost, PurchaseCost;

        [SerializeField] protected int unitLevelIncreaseLevel1, unitLevelIncreaseLevel2;

        // The center of the tower, used to find the closest enemy and position IU
        [SerializeField] protected Transform towerCenter;

        [SerializeField] protected float buildTime = 0.57f;

        [Header("Current Tower Attributes")]
        public float AttackSpeed;
        public float AttackRange;
        public float AttackDamage;

        [Space(10)]

        [InfoBox("These settings are modified by the game settings applier, so there is no use in modifying them here"), FoldoutGroup("Tower Settings"), ReadOnly]
        public float StartingRange;
        [FoldoutGroup("Tower Settings"), ReadOnly] public float RangeIncreasePerUpgrade;
        [FoldoutGroup("Tower Settings"), ReadOnly] public float StartingDamage;
        [FoldoutGroup("Tower Settings"), ReadOnly] public float DamageIncreasePerUpgrade;
        [FoldoutGroup("Tower Settings"), ReadOnly] public float StartingAttackSpeed;
        [FoldoutGroup("Tower Settings"), ReadOnly] public float AttackSpeedIncreasePerUpgrade;

        protected List<GameObject> towerUnits = new();

        protected int attackingUnitIndex = 0;

        protected readonly List<GameObject> availableTargets = new();

        protected IEnumerator fireCoroutine;

        protected GameObject currentTarget;

        protected Animator animator;

        // Start as true so that the tower cannot be upgraded while it is being built
        protected bool isUpgrading = true;
        protected bool isAttackDisabled = false;

        protected GameObject waypointListParent;

        // The current targets of the projectiles
        protected Dictionary<Projectile, Transform> projectileTargets = new();

        // Singleton References
        protected AudioManager audioManager;
        protected FMODEvents fmodEvents;
        protected EventBus eventBus;

        // Reference to the projectile parent in the level folder
        protected Transform projectileParent;

        // The layer that will be used to receive the enemies
        protected LayerMask enemyLayer;

        // The current tower level
        protected int currentLevel = 1;

        protected virtual void Start()
        {
            // Singleton References 
            audioManager = AudioManager.Instance;
            fmodEvents = FMODEvents.Instance;
            eventBus = EventBus.Instance;

            if (towerVisual == null)
            {
                Debug.LogError("Tower visual has not been assigned on tower!");
                return;
            }

            projectileParent = GameObject.FindGameObjectWithTag("ProjectileParent").transform;
            if (projectileParent == null)
            {
                Debug.LogError("Projectile parent not found!");
            }

            enemyLayer = GamePrefs.Instance.EnemyLayer;

            AttackDamage = StartingDamage;
            AttackRange = StartingRange;

            animator = towerVisual.GetComponent<Animator>();

            StartCoroutine(StartRoutine());
        }

        private void IncreaseTowerRange()
        {
            AttackRange += RangeIncreasePerUpgrade;
        }

        /// <summary>
        /// Coroutine called when the tower is first created, waits for the build time to pass before creating the units and starting the attack routine
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator StartRoutine()
        {
            yield return new WaitForSeconds(buildTime);

            CreateUnitsForLevel(1);

            ShowTowerUnits();

            isUpgrading = false;

            // Let the units load for a frame before getting the new target
            yield return new WaitForEndOfFrame();

            AttackSpeed = StartingAttackSpeed;

            // StartCoroutine(GetEnemiesCR());

            // Initially, get the enemies
            GetSurroundingEnemies();

            StartCoroutine(AttackEnemy());
            yield break;
        }

        public virtual void DestroyTower()
        {
            Destroy(gameObject);
        }

        private void SetAttackSpeed()
        {
            AttackSpeed -= AttackSpeedIncreasePerUpgrade;
        }

        /// <summary>
        /// Creates the units for the given level at the given unit marks from the unitMarksParent
        /// </summary>
        /// <param name="towerLevel"></param>
        protected virtual void CreateUnitsForLevel(int towerLevel)
        {
            // Sets the background sprite to the corresponding level
            if (backgroundSprites.Count > 0 && backgroundSprites[towerLevel - 1] != null)
                backgroundVisualRenderer.sprite = backgroundSprites[towerLevel - 1];

            foreach (var unit in towerUnits)
            {
                Destroy(unit);
            }

            towerUnits.Clear();

            GameObject towerUnitPrefab;

            if (towerLevel >= unitLevelIncreaseLevel2)
            {
                towerUnitPrefab = towerUnitLvl3Prefab;
            }
            else if (towerLevel >= unitLevelIncreaseLevel1)
            {
                towerUnitPrefab = towerUnitLvl2Prefab;
            }
            else
            {
                towerUnitPrefab = towerUnitLvl1Prefab;
            }

            if (towerUnitPrefab == null)
            {
                Debug.LogError("No tower unit prefab found!");
                return;
            }

            if (unitMarksParent != null && unitMarksParent.childCount > 0)
                foreach (Transform child in unitMarksParent)
                {
                    if (child.name == "Level " + towerLevel)
                    {
                        foreach (Transform unitMark in child)
                        {
                            if (unitMark == null)
                            {
                                Debug.LogError("Unit mark is null!");
                                continue;
                            }

                            GameObject newUnit = Instantiate(towerUnitPrefab, unitMark.position, Quaternion.identity, parent: this.transform);
                            towerUnits.Add(newUnit);
                            newUnit.SetActive(true);
                        }

                        return;
                    }
                }

            Debug.LogError("No unit marks found for level " + towerLevel);
        }

        protected virtual void GetSurroundingEnemies()
        {
            availableTargets.Clear();

            Collider2D[] enemies = Physics2D.OverlapCircleAll(towerCenter.transform.position, AttackRange, enemyLayer);

            foreach (var enemy in enemies)
            {
                availableTargets.Add(enemy.gameObject);
            }

            // If the current target is not in the available targets list, remove it
            if (!availableTargets.Contains(currentTarget))
            {
                //Debug.Log("Removed current target from available targets");
                currentTarget = null;
            }

            //Collider2D[] enemies = Physics2D.OverlapCircleAll(towerCenter.transform.position, AttackRange, enemyLayer);

            //GameObject[] enemiesGO = enemies.Select(e => e.gameObject).ToArray();

            // // Add all enemies to the available targets list
            // foreach (var enemy in enemies)
            // {
            //     if (!availableTargets.Contains(enemy.gameObject))
            //     {
            //         availableTargets.Add(enemy.gameObject);
            //     }
            // }

            // Debug.Log("Enemies in range: " + enemies.Length);

            // // If an enemy in the available targets list is not in the enemies array, remove it
            // for (int i = 0; i < enemiesGO.Length; i++)
            // {
            //     foreach(var availableTarget in availableTargets.ToArray())
            //     {
            //         // If the available target is null, remove it
            //         if (availableTarget == null)
            //         {
            //             availableTargets.Remove(availableTarget);
            //         }

            //         if (!enemiesGO.Contains(availableTarget))
            //         {
            //             if (availableTargets[i] == currentTarget)
            //             {
            //                 Debug.Log("Removed current target from available targets");
            //                 currentTarget = null;
            //             }

            //             availableTargets.Remove(availableTarget);

            //             Debug.Log("Removed enemy from available targets");
            //         }
            //     }
            // }

            // if (enemies.Length == 0)
            // {
            //     if (currentTarget == null)
            //         return;

            //     // If the current size of the available targets list is 0, the current target must be outside of the range
            //     if (availableTargets.Contains(currentTarget))
            //     availableTargets.Remove(currentTarget);
            //     currentTarget = null;
            // }
        }

        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if (!collision.CompareTag("Enemy"))
        //        return;

        //    if (!availableTargets.Contains(collision.gameObject))
        //    {
        //        availableTargets.Add(collision.gameObject);
        //    }
        //}

        //private void OnTriggerExit2D(Collider2D collision)
        //{
        //    if (!collision.CompareTag("Enemy"))
        //        return;

        //    if (currentTarget == collision.gameObject)
        //    {
        //        currentTarget = null;
        //    }

        //    if (availableTargets.Contains(collision.gameObject))
        //    {
        //        availableTargets.Remove(collision.gameObject);
        //    }
        //}

        protected bool HasTarget()
        {
            if (currentTarget != null && currentTarget.GetComponent<Character>().IsDead())
            {
                currentTarget = null;
                return false;
            }

            if (currentTarget == null)
            {
                return false;
            }

            return true;
        }

        protected bool HasAvailableTarget()
        {
            if (availableTargets.Count == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Fires a projectile at the current target
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator AttackEnemy()
        {
            // Every attack, the surrounding enemies are checked
            GetSurroundingEnemies();

            // Every attack, a new target is selected closest to its end point
            SelectTargetFromAvailable();

            //If the tower is not upgrading and has a target, fire a projectile
            if (!isUpgrading && HasTarget())
            {
                // Check if there is already a projectile headed towards the target with enough damage to kill it, if so, cancel the attack
                foreach (var projectile in projectileTargets)
                {
                    if (projectile.Key == null)
                    {
                        continue;
                    }

                    if (projectile.Value == currentTarget.transform && AttackDamage >= currentTarget.GetComponent<Character>().CurrentHealth)
                    {
                        yield return new WaitForFixedUpdate();
                        StartCoroutine(AttackEnemy());
                        yield break;
                    }

                }

                if (fireCoroutine == null)
                {
                    fireCoroutine = FireProjectile(towerUnits[0].GetComponent<TowerUnit>());
                    StartCoroutine(fireCoroutine);
                }
            }

            // Otherwise, wait for a fixed update and try again
            else
            {
                yield return new WaitForFixedUpdate();
                StartCoroutine(AttackEnemy());
                yield break;
            }

            yield return new WaitForSeconds(AttackSpeed);
            StartCoroutine(AttackEnemy());

            yield return null;
        }

        /// <summary>
        /// Fires a projectile from the given unit towards the current target
        /// </summary>
        protected virtual IEnumerator FireProjectile(TowerUnit unit)
        {
            if (!HasTarget())
            {
                towerUnits[attackingUnitIndex].GetComponent<TowerUnit>().StopAttacking();
                fireCoroutine = null;
                yield break;
            }

            unit.AttackTowards(currentTarget.transform);

            yield return new WaitForSeconds(unit.hitMarkTime);

            GetSurroundingEnemies();
            SelectTargetFromAvailable();

            if (unit == null || isUpgrading || isAttackDisabled)
            {
                Debug.Log("Unit is null");
                fireCoroutine = null;
                yield break;
            }

            if (currentTarget == null || currentTarget.GetComponent<Character>().IsDead())
            {
                // check if this is valid
                if (attackingUnitIndex < towerUnits.Count - 1)
                {
                    towerUnits[attackingUnitIndex].GetComponent<TowerUnit>().StopAttacking();
                }

                fireCoroutine = null;
                yield break;
            }

            else
            {
                // check null values
                if (unit == null || currentTarget == null || isUpgrading)
                {
                    fireCoroutine = null;
                    towerUnits[attackingUnitIndex].GetComponent<TowerUnit>().StopAttacking();
                    yield break;
                }

                // Sets the projectile to face upwards when it is created
                Quaternion initialQuat = Quaternion.Euler(0, 0, -90);

                Vector3 spawnPosition = unit.transform.GetChild(0).GetChild(0).transform.position;

                ProjectileSpawnRequest projectileSpawnRequest = new ProjectileSpawnRequest(
                   projectileType, spawnPosition, initialQuat, AttackDamage, currentTarget.transform, Vector3.zero);

                eventBus.Publish("ProjectileSpawnRequest", projectileSpawnRequest);

                // Need to re-implement this!
                // projectileTargets.Add(projectile, currentTarget.transform);
                PlayProjectileLaunchSound();
            }

            fireCoroutine = null;
            yield break;
        }

        protected virtual void PlayProjectileLaunchSound() { }

        /// <summary>
        /// Clears the current target and selects a new target from the available targets. The selected target is closest to its end point.
        /// </summary>
        protected virtual void SelectTargetFromAvailable()
        {
            if (!isUpgrading && !isAttackDisabled)
            {
                if (currentTarget != null)
                {
                    currentTarget = null;
                }

                GameObject foundTarget = null;

                float closestEnemyDistance = int.MaxValue;

                // If there are no available targets, return
                if (availableTargets.Count == 0)
                {
                    return;
                }

                foreach (var enemyIndex in availableTargets.ToArray())
                {
                    if (enemyIndex == null)
                    {
                        availableTargets.Remove(enemyIndex);
                        continue;
                    }

                    if (!enemyIndex.TryGetComponent(out Enemy enemy))
                    {
                        Debug.Log("Enemy component wasn't found in the available target");
                    }

                    if (enemy.IsDead())
                    {
                        continue;
                    }

                    float distance = enemy.GetTotalDistanceToTravel();

                    if (distance < closestEnemyDistance)
                    {
                        closestEnemyDistance = distance;
                        foundTarget = enemy.gameObject;
                    }
                }

                if (foundTarget != null)
                {
                    currentTarget = foundTarget;
                }
            }
        }

        public virtual void UpgradeTower(int level)
        {
            currentLevel++;
            currentTarget = null;
            isUpgrading = true;

            // Increase damage before creating units for the new level
            AttackDamage += DamageIncreasePerUpgrade;

            SetAnimationState("Upgrade" + level);

            IncreaseTowerRange();

            SetAttackSpeed();

            // Create units for the new level
            CreateUnitsForLevel(currentLevel);

            // Hide tower units before waiting
            HideTowerUnits();

            StartCoroutine(WaitTime(buildTime));
        }

        public bool IsUpgrading()
        {
            return isUpgrading;
        }

        public virtual void DisableTowerAttacks()
        {
            isAttackDisabled = true;
        }

        /// <summary>
        /// Waits for the build time to pass before showing the tower units
        /// </summary>
        private IEnumerator WaitTime(float time)
        {
            yield return new WaitForSeconds(time);

            ShowTowerUnits();

            isUpgrading = false;

            yield return null;
        }

        private void SetAnimationState(string newState)
        {
            animator.Play(newState);
        }

        protected virtual void HideTowerUnits()
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

            backgroundVisualRenderer.gameObject.SetActive(false);
        }

        protected virtual void ShowTowerUnits()
        {
            foreach (var unit in towerUnits)
            {
                TowerUnit towerUnit = unit.GetComponent<TowerUnit>();

                if (towerUnit == null)
                {
                    Debug.LogError("Tower unit is null, cannot show!");
                    continue;
                }

                towerUnit.ShowUnit();
            }

            backgroundVisualRenderer.gameObject.SetActive(true);
        }

    }
}