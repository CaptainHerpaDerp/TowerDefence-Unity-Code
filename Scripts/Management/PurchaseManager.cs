using System.Collections.Generic;
using UnityEngine;
using Towers;
using UIManagement;
using Analytics;
using Core;

namespace GameManagement
{
    /// <summary>
    /// Manages the purchase and upgrade of towers
    /// </summary>
    public class PurchaseManager : Singleton<PurchaseManager>
    {
        private int gold;

        private bool hasInfiniteMoney;

        public List<TowerType> BlacklistedTowers = new();

        private AnalyticsManager analyticsManager;

        public bool HasInfiniteMoney
        {
            get { return hasInfiniteMoney; }

            set
            {
                hasInfiniteMoney = value;

                if (value)
                {
                    GuiManager.Instance.UpdateGoldValue(9999);
                }
                else
                {
                    GuiManager.Instance.UpdateGoldValue(gold);
                }
            }
        }

        public float RefundPercentage { get; private set; } = 0.6f;

        private Dictionary<TowerType, int> purchaseCosts, upgradeCosts;

        public Dictionary<TowerType, int> PurchaseCosts => purchaseCosts;

        public int Gold
        {
            get { return gold; }
            set
            {
                gold = value;

                GuiManager.Instance.UpdateGoldValue(gold);
            }
        }

        private void Start()
        {
            if (purchaseCosts == null || upgradeCosts == null)
                SetDictionaries();

            analyticsManager = AnalyticsManager.Instance;
        }

        private void SetDictionaries()
        {
            purchaseCosts = new Dictionary<TowerType, int>
        {
            { TowerType.Archer, archerTowerPrefab.GetComponent<Tower>().PurchaseCost },
            { TowerType.Mage, mageTowerPrefab.GetComponent<Tower>().PurchaseCost },
            { TowerType.Bomber, catapultTowerPrefab.GetComponent<Tower>().PurchaseCost },
            { TowerType.MenAtArms, militiaTowerPrefab.GetComponent<Tower>().PurchaseCost }
        };

            upgradeCosts = new Dictionary<TowerType, int>
        {
            { TowerType.Archer, archerTowerPrefab.GetComponent<Tower>().TotalUpgradeCost },
            { TowerType.Mage, mageTowerPrefab.GetComponent<Tower>().TotalUpgradeCost },
            { TowerType.Bomber, catapultTowerPrefab.GetComponent<Tower>().TotalUpgradeCost },
            { TowerType.MenAtArms, militiaTowerPrefab.GetComponent<Tower>().TotalUpgradeCost }
        };
        }

        [SerializeField] private GameObject archerTowerPrefab, mageTowerPrefab, militiaTowerPrefab, catapultTowerPrefab;

        // The level of tower upgrades after the first, used to divide the total upgrade variable.
        private const int totalTowerUpgradeLevels = 6;

        #region Getter and Setter Methods

        public int GetPurchaseCost(TowerType type)
        {
            return purchaseCosts[type];
        }

        public int GetUpgradeCost(TowerType type, int currentLevel)
        {
            return GetUpgradeCostMultiplier(currentLevel, upgradeCosts[type]);
        }

        public void SetPurchaseCost(TowerType type, int cost)
        {
            if (purchaseCosts == null)
                SetDictionaries();

            purchaseCosts[type] = cost;
        }

        public void SetTotalUpgradeCost(TowerType type, int cost)
        {
            if (upgradeCosts == null)
                SetDictionaries();

            upgradeCosts[type] = cost;
        }

        #endregion

        #region Tower Management Methods
        public void SellTower(int totalValue)
        {
            if (LevelEventManager.Instance == null)
            {
                Debug.LogError("No LevelEventManager could be found in the scene");
                return;
            }

            if (LevelEventManager.Instance.GameStarted)
            {
                Gold += Mathf.RoundToInt(totalValue * RefundPercentage);
            }
            else
            {
                Gold += totalValue;
            }
        }

        public bool UpgradeTower(TowerType type, int currentLevel, TowerSpot towerSpot)
        {
            if (hasInfiniteMoney)
            {
                towerSpot.MoneySpentOnTower += GetUpgradeCost(type, currentLevel);

                return true;
            }

            if (GetUpgradeCost(type, currentLevel) <= gold)
            {   
                Gold -= GetUpgradeCost(type, currentLevel);

                towerSpot.MoneySpentOnTower += GetUpgradeCost(type, currentLevel);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true and spends the required gold if the tower can be bought, otherwise return false
        /// </summary>
        /// <param name="type"></param>
        public bool BuyTower(TowerType type, TowerSpot towerSpot)
        {
            if (BlacklistedTowers.Contains(type))
            {
                Debug.LogWarning("Tower type is blacklisted and cannot be purchased");
                return false;
            }

            if (hasInfiniteMoney)
            {
                towerSpot.MoneySpentOnTower += GetPurchaseCost(type);

                analyticsManager.SentTowerTypeConstructed(type);

                return true;
            }

            if (GetPurchaseCost(type) <= gold)
            {
                Gold -= GetPurchaseCost(type);

                towerSpot.MoneySpentOnTower += GetPurchaseCost(type);

                analyticsManager.SentTowerTypeConstructed(type);

                return true;
            }

            return false;
        }

        #endregion

        public int GetRefundPercentage(int totalValue)
        {
            if (LevelEventManager.Instance == null)
            {
                Debug.LogError("No LevelEventManager could be found in the scene");
                return 0;
            }

            if (LevelEventManager.Instance.GameStarted)
            {
                return Mathf.RoundToInt(totalValue * RefundPercentage);
            }
            else
            {
                return totalValue;
            }
        }

        // Helper method to calculate the upgrade cost multiplier
        private int GetUpgradeCostMultiplier(int currentLevel, int TotalUpgradeCost)
        {
            const int totalTowerUpgradeLevels = 7; // Adjust the total number of levels as needed

            // Example formula for increasing cost per level (you can adjust this formula based on your preferences)
            int baseCost = Mathf.RoundToInt(TotalUpgradeCost / (2 * totalTowerUpgradeLevels)); // Initial cost
            int additionalCost = Mathf.RoundToInt(baseCost * 0.5f * currentLevel); // Additional cost per level

            return baseCost + additionalCost;
        }
    }
}