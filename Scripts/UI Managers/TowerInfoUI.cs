using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Towers;

namespace UI.Management
{
    /// <summary>
    /// A UI element that displays upgrade and purchase information about towers.
    /// </summary>
    public class TowerInfoUI : MonoBehaviour
    {
        public static TowerInfoUI Instance { get; private set; }

       // [SerializeField] private GameObject background;
        [SerializeField] private TextMeshProUGUI towerTitle;

        [SerializeField] private GameObject objectGroup;

        [SerializeField] private TowerInfoPoint towerInfoPointPrefab;
        [SerializeField] private Transform towerInfoPointParent;

        [SerializeField] private DPSInfoPoint dpsInfoPoint;

        // Object pooling variables
        private List<TowerInfoPoint> towerInfoPointPool;
        private const int InitialPoolSize = 5;

        [Header("Tower Prefabs")]
        [SerializeField] private ArcherTower archerTowerPrefab;
        [SerializeField] private CatapultTower catapultTowerPrefab;
        [SerializeField] private MageTower mageTowerPrefab;
        [SerializeField] private MilitiaTower militiaTowerPrefab;

        // Icon groups
        [SerializeField] private TowerInfoIcon damageGroup, rangeGroup, speedGroup, healthGroup;
        [SerializeField] private GameObject towerInfoIconParent;

        [SerializeField] private Vector2 windowOffsetPos;
        // [SerializeField] private float fadeSpeed = 5;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("TowerInfoUI instance already exists!");
                Destroy(gameObject);
            }

