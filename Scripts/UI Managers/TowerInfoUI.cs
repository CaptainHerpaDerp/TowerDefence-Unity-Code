using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Towers;
using Core;
using Sirenix.OdinInspector;
using UIElements;


namespace UIManagement
{
    /// <summary>
    /// A UI element that displays upgrade and purchase information about towers.
    /// </summary>
    public class TowerInfoUI : Singleton<TowerInfoUI>
    {

        [BoxGroup("Component References"), SerializeField] private TextMeshProUGUI towerTitle;
        [BoxGroup("Component References"), SerializeField] private TowerInfoPoint towerInfoPointPrefab;
        [BoxGroup("Component References"), SerializeField] private DPSInfoPoint dpsInfoPoint;
        [BoxGroup("Component References"), SerializeField] private GameObject objectGroup;
        [BoxGroup("Component References"), SerializeField] private Transform towerInfoPointParent;
        [BoxGroup("Component References"), SerializeField] private RectTransform towerInfoGroupRectTransform;
        [BoxGroup("Component References"), SerializeField] private CanvasGroup canvasGroup;

        [BoxGroup("Component References"), SerializeField] private TowerInfoIcon damageGroup, rangeGroup, speedGroup, healthGroup;
        [BoxGroup("Component References"), SerializeField] private GameObject towerInfoIconParent;

        [BoxGroup("Prefab References"), SerializeField] private ArcherTower archerTowerPrefab;
        [BoxGroup("Prefab References"), SerializeField] private CatapultTower catapultTowerPrefab;
        [BoxGroup("Prefab References"), SerializeField] private MageTower mageTowerPrefab;
        [BoxGroup("Prefab References"), SerializeField] private MilitiaTower militiaTowerPrefab;

        [BoxGroup("Visual Settings"), SerializeField] private float offsetYAbove, offsetYBelow;
        [BoxGroup("Visual Settings"), SerializeField] private float canvasFadeInTime = 0.1f;
        [BoxGroup("Visual Settings"), SerializeField] private float canvasFadeOutTime = 0.1f;
        [BoxGroup("Visual Settings"), SerializeField] private float windowPaddingTop = 5;
        [BoxGroup("Visual Settings"), SerializeField] private float windowPaddingBottom = 5;




        // Object pooling variables
        private List<TowerInfoPoint> towerInfoPointPool;
        private const int InitialPoolSize = 5;

       private bool isActive;

        protected override void Awake()
        {
            base.Awake();
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
                int prefabIndex = towerInfoPointPrefab.transform.GetSiblingIndex();
                TowerInfoPoint infoPoint = Instantiate(towerInfoPointPrefab, towerInfoPointParent);
                infoPoint.transform.SetSiblingIndex(prefabIndex + 1);
                infoPoint.gameObject.SetActive(false);
                towerInfoPointPool.Add(infoPoint);
            }
        }

