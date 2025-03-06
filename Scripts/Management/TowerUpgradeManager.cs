using UnityEngine;
using UnityEngine.UI;
using Towers;
using UIManagement;
using Core;
using Sirenix.OdinInspector;
using UIManagers;
using Utilities;    

namespace GameManagement
{
    /// <summary>
    /// Displays the tower purchase and upgrade options when a tower spot is selected
    /// </summary>
    public class TowerUpgradeManager : Singleton<TowerUpgradeManager>
    {
        [BoxGroup("Component References"), SerializeField] private RectTransform rectTransform;

        [BoxGroup("Tower UI Element Groups"), SerializeField] private TowerPurchaseGroup towerPurchaseGroup;
        [BoxGroup("Tower UI Element Groups"), SerializeField] private TowerUpgradeGroup towerUpgradeGroup;

        [Header("Fading Options")]
        [SerializeField] private float fadeTime;

        [Header("Tower Prefabs")]
        [SerializeField] private ArcherTower archerTower;
        [SerializeField] private MageTower mageTower;
        [SerializeField] private MilitiaTower militiaTower;
        [SerializeField] private CatapultTower catapultTower;

        [BoxGroup("Group Components"), SerializeField] private RectTransform purchaseGroupRectTransform, upgradeGroupRectTransform;

        private bool isFadingOut = false;
        private TowerSpot selectedTowerSpot;

        private bool isActive = false;

        [SerializeField] private Vector2 UIPurchaseOffsetPosition;

        [Header("The minimum and maximum Y values the purchase ui group can be at")]
        [SerializeField] private float purchaseUIMaxY;
        [SerializeField] private float purchaseUIMinY;

        [Header("The minimum and maximum Y values the upgrade ui group can be at")]
        [SerializeField] private float upgradeUIMaxY;
        [SerializeField] private float upgradeUIMinY;

        [SerializeField] private GridLayoutGroup purchaseLayoutGroupComponent;
        [SerializeField] private VerticalLayoutGroup upgradeLayoutGroupComponent;

        private RectTransform upgradeLayoutGroupRectTransform;

        // Singletons
        private PurchaseManager purchaseManager;
        private TowerInfoUI towerInfoUI;
        private EventBus eventBus;
    
        private void Start()
        {
            // Singleton Assignment
            purchaseManager = PurchaseManager.Instance;
            towerInfoUI = TowerInfoUI.Instance;
            eventBus = EventBus.Instance;

            rectTransform = GetComponent<RectTransform>();
            upgradeLayoutGroupRectTransform = upgradeLayoutGroupComponent.GetComponent<RectTransform>();

            if (towerPurchaseGroup.PurchaseOptions == null)
            {
                Debug.LogError("Tower purchase options not assigned to TowerUpgradeManager script");
            }

            // Set the button listeners for the purchase options
            foreach (var purchaseOption in towerPurchaseGroup.PurchaseOptions)
            {
                purchaseOption.Value.ButtonComponent.onClick.AddListener(() => PurchaseTowerOfType(purchaseOption.Key));
            }

            // Set the button listeners for the upgrade options
            towerUpgradeGroup.UpgradeButton.ButtonComponent.onClick.AddListener(() => UpgradeTower());
            towerUpgradeGroup.SellButton.ButtonComponent.onClick.AddListener(() => SellTower());
            towerUpgradeGroup.MilitiaWaypointButton.ButtonComponent.onClick.AddListener(() => SetMilitiaWaypoint());
        }

        private void Update()
        {
            if (!IsActive() || isFadingOut)
            {
                return;
            }

            // Deactivate the range circle every frame, if it is active
            DeactivatePreviewRangeCircle();
            CheckMouseOverOptions();
        }

        private void ActivatePreviewRangeCircle(float radius)
        {
            selectedTowerSpot.ShowTowerRangeCircleWithRadius(radius);
        }

        public void DeactivatePreviewRangeCircle()
        {
            if (selectedTowerSpot != null && towerPurchaseGroup.IsVisible())
                selectedTowerSpot.HideRangeCircle();
        }

