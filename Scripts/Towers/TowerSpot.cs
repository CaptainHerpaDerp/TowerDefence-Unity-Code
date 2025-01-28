using System.Collections.Generic;
using System;
using UnityEngine;
using Core;

namespace Towers
{
    public enum TowerPurchaseLevel
    {
        Available,
        Upgradable,
        FullyUpgraded
    }

    /// <summary>
    /// A spot on the map where a tower can be placed, contains the logic for purchasing, upgrading and selling towers and houses the tower itself when built
    /// </summary>
    public class TowerSpot : MonoBehaviour
    {
        // Reference to the sound effect manager so that tower construction sounds can be played
        SoundEffectManager soundEffectManager;

        [SerializeField] private CircleCollider2D mouseClickArea;
        [SerializeField] private GameObject hoverCircle;
        [SerializeField] private GameObject rangeCircle;
        [SerializeField] private GameObject upgradeRangeCircle;

        [SerializeField] private GameObject buildSite;

        [SerializeField] private GameObject archerTowerPrefab, mageTowerPrefab, menAtArmsTowerPrefab, bomberTowerPrefab;

        private Dictionary<TowerType, GameObject> towerTypePrefabPairs;

        // The total amount of money spent on the tower so that it can be sold for the correct amount
        public int MoneySpentOnTower = 0;

        [Header("Debugging")]
        [SerializeField] private bool constructOnStart = false;
        [SerializeField] private TowerType constructOnStartType;

        public Tower LinkedTower { get; private set; }

        public TowerPurchaseLevel TowerPurchaseLevel { get; private set; } = TowerPurchaseLevel.Available;

        public event Action<int> OnSell;

        public int TowerLevel { get; private set; } = 0;

        private void Start()
        {
            soundEffectManager = SoundEffectManager.Instance;

            towerTypePrefabPairs = new Dictionary<TowerType, GameObject>
            {
                { TowerType.Archer, archerTowerPrefab },
                { TowerType.Mage, mageTowerPrefab },
                { TowerType.MenAtArms, menAtArmsTowerPrefab },
                { TowerType.Bomber, bomberTowerPrefab }
            };

            if (constructOnStart)
            {
                PurchaseTower(constructOnStartType);
            }
        }

        public Vector3 GetTowerCenter()
        {
            return hoverCircle.transform.position;
        }

        public void ActivateHoverCircle()
        {
            if (hoverCircle != null && !hoverCircle.activeInHierarchy)
            {
                hoverCircle.SetActive(true);
            }
        }

        public void DeactivateHoverCircle()
        {
            if (hoverCircle != null && hoverCircle.activeInHierarchy)
            {
                hoverCircle.SetActive(false);
            }
        }

        public void ShowTowerRangeCircle()
        {
            rangeCircle.SetActive(true);

            float rangeScale = LinkedTower.AttackRange * 2;

            rangeCircle.transform.localScale = new Vector3 (rangeScale, rangeScale, 1);
        }

        public void ShowTowerRangeCircleWithRadius(float radius)
        {
            rangeCircle.SetActive(true);
            rangeCircle.transform.localScale = new Vector3(radius, radius, 1);
        }

        /// <summary>
        /// Previews the next upgrade level's range of the tower
        /// </summary>
        public void ShowTowerUpgradeRangeCircle()
        {
            // Do not display the upgrade range circle for MenAtArms towers
            if (LinkedTower.TowerType == TowerType.MenAtArms)
                return;

            upgradeRangeCircle.SetActive(true);

            // Calculate the range of the tower at the next level
            float rangeScale = LinkedTower.StartingRange + (LinkedTower.RangeIncreasePerUpgrade * TowerLevel);

            upgradeRangeCircle.transform.localScale = new Vector3(rangeScale * 2, rangeScale * 2, 1);
        }
 
        public void HideTowerUpgradeRangeCircle()
        {
            upgradeRangeCircle.SetActive(false);
        }

        public void HideRangeCircle()
        {
            upgradeRangeCircle.SetActive(false);
            rangeCircle.SetActive(false);
        }

        public void HideTowerUI()
        {
            DeactivateHoverCircle();
            HideRangeCircle();
        }

        public void PurchaseTower(TowerType type)
        {
            if (TowerPurchaseLevel != TowerPurchaseLevel.Available)
            {
                Debug.LogError("This tower spot has already been marked as purchased!");
                return;
            }

            TowerPurchaseLevel = TowerPurchaseLevel.Upgradable;

            // Disables the build site sprite
            buildSite.SetActive(false);

            // Instantiate the tower prefab based on the type
            GameObject newTower = Instantiate(towerTypePrefabPairs[type], transform.position, Quaternion.identity, parent: this.transform);

            // Assigns this tower spot's linked tower to the newly created tower
            LinkedTower = newTower.GetComponent<Tower>();

            TowerLevel = 1;
        }

        public void UpgradeTower()
        {
            if (LinkedTower == null)
            {
                Debug.LogError("No linked tower found!");
            }

            TowerLevel++;

            LinkedTower.UpgradeTower(TowerLevel);   
        }

        public bool IsUpgrading()
        {
            if (LinkedTower == null)
            {
                return false;
            }

            return LinkedTower.IsUpgrading();
        }

        /// <summary>
        /// Removes the tower from the tower spot and refunds the player the correct amount of gold, depending on wether the game has started or not
        /// </summary>
        public void SellTower()
        {
            OnSell.Invoke(MoneySpentOnTower);

            soundEffectManager.PlayTowerSellSound();

            ResetTower();
        }

        public void ResetTower()
        {
            // Removes any listeners from the OnSell event
            OnSell = null;

            MoneySpentOnTower = 0;

            TowerLevel = 0;

            if (LinkedTower != null)
            {
                LinkedTower.DestroyTower();
            }

            TowerPurchaseLevel = TowerPurchaseLevel.Available;

            // Enables the build site sprite
            buildSite.SetActive(true);
        }
    }
}