        public void DeactivateTowerPurchaseInfo()
        {
            FadeOutCanvas();
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
        private void SetNextInfoPoint(string text)
        {
            foreach (TowerInfoPoint point in towerInfoPointPool)
            {
                if (point.gameObject.activeInHierarchy && point.Text == text)
                    return;
            }
            foreach (TowerInfoPoint point in towerInfoPointPool)
            {
                if (!point.gameObject.activeInHierarchy)
                {
                    point.Text = text;
                    return;
                }
            }
            Debug.Log("All points are active or text already exists");
        }

        /// <summary>
        /// Sets the window's position and pivot based on screen position.
        /// </summary>
        private void SetPositionAndPivot(RectTransform windowRectTransform, RectTransform parentRect)
        {
            RectTransform rect = GetComponent<RectTransform>();

            // If the window is above the center of the screen, we need to set the position of the info panel to below the window
            if (parentRect.localPosition.y > 0)
            {
                // Get the height from the window's rect transform
                float windowHeight = windowRectTransform.rect.height;


                rect.transform.localPosition = new Vector2(0, (-windowHeight / 2) - windowPaddingTop);

                towerInfoGroupRectTransform.anchorMin = new Vector2(0.5f, 1);
                towerInfoGroupRectTransform.anchorMax = new Vector2(0.5f, 1);
                towerInfoGroupRectTransform.pivot = new Vector2(0.5f, 1);
            }

            // If the window is below the center of the screen, we need to set the position of the info panel to above the window
            else
            {
                // Get the height from the window's rect transform
                float windowHeight = windowRectTransform.rect.height;

                rect.transform.localPosition = new Vector2(0, (windowHeight / 2) + windowPaddingBottom);

                towerInfoGroupRectTransform.anchorMin = new Vector2(0.5f, 0);
                towerInfoGroupRectTransform.anchorMax = new Vector2(0.5f, 0);
                towerInfoGroupRectTransform.pivot = new Vector2(0.5f, 0);
            }
        }

        /// <summary>
        /// Common UI setup for positioning, deactivating info points,
        /// and setting the default or upgrade colors.
        /// </summary>
        private void SetupUI(RectTransform windowRectTransform, RectTransform parentRect, bool useDefaultColors, bool showTowerIcons)
        {
            objectGroup.SetActive(true);

            // Reset info groups
            damageGroup.Deactivate();
            rangeGroup.Deactivate();
            speedGroup.Deactivate();
            healthGroup.Deactivate();

            if (useDefaultColors)
            {
                damageGroup.SetColorDefault();
                rangeGroup.SetColorDefault();
                speedGroup.SetColorDefault();
                healthGroup.SetColorDefault();
            }
            else
            {
                damageGroup.SetColorUpgrade();
                rangeGroup.SetColorUpgrade();
                speedGroup.SetColorUpgrade();
                healthGroup.SetColorUpgrade();
            }

            DeactivateAllInfoPoints();
            towerInfoIconParent.SetActive(showTowerIcons);

            SetPositionAndPivot(windowRectTransform, parentRect);
        }

        ///// <summary>
        ///// Activates the tower info UI and sets the text based on the tower type and level.
        ///// </summary>
        //public void ActivateTowerInfo(Vector2 position, TowerType towerType, int towerLevel)
        //{
        //    Vector2 offset = new Vector2(0, GetWindowOffset(position));
        //    SetupUI(position, offset, true, true);

        //    switch (towerType)
        //    {
        //        case TowerType.Archer:
        //            towerTitle.text = "Archer Tower";
        //            damageGroup.Activate();
        //            rangeGroup.Activate();
        //            speedGroup.Activate();

        //            float currentDamage = archerTowerPrefab.StartingDamage + (archerTowerPrefab.DamageIncreasePerUpgrade * towerLevel);
        //            float currentRange = archerTowerPrefab.StartingRange + (archerTowerPrefab.RangeIncreasePerUpgrade * towerLevel);
        //            float currentAttackSpeed = archerTowerPrefab.StartingAttackSpeed - (archerTowerPrefab.AttackSpeedIncreasePerUpgrade * towerLevel);

        //            damageGroup.Text = currentDamage.ToString();
        //            rangeGroup.Text = currentRange.ToString();
        //            float archerAttackSpeed = GetAttacksPerSecond(currentAttackSpeed);
        //            speedGroup.Text = $"{archerAttackSpeed}/s";
        //            break;

        //        case TowerType.Mage:
        //            towerTitle.text = "Mage Tower";

        //            SetNextInfoPoint("Slow Attack Speed");
        //            SetNextInfoPoint("Low Range");
        //            SetNextInfoPoint("High Damage");

        //            damageGroup.Activate();
        //            rangeGroup.Activate();
        //            speedGroup.Activate();

        //            damageGroup.Text = mageTowerPrefab.StartingDamage.ToString();
        //            rangeGroup.Text = mageTowerPrefab.StartingRange.ToString();

        //            float mageAttackSpeed = GetAttacksPerSecond(mageTowerPrefab.StartingAttackSpeed);
        //            speedGroup.Text = $"{mageAttackSpeed}/s";
        //            break;

        //        case TowerType.Bomber:
        //            towerTitle.text = "Catapult Tower";

        //            SetNextInfoPoint("Slow Attack Speed");
        //            SetNextInfoPoint("High Splash Damage");
        //            SetNextInfoPoint("Multiple Projectiles");
        //            SetNextInfoPoint("Weak vs Fast Targets");

        //            damageGroup.Activate();
        //            rangeGroup.Activate();
        //            speedGroup.Activate();

        //            damageGroup.Text = catapultTowerPrefab.StartingDamage.ToString();
        //            rangeGroup.Text = catapultTowerPrefab.StartingRange.ToString();

        //            float catapultAttackSpeed = GetAttacksPerSecond(catapultTowerPrefab.StartingAttackSpeed);
        //            speedGroup.Text = $"{catapultAttackSpeed}/s";
        //            break;

        //        case TowerType.MenAtArms:
        //            towerTitle.text = "Militia Tower";

        //            SetNextInfoPoint("High Health");
        //            SetNextInfoPoint("Low Damage");
        //            SetNextInfoPoint("Stops Ground Enemies");

        //            damageGroup.Activate();
        //            healthGroup.Activate();

        //            damageGroup.Text = militiaTowerPrefab.startingUnitDamage.ToString();
        //            healthGroup.Text = militiaTowerPrefab.startingUnitHealth.ToString();
        //            break;
        //    }
        //}

        /// <summary>
        /// Activates the tower upgrade info UI and sets the text based on the tower type and level.
        /// </summary>
        public void ActivateTowerUpgradeInfo(RectTransform windowRectTransform, RectTransform parentRect, TowerType towerType, int towerLevel)
        {
            int previewTowerLevel = towerLevel + 1;
            SetupUI(windowRectTransform, parentRect, false, false);

            float attackSpeedIncreasePercentage, rangeIncreasePercentage, damageIncreasePercentage;
            float currentDPS, addedDPS;

            switch (towerType)
            {
                case TowerType.Archer:
                    towerTitle.text = "Archer Level " + previewTowerLevel;

                    attackSpeedIncreasePercentage = (float)Math.Round(((archerTowerPrefab.AttackSpeedIncreasePerUpgrade / archerTowerPrefab.StartingAttackSpeed) * 100), 1);
                    rangeIncreasePercentage = (float)Math.Round(((archerTowerPrefab.RangeIncreasePerUpgrade / archerTowerPrefab.StartingRange) * 100), 1);

                    currentDPS = (float)Math.Round(
                        (archerTowerPrefab.StartingDamage + (archerTowerPrefab.DamageIncreasePerUpgrade * (towerLevel - 1))) /
                        (archerTowerPrefab.StartingAttackSpeed - (archerTowerPrefab.AttackSpeedIncreasePerUpgrade * (towerLevel - 1))), 1);

                    addedDPS = (float)Math.Round(
                        (archerTowerPrefab.StartingDamage + (archerTowerPrefab.DamageIncreasePerUpgrade * (previewTowerLevel - 1))) /
                        (archerTowerPrefab.StartingAttackSpeed - (archerTowerPrefab.AttackSpeedIncreasePerUpgrade * (previewTowerLevel - 1))), 1);

                    SetNextInfoPoint($"Attack Speed +{attackSpeedIncreasePercentage}%");
                    SetNextInfoPoint($"Range +{rangeIncreasePercentage}%");

                    dpsInfoPoint.SetDPSToFromText(currentDPS, addedDPS);
                    break;

                case TowerType.Bomber:
                    towerTitle.text = "Catapult Level " + previewTowerLevel;

                    attackSpeedIncreasePercentage = (float)Math.Round(((catapultTowerPrefab.AttackSpeedIncreasePerUpgrade / catapultTowerPrefab.StartingAttackSpeed) * 100), 1);
                    damageIncreasePercentage = (float)Math.Round(((catapultTowerPrefab.DamageIncreasePerUpgrade / catapultTowerPrefab.StartingDamage) * 100), 1);
                    rangeIncreasePercentage = (float)Math.Round(((catapultTowerPrefab.RangeIncreasePerUpgrade / catapultTowerPrefab.StartingRange) * 100), 1);

                    currentDPS = (float)Math.Round(
                        (catapultTowerPrefab.StartingDamage + (catapultTowerPrefab.DamageIncreasePerUpgrade * (towerLevel - 1))) /
                        (catapultTowerPrefab.StartingAttackSpeed - (catapultTowerPrefab.AttackSpeedIncreasePerUpgrade * (towerLevel - 1))), 1);

                    addedDPS = (float)Math.Round(
                        (catapultTowerPrefab.StartingDamage + (catapultTowerPrefab.DamageIncreasePerUpgrade * (previewTowerLevel - 1))) /
                        (catapultTowerPrefab.StartingAttackSpeed - (catapultTowerPrefab.AttackSpeedIncreasePerUpgrade * (previewTowerLevel - 1))), 1);

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
                    for (int i = catapultTowerPrefab.ProjectileChanges.Count - 1; i > 0; i--)
                    {
                        nextProjectiles = catapultTowerPrefab.ProjectileChanges[i].numberOfProjectiles;
                        if (previewTowerLevel == catapultTowerPrefab.ProjectileChanges[i].level)
                        {
                            SetNextInfoPoint("Projectiles +1");
                            Debug.Log($"Number of projectiles: {numberOfProjectiles}, TowerLevel: {previewTowerLevel}");
                            break;
                        }
                    }
                    dpsInfoPoint.SetDPSToFromText(currentDPS * numberOfProjectiles, addedDPS * nextProjectiles);
                    break;

                case TowerType.Mage:
                    towerTitle.text = "Mage Level " + previewTowerLevel;

                    float mageDamageIncreasePercentage = (float)Math.Round(((mageTowerPrefab.DamageIncreasePerUpgrade / mageTowerPrefab.StartingDamage) * 100), 1);
                    float mageRangeIncreasePercentage = (float)Math.Round(((mageTowerPrefab.RangeIncreasePerUpgrade / mageTowerPrefab.StartingRange) * 100), 1);

                    currentDPS = (float)Math.Round(
                        (mageTowerPrefab.StartingDamage + (mageTowerPrefab.DamageIncreasePerUpgrade * (towerLevel - 1))) /
                        (mageTowerPrefab.StartingAttackSpeed - (mageTowerPrefab.AttackSpeedIncreasePerUpgrade * (towerLevel - 1))), 1);

                    addedDPS = (float)Math.Round(
                        (mageTowerPrefab.StartingDamage + (mageTowerPrefab.DamageIncreasePerUpgrade * (previewTowerLevel - 1))) /
                        (mageTowerPrefab.StartingAttackSpeed - (mageTowerPrefab.AttackSpeedIncreasePerUpgrade * (previewTowerLevel - 1))), 1);

                    SetNextInfoPoint($"Damage +{mageDamageIncreasePercentage}%");
                    SetNextInfoPoint($"Range +{mageRangeIncreasePercentage}%");

                    dpsInfoPoint.SetDPSToFromText(currentDPS, addedDPS);
                    break;

                case TowerType.MenAtArms:
                    towerTitle.text = "Militia Level " + previewTowerLevel;

                    float militiaDamageIncreasePercentage = (float)Math.Round(((militiaTowerPrefab.UnitDamageIncreasePerUpgrade / militiaTowerPrefab.StartingUnitDamage) * 100), 1);
                    float militiaHealthIncreasePercentage = (float)Math.Round(((militiaTowerPrefab.UnitHealthIncreasePerUpgrade / militiaTowerPrefab.StartingUnitHealth) * 100), 1);

                    SetNextInfoPoint($"Health +{militiaHealthIncreasePercentage}%");
                    SetNextInfoPoint($"Damage +{militiaDamageIncreasePercentage}%");

                    towerInfoIconParent.SetActive(true);
                    damageGroup.Activate();
                    healthGroup.Activate();

                    float currentMilitiaDamage = militiaTowerPrefab.StartingUnitDamage + (militiaTowerPrefab.UnitDamageIncreasePerUpgrade * (previewTowerLevel - 1));
                    float currentMilitiaHealth = militiaTowerPrefab.StartingUnitHealth + (militiaTowerPrefab.UnitHealthIncreasePerUpgrade * (previewTowerLevel - 1));

                    // Example of conditional color setting
                    if (currentMilitiaDamage != militiaTowerPrefab.StartingUnitDamage + (militiaTowerPrefab.UnitDamageIncreasePerUpgrade * (previewTowerLevel - 1)))
                        damageGroup.SetColorUpgrade();
                    else
                        damageGroup.SetColorDefault();

                    if (currentMilitiaHealth != militiaTowerPrefab.StartingUnitHealth + (militiaTowerPrefab.UnitDamageIncreasePerUpgrade * (previewTowerLevel - 1)))
                        healthGroup.SetColorUpgrade();
                    else
                        healthGroup.SetColorDefault();

                    damageGroup.Text = currentMilitiaDamage.ToString();
                    healthGroup.Text = currentMilitiaHealth.ToString();
                    break;
            }

            FadeInCanvas();
        }

        /// <summary>
        /// Activates the tower purchase info UI and sets the text based on the tower type.
        /// </summary>
        public void ActivateTowerPurchaseInfo(RectTransform windowRectTransform, RectTransform parentRect, TowerType type)
        {
            //Vector2 offset = new Vector2(0, GetWindowOffset(position));
            SetupUI(windowRectTransform, parentRect, true, true);

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
                    rangeGroup.Text = archerTowerPrefab.StartingRange.ToString();

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
                    rangeGroup.Text = mageTowerPrefab.StartingRange.ToString();

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
                    rangeGroup.Text = catapultTowerPrefab.StartingRange.ToString();

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

                    damageGroup.Text = militiaTowerPrefab.StartingUnitDamage.ToString();
                    healthGroup.Text = militiaTowerPrefab.StartingUnitHealth.ToString();
                    break;
            }

            FadeInCanvas();
        }

        /// <summary>
        /// Converts an attack speed delay into attacks per second.
        /// </summary>
        private float GetAttacksPerSecond(float attackSpeed)
        {
            return (float)Math.Round(1 / attackSpeed, 1);
        }

        #region Canvas Fading

        private void FadeInCanvas()
        {
            if (isActive)
                return;

            isActive = true;

            // Ensure no other fading coroutines are running
            StopAllCoroutines();
            StartCoroutine(Utilities.Utils.FadeInCanvasGroup(canvasGroup, canvasFadeInTime));
        }

        private void FadeOutCanvas()
        {
            if (!isActive)
            return;

            isActive = false;

            // Ensure no other fading coroutines are running
            StopAllCoroutines();
            StartCoroutine(Utilities.Utils.FadeOutCanvasGroup(canvasGroup, canvasFadeOutTime));
        }


        #endregion
    }
}