        private void CheckMouseOverOptions()
        {
            // Go through each of the tower purchase buttons and see if they're selected
            foreach (var purchaseOption in towerPurchaseGroup.PurchaseOptions)
            {
                if (purchaseOption.Value.IsSelected)
                {
                    ShowTowerPurchaseInfo(purchaseOption.Key);
                    return;
                }
            }

            // Check if the upgrade button is selected
            if (towerUpgradeGroup.UpgradeButton.IsSelected)
            {
                // Display the upgraded range circle preview
                selectedTowerSpot.ShowTowerUpgradeRangeCircle();

                if (selectedTowerSpot.LinkedTower == null)
                {
                    Debug.LogError("Selected tower spot has no linked tower, cannot display tower upgrade info!");
                }

                else
                {
                    towerInfoUI.ActivateTowerUpgradeInfo(upgradeLayoutGroupRectTransform, rectTransform, selectedTowerSpot.LinkedTower.TowerType, selectedTowerSpot.TowerLevel);
                }

                return;
            }

            // At this point, no tower purchase or upgrade options are selected

            // Hide the purchase info and the range circle
            towerInfoUI.DeactivateTowerPurchaseInfo();

            if (selectedTowerSpot != null)
            selectedTowerSpot.HideTowerUpgradeRangeCircle();    
        }

        private void ShowTowerPurchaseInfo(TowerType towerType)
        {
            switch (towerType)
            {
                case TowerType.Archer:
                    ActivatePreviewRangeCircle(archerTower.StartingRange * 2);
                    towerInfoUI.ActivateTowerPurchaseInfo(purchaseGroupRectTransform, rectTransform, TowerType.Archer);
                    break;

                case TowerType.Mage:
                    ActivatePreviewRangeCircle(mageTower.StartingRange * 2);
                    towerInfoUI.ActivateTowerPurchaseInfo(purchaseGroupRectTransform, rectTransform, TowerType.Mage);
                    break;

                case TowerType.MenAtArms:
                    ActivatePreviewRangeCircle(militiaTower.StartingRange * 2);
                    towerInfoUI.ActivateTowerPurchaseInfo(purchaseGroupRectTransform, rectTransform, TowerType.MenAtArms);
                    break;

                case TowerType.Bomber:
                    ActivatePreviewRangeCircle(catapultTower.StartingRange * 2);
                    towerInfoUI.ActivateTowerPurchaseInfo(purchaseGroupRectTransform, rectTransform, TowerType.Bomber);
                    break;
            }
        }

        public bool IsActive()
        {
            if (towerPurchaseGroup.IsVisible() || towerUpgradeGroup.IsVisible())    
            {
                return true;
            }

            return false;
        }

        #region Activation Methods

        public void ActivateTowerPurchaseUIAtPosition(TowerSpot towerSpot)
        {
            isActive = true;
            isFadingOut = false;

            selectedTowerSpot = towerSpot;

            // Fade out the upgrade options group (if it is active)
            towerUpgradeGroup.QuickFadeOutGroup();

            StopAllCoroutines();

            transform.position = Camera.main.WorldToScreenPoint(towerSpot.GetTowerCenter());

            // Set the purchasing costs of all the tower types
            towerPurchaseGroup.SetPurchaseCosts(purchaseManager.PurchaseCosts);
            
            // Ensure the window is within the boundaries of the screen
            towerPurchaseGroup.KeepWindowInBoundaries();

            towerPurchaseGroup.FadeInGroup();
        }