            InitializeObjectPool();
        }

        /// <summary>
        /// Creates a pool of TowerInfoPoint objects.
        /// </summary>
        private void InitializeObjectPool()
        {
            towerInfoPointPool = new List<TowerInfoPoint>();

            for (int i = 0; i < InitialPoolSize; i++)
            {
                // Get the child index of the tower info point prefab
                int prefabIndex = towerInfoPointPrefab.transform.GetSiblingIndex();

                TowerInfoPoint infoPoint = Instantiate(towerInfoPointPrefab, towerInfoPointParent);

                // Set the new info point as the next sibling of the prefab
                infoPoint.transform.SetSiblingIndex(prefabIndex + 1);

                infoPoint.gameObject.SetActive(false);
                towerInfoPointPool.Add(infoPoint);
            }
        }

        public void DeactivateTowerPurchaseInfo()
        {
            objectGroup.SetActive(false);
        }

        private void DeactivateAllInfoPoints()
        {
            foreach (TowerInfoPoint infoPoint in towerInfoPointPool)
            {
                infoPoint.Deactivate();
            }

            dpsInfoPoint.Deactivate();
        }

        /// <summary>
        /// Activates the next available info point and sets the text.
        /// </summary>
        /// <param name="text"></param>
        private void SetNextInfoPoint(string text)
        {
            foreach (TowerInfoPoint point in towerInfoPointPool)
            {
                // Check if the text already exists in an active point
                if (point.gameObject.activeInHierarchy && point.Text == text)
                {
                    return;
                }
            }

            foreach (TowerInfoPoint point in towerInfoPointPool)
            {
                // Check if the point is inactive
                if (!point.gameObject.activeInHierarchy)
                {
                    point.Text = text;
                    return; // Break out of the loop after setting the text in the first inactive point
                }
            }

            Debug.Log("All points are active or text already exists");
        }

        /// <summary>
        /// Activates the tower info UI and sets the text based on the tower type and level.
        /// </summary>
        public void ActivateTowerInfo(Vector2 position, TowerType towerType, int towerLevel)
        {
            Vector2 actualOffset = new Vector3(windowOffsetPos.x * ((float)Screen.width / 1920.0f), windowOffsetPos.y * ((float)Screen.height / 1080.0f));

            if (position.y > Screen.height / 2)
            {
                transform.position = position - actualOffset;

                // Change the pivot point so that the window expands in the correct direction
                objectGroup.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
            }
            else
            {
                transform.position = position + actualOffset;

                // Change the pivot point so that the window expands in the correct direction
                objectGroup.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            }


            objectGroup.SetActive(true);

            damageGroup.Deactivate();
            rangeGroup.Deactivate();
            speedGroup.Deactivate();
            healthGroup.Deactivate();

            damageGroup.SetColorDefault();
            rangeGroup.SetColorDefault();
            speedGroup.SetColorDefault();
            healthGroup.SetColorDefault();

            // Disable all info points
            DeactivateAllInfoPoints();
            towerInfoIconParent.SetActive(true);

            switch (towerType)
            {
                case TowerType.Archer:
                    towerTitle.text = "Archer Tower";

                    damageGroup.Activate();
                    rangeGroup.Activate();
                    speedGroup.Activate();

                    float currentDamage = archerTowerPrefab.StartingDamage + (archerTowerPrefab.DamageIncreasePerUpgrade * towerLevel);
                    float currentRange = archerTowerPrefab.StartingRange + (archerTowerPrefab.RangeIncreasePerUpgrade * towerLevel);
                    float currentAttackSpeed = archerTowerPrefab.StartingAttackSpeed - (archerTowerPrefab.AttackSpeedIncreasePerUpgrade * towerLevel);

                    damageGroup.Text = currentDamage.ToString();
                    rangeGroup.Text = currentRange.ToString();

                    float archerAttackSpeed = GetAttacksPerSecond(currentAttackSpeed);

                    speedGroup.Text = $"{archerAttackSpeed}/s";

                    break;

                case TowerType.Mage:
                    towerTitle.text = "Mage Tower";

                    SetNextInfoPoint("Slow Attack Speed");
                    SetNextInfoPoint("Low Range");
                    SetNextInfoPoint("High Damage");

                    damageGroup.Activate();
                    rangeGroup.Activate();
                    speedGroup.Activate();

                    damageGroup.Text = mageTowerPrefab.StartingDamage.ToString();
                    rangeGroup.Text = (mageTowerPrefab.StartingRange).ToString();

                    // Turn attack speed delay into attacks per second
                    float mageAttackSpeed = GetAttacksPerSecond(mageTowerPrefab.StartingAttackSpeed);

                    speedGroup.Text = $"{mageAttackSpeed}/s";
                    break;

                case TowerType.Bomber:
                    towerTitle.text = "Catapult Tower";

                    SetNextInfoPoint("Slow Attack Speed");
                    SetNextInfoPoint("High Splash Damage");
                    SetNextInfoPoint("Multiple Projectiles");
                    SetNextInfoPoint("Weak vs Fast Targets");

                    damageGroup.Activate();
                    rangeGroup.Activate();
                    speedGroup.Activate();

                    damageGroup.Text = catapultTowerPrefab.StartingDamage.ToString();
                    rangeGroup.Text = (catapultTowerPrefab.StartingRange).ToString();

                    // Turn attack speed delay into attacks per second
                    float catapultAttackSpeed = GetAttacksPerSecond(catapultTowerPrefab.StartingAttackSpeed);

                    speedGroup.Text = $"{catapultAttackSpeed}/s";
                    break;

                case TowerType.MenAtArms:
                    towerTitle.text = "Militia Tower";

                    SetNextInfoPoint("High Health");
                    SetNextInfoPoint("Low Damage");
                    SetNextInfoPoint("Stops Ground Enemies");

                    damageGroup.Activate();
                    healthGroup.Activate();

                    damageGroup.Text = militiaTowerPrefab.startingUnitDamage.ToString();
                    healthGroup.Text = militiaTowerPrefab.startingUnitHealth.ToString();
                    break;

            }
        }

        /// <summary>
        /// Activates the tower upgrade info UI and sets the text based on the tower type and level.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="towerType"></param>
        /// <param name="towerLevel"></param>
        public void ActivateTowerUpgradeInfo(Vector2 position, TowerType towerType, int towerLevel)
        {
            int previewTowerLevel = towerLevel + 1;

            Vector2 actualOffset = new Vector3(windowOffsetPos.x * ((float)Screen.width / 1920.0f), windowOffsetPos.y * ((float)Screen.height / 1080.0f));

            if (position.y > Screen.height / 2)
            {
                transform.position = position - actualOffset;

                // Change the pivot point so that the window expands in the correct direction
                objectGroup.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1); 
            }
            else
            {
                transform.position = position + actualOffset;

                // Change the pivot point so that the window expands in the correct direction
                objectGroup.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            }

            objectGroup.SetActive(true);

            damageGroup.Deactivate();
            rangeGroup.Deactivate();
            speedGroup.Deactivate();
            healthGroup.Deactivate();

            damageGroup.SetColorUpgrade();
            rangeGroup.SetColorUpgrade();
            speedGroup.SetColorUpgrade();
            healthGroup.SetColorUpgrade();

            //background.SetActive(true);

            // Disable all info points
            DeactivateAllInfoPoints();
            towerInfoIconParent.SetActive(false);

            // Define the variables
            float attackSpeedIncreasePercentage = 0, rangeIncreasePercentage = 0, damageIncreasePercentage = 0;
            float currentDPS = 0, addedDPS = 0;

            switch (towerType)
            {
                case TowerType.Archer:
                    towerTitle.text = "Archer Level " + (previewTowerLevel);

                    attackSpeedIncreasePercentage = (float)Math.Round(((archerTowerPrefab.AttackSpeedIncreasePerUpgrade / archerTowerPrefab.StartingAttackSpeed) * 100), 1);
                    rangeIncreasePercentage = (float)Math.Round(((archerTowerPrefab.RangeIncreasePerUpgrade / archerTowerPrefab.StartingRange) * 100), 1);
                    //    float archerDamageIncreasePercentage = (float)Math.Round(((archerTowerPrefab.DamageIncreasePerUpgrade / archerTowerPrefab.StartingDamage) * 100), 1);

                    currentDPS = (float)Math.Round((archerTowerPrefab.StartingDamage + (archerTowerPrefab.DamageIncreasePerUpgrade * (towerLevel - 1))) / (archerTowerPrefab.StartingAttackSpeed - (archerTowerPrefab.AttackSpeedIncreasePerUpgrade * (towerLevel - 1))), 1);
                    addedDPS = (float)Math.Round((archerTowerPrefab.StartingDamage + (archerTowerPrefab.DamageIncreasePerUpgrade * (previewTowerLevel - 1))) / (archerTowerPrefab.StartingAttackSpeed - (archerTowerPrefab.AttackSpeedIncreasePerUpgrade * (previewTowerLevel - 1))), 1);

                    SetNextInfoPoint($"Attack Speed +{attackSpeedIncreasePercentage}%");
                    SetNextInfoPoint($"Range +{rangeIncreasePercentage}%");

                    dpsInfoPoint.SetDPSToFromText(currentDPS, addedDPS);
                    //  SetNextInfoPoint($"Damage +{archerDamageIncreasePercentage}%");

                    //SetNextInfoPoint($"DPS: {currentDPS} (+{addedDPS})");

                    //damageGroup.Activate();
                    //rangeGroup.Activate();
                    //speedGroup.Activate();

                    //float currentDamage = archerTowerPrefab.StartingDamage + (archerTowerPrefab.DamageIncreasePerUpgrade * previewTowerLevel);
                    //float currentRange = archerTowerPrefab.StartingRange + (archerTowerPrefab.RangeIncreasePerUpgrade * previewTowerLevel);

                    //// Set the text colors to green
                    //damageGroup.SetColorUpgrade();
                    //rangeGroup.SetColorUpgrade();
                    //damageGroup.SetColorUpgrade();

                    //damageGroup.Text = ((float)Math.Round(currentDamage, 1)).ToString();
                    //rangeGroup.Text = ((float)Math.Round(currentRange, 1)).ToString();

                    //float archerAttackSpeed = GetAttacksPerSecond(archerTowerPrefab.StartingAttackSpeed) + (archerTowerPrefab.AttackSpeedIncreasePerUpgrade * previewTowerLevel);

                    //speedGroup.Text = $"{(float)Math.Round(archerAttackSpeed, 1)}/s";
                    break;

                case TowerType.Bomber:
                    towerTitle.text = "Catapult Level " + (previewTowerLevel);

                    attackSpeedIncreasePercentage = (float)Math.Round(((catapultTowerPrefab.AttackSpeedIncreasePerUpgrade / catapultTowerPrefab.StartingAttackSpeed) * 100), 1);
                    damageIncreasePercentage = (float)Math.Round(((catapultTowerPrefab.DamageIncreasePerUpgrade / catapultTowerPrefab.StartingDamage) * 100), 1);
                    rangeIncreasePercentage = (float)Math.Round(((catapultTowerPrefab.RangeIncreasePerUpgrade / catapultTowerPrefab.StartingRange) * 100), 1);

                    currentDPS = (float)Math.Round((catapultTowerPrefab.StartingDamage + (catapultTowerPrefab.DamageIncreasePerUpgrade * (towerLevel - 1))) / (catapultTowerPrefab.StartingAttackSpeed - (catapultTowerPrefab.AttackSpeedIncreasePerUpgrade * (towerLevel - 1))), 1);
                    addedDPS = (float)Math.Round((catapultTowerPrefab.StartingDamage + (catapultTowerPrefab.DamageIncreasePerUpgrade * (previewTowerLevel - 1))) / (catapultTowerPrefab.StartingAttackSpeed - (catapultTowerPrefab.AttackSpeedIncreasePerUpgrade * (previewTowerLevel - 1))), 1);

                    SetNextInfoPoint($"Attack Speed +{attackSpeedIncreasePercentage}%");
                    SetNextInfoPoint($"Damage +{damageIncreasePercentage}%");
                    SetNextInfoPoint($"Range +{rangeIncreasePercentage}%");

                    int numberOfProjectiles = 0, nextProjectiles = 0;
                    for (int i = catapultTowerPrefab.ProjectileChanges.Count - 1; i > 0; i--)
                    {
                        numberOfProjectiles = catapultTowerPrefab.ProjectileChanges[i].numberOfProjectiles;

                        if (towerLevel == catapultTowerPrefab.ProjectileChanges[i].level)
                        {
                            Debug.Log($"Number of projectiles: {numberOfProjectiles}, TowerLevel: {towerLevel}");
                            break;
                        }          
                    }

                    // Iterate through the projectile changes of the catapult tower prefab, if the key is equal to the tower level, add a new info point displaying an additional projectile
                    for (int i = catapultTowerPrefab.ProjectileChanges.Count - 1; i > 0; i--)
                    {
                        nextProjectiles = catapultTowerPrefab.ProjectileChanges[i].numberOfProjectiles;

                        if (previewTowerLevel == catapultTowerPrefab.ProjectileChanges[i].level)
                        {
                            SetNextInfoPoint($"Projectiles +1");

                            Debug.Log($"Number of projectiles: {numberOfProjectiles}, TowerLevel: {previewTowerLevel}");
                            break;
                        }
                    }

                    dpsInfoPoint.SetDPSToFromText(currentDPS * numberOfProjectiles, addedDPS * nextProjectiles);

                    //damageGroup.Activate();
                    //rangeGroup.Activate();
                    //speedGroup.Activate();

                    //float currentCatapultDamage = catapultTowerPrefab.StartingDamage + (catapultTowerPrefab.DamageIncreasePerUpgrade * previewTowerLevel);
                    //float currentCatapultRange = catapultTowerPrefab.StartingRange + (catapultTowerPrefab.RangeIncreasePerUpgrade * previewTowerLevel);

                    //// Set the text colors to green
                    //damageGroup.SetColorUpgrade();
                    //rangeGroup.SetColorUpgrade();
                    //damageGroup.SetColorUpgrade();

                    //damageGroup.Text = $"{currentCatapultDamage}";
                    //rangeGroup.Text = (currentCatapultRange).ToString();

                    //float catapultAttackSpeed = GetAttacksPerSecond(catapultTowerPrefab.StartingAttackSpeed) + (catapultTowerPrefab.AttackSpeedIncreasePerUpgrade * previewTowerLevel);

                    //speedGroup.Text = $"{catapultAttackSpeed}/s";

                    break;

                case TowerType.Mage:
                    towerTitle.text = "Mage Level " + (previewTowerLevel);

                    float mageDamageIncreasePercentage = (float)Math.Round(((mageTowerPrefab.DamageIncreasePerUpgrade / mageTowerPrefab.StartingDamage) * 100), 1);
                    float mageRangeIncreasePercentage = (float)Math.Round(((mageTowerPrefab.RangeIncreasePerUpgrade / mageTowerPrefab.StartingRange) * 100), 1);

                    currentDPS = (float)Math.Round((mageTowerPrefab.StartingDamage + (mageTowerPrefab.DamageIncreasePerUpgrade * (towerLevel - 1))) / (mageTowerPrefab.StartingAttackSpeed - (mageTowerPrefab.AttackSpeedIncreasePerUpgrade * (towerLevel - 1))), 1);
                    addedDPS = (float)Math.Round((mageTowerPrefab.StartingDamage + (mageTowerPrefab.DamageIncreasePerUpgrade * (previewTowerLevel - 1))) / (mageTowerPrefab.StartingAttackSpeed - (mageTowerPrefab.AttackSpeedIncreasePerUpgrade * (previewTowerLevel - 1))), 1);

                    SetNextInfoPoint($"Damage +{mageDamageIncreasePercentage}%");
                    SetNextInfoPoint($"Range +{mageRangeIncreasePercentage}%");

                    dpsInfoPoint.SetDPSToFromText(currentDPS, addedDPS);

                    //damageGroup.Activate();
                    //rangeGroup.Activate();
                    //speedGroup.Activate();

                    //float currentMageDamage = mageTowerPrefab.StartingDamage + (mageTowerPrefab.DamageIncreasePerUpgrade * previewTowerLevel);
                    //float currentMageRange = mageTowerPrefab.StartingRange + (mageTowerPrefab.RangeIncreasePerUpgrade * previewTowerLevel);
                    //float currentMageAttackSpeed = mageTowerPrefab.StartingAttackSpeed + (mageTowerPrefab.AttackSpeedIncreasePerUpgrade * previewTowerLevel);

                    //// If the value changes from the previous level, set the text color to green 
                    //damageGroup.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;

                    //if (currentMageDamage != mageTowerPrefab.StartingDamage + (mageTowerPrefab.DamageIncreasePerUpgrade * (previewTowerLevel - 1)))
                    //{
                    //    damageGroup.SetColorUpgrade();
                    //}
                    //else
                    //{
                    //    damageGroup.SetColorDefault();
                    //}

                    //// If the value changes from the previous level, set the text color to green 
                    //if (currentMageRange != mageTowerPrefab.StartingRange + (mageTowerPrefab.RangeIncreasePerUpgrade * (previewTowerLevel - 1)))
                    //{
                    //    rangeGroup.SetColorUpgrade();
                    //}
                    //else
                    //{
                    //    rangeGroup.SetColorDefault();
                    //}

                    //// If the value changes from the previous level, set the text color to green    
                    //if (currentMageAttackSpeed != mageTowerPrefab.StartingAttackSpeed + (mageTowerPrefab.AttackSpeedIncreasePerUpgrade * (previewTowerLevel - 1)))
                    //{
                    //    speedGroup.SetColorUpgrade();
                    //}
                    //else
                    //{
                    //    speedGroup.SetColorDefault();
                    //}

                    //damageGroup.Text = ((float)Math.Round(currentMageDamage, 1)).ToString();
                    //rangeGroup.Text = ((float)Math.Round(currentMageRange, 1)).ToString();

                    //float mageAttackSpeed = GetAttacksPerSecond(mageTowerPrefab.StartingAttackSpeed) + (mageTowerPrefab.AttackSpeedIncreasePerUpgrade * previewTowerLevel);
                    //speedGroup.Text = $"{(float)Math.Round(mageAttackSpeed, 1)}/s";
                    break;

                case TowerType.MenAtArms:
                    towerTitle.text = "Militia Level " + (previewTowerLevel);

                    float militiaDamageIncreasePercentage = (float)Math.Round(((militiaTowerPrefab.unitDamageIncreasePerUpgrade / militiaTowerPrefab.startingUnitDamage) * 100), 1);
                    float militiaHealthIncreasePercentage = (float)Math.Round(((militiaTowerPrefab.unitHealthIncreasePerUpgrade / militiaTowerPrefab.startingUnitHealth) * 100), 1);

                    SetNextInfoPoint($"Health +{militiaHealthIncreasePercentage}%");
                    SetNextInfoPoint($"Damage +{militiaDamageIncreasePercentage}%");

                    towerInfoIconParent.SetActive(true);

                    damageGroup.Activate();
                    healthGroup.Activate();

                    float currentMilitiaDamage = militiaTowerPrefab.startingUnitDamage + (militiaTowerPrefab.unitDamageIncreasePerUpgrade * previewTowerLevel - 1);
                    float currentMilitiaHealth = militiaTowerPrefab.startingUnitHealth + (militiaTowerPrefab.unitHealthIncreasePerUpgrade * previewTowerLevel - 1);

                    // If the value changes from the previous level, set the text color to green 
                    if (currentMilitiaDamage != militiaTowerPrefab.startingUnitDamage + (militiaTowerPrefab.unitDamageIncreasePerUpgrade * (previewTowerLevel - 1)))
                    {
                        damageGroup.SetColorUpgrade();
                    }
                    else
                    {
                        damageGroup.SetColorDefault();
                    }

                    // If the value changes from the previous level, set the text color to green 
                    if (currentMilitiaHealth != militiaTowerPrefab.startingUnitHealth + (militiaTowerPrefab.unitDamageIncreasePerUpgrade * (previewTowerLevel - 1)))
                    {
                        healthGroup.SetColorUpgrade();
                    }
                    else
                    {
                        healthGroup.SetColorDefault();
                    }

                    damageGroup.Text = currentMilitiaDamage.ToString();
                    healthGroup.Text = currentMilitiaHealth.ToString();
                    break;
            }
        }

        /// <summary>
        /// Activates the tower purchase info UI and sets the text based on the tower type and level.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="type"></param>
        public void ActivateTowerPurchaseInfo(Vector2 position, TowerType type)
        {
            Vector2 actualOffset = new Vector3(windowOffsetPos.x * ((float)Screen.width / 1920.0f), windowOffsetPos.y * ((float)Screen.height / 1080.0f));

            // Check if the given position is on the upper or lower half of the screen
            if (position.y > Screen.height / 2)
            {
                transform.position = position - actualOffset;

                // Change the pivot point so that the window expands in the correct direction
                objectGroup.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
            }
            else
            {
                transform.position = position + actualOffset;

                // Change the pivot point so that the window expands in the correct direction
                objectGroup.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            }


            objectGroup.SetActive(true);

            damageGroup.Deactivate();
            rangeGroup.Deactivate();
            speedGroup.Deactivate();
            healthGroup.Deactivate();

            damageGroup.SetColorDefault();
            rangeGroup.SetColorDefault();
            speedGroup.SetColorDefault();
            healthGroup.SetColorDefault();

            // Disable all info points
            DeactivateAllInfoPoints();
            towerInfoIconParent.SetActive(true);

            switch (type)
            {
                case TowerType.Archer:
                    towerTitle.text = "Archer Tower";

                    SetNextInfoPoint("Fast Attack Speed");
                    SetNextInfoPoint("High Range");
                    SetNextInfoPoint("Low Damage");

                    damageGroup.Activate();
                    rangeGroup.Activate();
                    speedGroup.Activate();

                    damageGroup.Text = archerTowerPrefab.StartingDamage.ToString();
                    rangeGroup.Text = (archerTowerPrefab.StartingRange).ToString();

                    float archerAttackSpeed = GetAttacksPerSecond(archerTowerPrefab.StartingAttackSpeed);

                    speedGroup.Text = $"{archerAttackSpeed}/s";
                    break;

                case TowerType.Mage:
                    towerTitle.text = "Mage Tower";

                    SetNextInfoPoint("Slow Attack Speed");
                    SetNextInfoPoint("Low Range");
                    SetNextInfoPoint("High Damage");

                    damageGroup.Activate();
                    rangeGroup.Activate();
                    speedGroup.Activate();

                    damageGroup.Text = mageTowerPrefab.StartingDamage.ToString();
                    rangeGroup.Text = (mageTowerPrefab.StartingRange).ToString();

                    // Turn attack speed delay into attacks per second
                    float mageAttackSpeed = GetAttacksPerSecond(mageTowerPrefab.StartingAttackSpeed);

                    speedGroup.Text = $"{mageAttackSpeed}/s";
                    break;

                case TowerType.Bomber:
                    towerTitle.text = "Catapult Tower";

                    SetNextInfoPoint("Slow Attack Speed");
                    SetNextInfoPoint("High Splash Damage");
                    SetNextInfoPoint("Multiple Projectiles");
                    SetNextInfoPoint("Weak vs Fast Targets");

                    damageGroup.Activate();
                    rangeGroup.Activate();
                    speedGroup.Activate();

                    damageGroup.Text = catapultTowerPrefab.StartingDamage.ToString();
                    rangeGroup.Text = (catapultTowerPrefab.StartingRange).ToString();

                    // Turn attack speed delay into attacks per second
                    float catapultAttackSpeed = GetAttacksPerSecond(catapultTowerPrefab.StartingAttackSpeed);

                    speedGroup.Text = $"{catapultAttackSpeed}/s";
                    break;

                case TowerType.MenAtArms:
                    towerTitle.text = "Militia Tower";

                    SetNextInfoPoint("High Health");
                    SetNextInfoPoint("Low Damage");
                    SetNextInfoPoint("Stops Ground Enemies");

                    damageGroup.Activate();
                    healthGroup.Activate();

                    damageGroup.Text = militiaTowerPrefab.startingUnitDamage.ToString();
                    healthGroup.Text = militiaTowerPrefab.startingUnitHealth.ToString();
                    break;

            }
        }

        // Convert attack speed delay to attacks per second
        private float GetAttacksPerSecond(float attackSpeed)
        {
            // Round to 2 decimal places
            double divided = (1 / attackSpeed);

            return (float)Math.Round(divided, 1);
        }

        private float GetDPSCurrent(float damage, float attackSpeed)
        {
            return damage * GetAttacksPerSecond(attackSpeed);
        }
    }

}