        public void ActivateTowerUpgradeManagerAtPosition(TowerSpot towerSpot)
        {
            isActive = true;
            isFadingOut = false;
            
            selectedTowerSpot = towerSpot;

            // Fade out the purchase options group (if it is active)
            towerPurchaseGroup.QuickFadeOutGroup();

            StopAllCoroutines();

            TowerType type = towerSpot.LinkedTower.TowerType;
            int currentTowerLevel = towerSpot.TowerLevel;
            Vector3 UIPosition = towerSpot.GetTowerCenter() + (Vector3)UIPurchaseOffsetPosition;

            transform.position = Camera.main.WorldToScreenPoint(UIPosition);

            // Set the price labels to the correct values
            int sellValue = purchaseManager.GetRefundPercentage(towerSpot.MoneySpentOnTower);
            towerUpgradeGroup.SetPurchaseCosts(purchaseManager.GetUpgradeCost(type, currentTowerLevel), sellValue);

            // Set the tower info for the upgrade group
            towerUpgradeGroup.SetTowerInfo(type, currentTowerLevel);

            // Ensure the window is within the boundaries of the screen
            towerUpgradeGroup.KeepWindowInBoundaries();

            // Fade in the upgrade group
            towerUpgradeGroup.FadeInGroup();
        }

        #endregion

        #region Activation

        public void Deactivate()
        {
            if (!isActive)
            {
                return;
            }

            isActive = false;
            isFadingOut = true;

            DeactivatePreviewRangeCircle();

            towerPurchaseGroup.FadeOutGroup();
            towerUpgradeGroup.FadeOutGroup();

            towerInfoUI.DeactivateTowerPurchaseInfo();
        }

        public void Disable()
        {
            towerPurchaseGroup.QuickFadeOutGroup();
            towerUpgradeGroup.QuickFadeOutGroup();

            isActive = false;
        }

        #endregion

        #region Tower Purchase Methods

        private void PurchaseTowerOfType(TowerType type)
        {
            if (selectedTowerSpot == null)
            {
                Debug.LogError("No tower spot selected to purchase tower");
                return;
            }

            // If we cannot afford the tower, do not purchase it
            if (!purchaseManager.BuyTower(type, selectedTowerSpot))
            {
                // TODO: Show some visual feedback to the player that they cannot afford the tower
                return;
            }

            selectedTowerSpot.OnSell += purchaseManager.SellTower;

            selectedTowerSpot.PurchaseTower(type);

            // Deactivate the range circle and hover circle
            selectedTowerSpot.HideTowerUI();
            selectedTowerSpot = null;

            eventBus.Publish("TowerPurchasePerformed");

            // Close the window
            Deactivate();
        }

        #endregion

        #region Tower Upgrade Methods

        private void UpgradeTower()
        {
            if (selectedTowerSpot != null)
            {
                if (selectedTowerSpot.IsUpgrading())
                {
                    // Queue the tower upgrade if the tower is already upgrading
                    StartCoroutine(Utils.WaitConditionAndExecuteCR(() => !selectedTowerSpot.IsUpgrading(), () => UpgradeTower()));

                    Deactivate();

                    return;
                }

                if (!purchaseManager.UpgradeTower(selectedTowerSpot.LinkedTower.TowerType, selectedTowerSpot.TowerLevel, selectedTowerSpot))
                    return;

                selectedTowerSpot.UpgradeTower();

                selectedTowerSpot.HideTowerUI();
                selectedTowerSpot = null;

                eventBus.Publish("TowerPurchasePerformed");

                Deactivate();
            }
        }

        private void SellTower()
        {
            if (selectedTowerSpot == null)
                return;

            selectedTowerSpot.SellTower();

            selectedTowerSpot.HideTowerUI();
            selectedTowerSpot = null;

            eventBus.Publish("TowerPurchasePerformed");

            Deactivate();
        }

        private void SetMilitiaWaypoint()
        {
            if (selectedTowerSpot == null)
                return;

            eventBus.Publish("PositioningRallyPoint");

            Deactivate();

            selectedTowerSpot.ShowTowerRangeCircle();
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Returns true if any of the tower purchase or upgrade buttons are currenly selected by the player
        /// </summary>
        /// <returns></returns>
        public bool IsAnyButtonSelected()
        {
            // Check to see if any of the purchase buttons are selected
            foreach (var purchaseOption in towerPurchaseGroup.PurchaseOptions)
            {
                if (purchaseOption.Value.IsSelected)
                {
                    return true;
                }
            }

            // Check to see if any of the upgrade buttons are selected
            if (towerUpgradeGroup.IsAnyButtonSelected())
            {
                return true;
            }

            return false;
        }

        #endregion

    }
}