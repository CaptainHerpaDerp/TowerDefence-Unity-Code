using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Towers;
using Interactables;
using Core;

namespace Management
{
    /// <summary>
    /// Detects mouse clicks and hovers over objects in the game world
    /// </summary>
    public class MouseClickDetection : MonoBehaviour
    {
        public static MouseClickDetection Instance;

        private TowerUpgradeManager TowerUpgradeManager;
        private PurchaseManager purchaseManager;
        private GameObject selectedTowerObj;
        private bool towerClicked = false;

        private bool positioningMilitiaWaypoint = false;

        private bool disableClicks = false;

        LevelEventManager levelEventManager;
        EventBus eventBus;

        bool isCursorOverUI = false;

        bool isGameWindowOpen = false;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            else
            {
                Debug.LogError("More than one MouseClickDetection instance in scene!");
                Destroy(this);
                return;
            }

            levelEventManager = LevelEventManager.Instance;
            eventBus = EventBus.Instance;

            eventBus.Subscribe("GameOver", DisableMouseUsage);
            eventBus.Subscribe("GameReset", EnableMouseUsage);

            eventBus.Subscribe("GamePaused", DisableMouseUsage);
            eventBus.Subscribe("GameUnpaused", EnableMouseUsage);

            eventBus.Subscribe("DisableMouseUsage", DisableMouseUsage);
            eventBus.Subscribe("EnableMouseUsage", EnableMouseUsage);

            eventBus.Subscribe("GameWindowOpened", () =>
            {
                // If positioning the rally point, stop positioning it
                positioningMilitiaWaypoint = false;

                isGameWindowOpen = true;
            });

            eventBus.Subscribe("GameWindowClosed", () =>
            {
                isGameWindowOpen = false;
            });

            //levelEventManager.OnGameOver += () => DisableMouseUsage();
            //levelEventManager.OnGameReset += () => EnableMouseUsage();

            //levelEventManager.OnGamePaused += () => DisableMouseUsage();
            //levelEventManager.OnGameUnpaused += () => EnableMouseUsage();

            if (levelEventManager == null)
            {
                Debug.LogError("No LevelEventManager found in scene");
            }

            TowerUpgradeManager = TowerUpgradeManager.Instance;

            if (TowerUpgradeManager == null)
            {
                Debug.LogError("No TowerUpgradeManager found in scene");
            }

            purchaseManager = PurchaseManager.Instance;

            if (purchaseManager == null)
            {
                Debug.LogError("No PurchaseManager found in scene");
            }

            else
            {
                TowerUpgradeManager.MageTowerOption.GetComponent<Button>().onClick.AddListener(PurchaseMageTower);
                TowerUpgradeManager.ArcherTowerOption.GetComponent<Button>().onClick.AddListener(PurchaseArcherTower);
                TowerUpgradeManager.MenAtArmsTowerOption.GetComponent<Button>().onClick.AddListener(PurchaseMenAtArmsTower);
                TowerUpgradeManager.BomberTowerOption.GetComponent<Button>().onClick.AddListener(PurchaseBomberTower);

                TowerUpgradeManager.UpgradeTowerOption.GetComponent<Button>().onClick.AddListener(() => UpgradeTower());
                TowerUpgradeManager.SellTowerOption.GetComponent<Button>().onClick.AddListener(() => SellTower());
                TowerUpgradeManager.MilitiaWaypointOption.GetComponent<Button>().onClick.AddListener(() => SetMilitiaWaypoint());
            }
        }

        private void OnEnable()
        {
            if (EventBus.Instance == null)
            {
                return;
            }

            EventBus.Instance.Subscribe("GameOver", DisableMouseUsage);
            EventBus.Instance.Subscribe("GameReset", EnableMouseUsage);

            EventBus.Instance.Subscribe("GamePaused", DisableMouseUsage);
            EventBus.Instance.Subscribe("GameUnpaused", EnableMouseUsage);
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe("GameOver", DisableMouseUsage);
            EventBus.Instance.Unsubscribe("GameReset", EnableMouseUsage);

            EventBus.Instance.Unsubscribe("GamePaused", DisableMouseUsage);
            EventBus.Instance.Unsubscribe("GameUnpaused", EnableMouseUsage);
        }

        public void EnableMouseUsage(object e)
        {
            disableClicks = false;
        }

        /// <summary>
        /// Enables the player to be able to interact with tower spots
        /// </summary>
        public void DisableMouseUsage(object e)
        {
            disableClicks = true;

            if (selectedTowerObj != null)
            {
                TowerSpot towerSpot = selectedTowerObj.GetComponent<TowerSpot>();
                towerSpot.HideTowerUI();
            }

            selectedTowerObj = null;

            towerClicked = false;

            // If visible, deactivate the tower upgrade UI
            TowerUpgradeManager.Deactivate();
        }

        void Update()
        {
            // If the player clicks the left mouse button at all
            if (Input.GetMouseButtonDown(0))
            {
                eventBus.Publish("MousePressAnyState");
            }

            if (Input.GetMouseButtonDown(1))
            {
                eventBus.Publish("RightMousePressAnyState");
            }

            CheckMouseOverClickableObjects();

            if (disableClicks)
                return;

            // If the player clicks the left mouse button while the game is not paused
            if (Input.GetMouseButtonDown(0))
            {
                eventBus.Publish("MousePressValidState");
            }

            if (Input.GetMouseButtonDown(1))
            {
                eventBus.Publish("RightMousePressValidState");
            }
        }

        #region Tower Purchase Methods

        private void PurchaseMageTower()
        {
            if (selectedTowerObj == null)
                return;

            if (selectedTowerObj.TryGetComponent(out TowerSpot towerSpot) && purchaseManager.BuyTower(TowerType.Mage, towerSpot))
            {
                PurchaseTowerOfType(TowerType.Mage);
            }
        }

        private void PurchaseArcherTower()
        {
            if (selectedTowerObj == null)
                return;

            if (selectedTowerObj.TryGetComponent(out TowerSpot towerSpot) && purchaseManager.BuyTower(TowerType.Archer, towerSpot))
            {
                PurchaseTowerOfType(TowerType.Archer);
            }
        }

        private void PurchaseMenAtArmsTower()
        {
            if (selectedTowerObj == null)
                return;

            if (selectedTowerObj.TryGetComponent(out TowerSpot towerSpot) && purchaseManager.BuyTower(TowerType.MenAtArms, towerSpot))
            {
                PurchaseTowerOfType(TowerType.MenAtArms);               
            }
        }

        private void PurchaseBomberTower()
        {
            if (selectedTowerObj == null)
                return;

            if (selectedTowerObj.TryGetComponent(out TowerSpot towerSpot) && purchaseManager.BuyTower(TowerType.Bomber, towerSpot))
            {
                PurchaseTowerOfType(TowerType.Bomber);
            }
        }

        private void PurchaseTowerOfType(TowerType type)
        {
            if (selectedTowerObj == null)
                return;

            if (!selectedTowerObj.TryGetComponent(out TowerSpot towerSpot))
            {
                Debug.LogWarning("Selected tower object does not have a TowerSpot component");
                return;
            }

            towerSpot.OnSell += purchaseManager.SellTower;

            towerSpot.PurchaseTower(type);

            towerSpot.DeactivateHoverCircle();
            selectedTowerObj = null;

            towerClicked = false;
            TowerUpgradeManager.Deactivate();
        }

        #endregion

        #region Tower Upgrade Methods

        private void UpgradeTower()
        {
            if (selectedTowerObj != null)
            {
                TowerSpot towerSpot = selectedTowerObj.GetComponent<TowerSpot>();

                if (towerSpot.IsUpgrading())
                {
                    Debug.Log("Tower is already upgrading");
                    StartCoroutine(QueueUpgrade(towerSpot));
                    return;
                }

                if (!purchaseManager.UpgradeTower(towerSpot.LinkedTower.TowerType, towerSpot.TowerLevel, towerSpot))
                    return;

                towerSpot.UpgradeTower();

                towerSpot.HideTowerUI();
                selectedTowerObj = null;

                towerClicked = false;
                TowerUpgradeManager.Deactivate();
            }
        }

        private IEnumerator QueueUpgrade(TowerSpot towerSpot)
        {
            while (true)
            {
                if (!towerSpot.IsUpgrading())
                {
                    if (!purchaseManager.UpgradeTower(towerSpot.LinkedTower.TowerType, towerSpot.TowerLevel, towerSpot))
                        yield break;

                    towerSpot.UpgradeTower();

                    towerSpot.HideTowerUI();
                    selectedTowerObj = null;

                    towerClicked = false;
                    TowerUpgradeManager.Deactivate();

                    yield break;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        private void SellTower()
        {
            if (selectedTowerObj == null)
                return;

            TowerSpot towerSpot = selectedTowerObj.GetComponent<TowerSpot>();

            towerSpot.HideTowerUI();
            selectedTowerObj = null;

            towerClicked = false;
            TowerUpgradeManager.Deactivate();

            towerSpot.SellTower();

        }

        private void SetMilitiaWaypoint()
        {
            if (selectedTowerObj == null)
                return;

            // If the player is already setting a waypoint, do not allow them to click the button again
            if (positioningMilitiaWaypoint)
                return;


            positioningMilitiaWaypoint = true;

            TowerUpgradeManager.Deactivate();

            TowerSpot towerSpot = selectedTowerObj.GetComponent<TowerSpot>();

            towerSpot.ShowTowerRangeCircle();

        }

        #endregion

        void CheckMouseOverClickableObjects()
        {
            if (EventSystem.current == null)
            {
                Debug.LogError("No EventSystem found in scene");
                return;
            }

            isCursorOverUI = EventSystem.current.IsPointerOverGameObject();

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (TowerUpgradeManager.IsActive() && isCursorOverUI)
            {
                // If the UI is active, do not process clicks on other towers
                return;
            }

            DoMilitiaWaypointPositioning(mousePosition);

            DoTowerInteraction(hit);
        }

        /// <summary>
        /// Detect if the mouse is hovering over a tower spot
        /// </summary>
        /// <param name="hit"></param>
        private void DoTowerInteraction(RaycastHit2D hit)
        {
            if (isGameWindowOpen)
                return;

            // If the mouse is hovering over a tower spot
            if (hit.collider != null && hit.collider.GetComponent<TowerSpot>())
            {
                // If the mouse is hovering over a tower spot
                GameObject hitObject = hit.collider.gameObject;
                TowerSpot towerSpot = hitObject.GetComponent<TowerSpot>();

                if (selectedTowerObj == null)
                {
                    selectedTowerObj = hitObject;
                    towerSpot.ActivateHoverCircle();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    // Checks to see if the player clicked on another tower spot while a tower spot is already selected
                    if (selectedTowerObj != hitObject)
                    {
                        // Hide all the ui of the old tower spot (Hover circle and range circle)
                        selectedTowerObj.GetComponent<TowerSpot>().HideTowerUI();

                        // Sets the new selected tower to the newly clicked on tower
                        selectedTowerObj = hitObject;

                        // Activates the hover circle of the newly clicked on tower
                        towerSpot.ActivateHoverCircle();

                        // Deactivates the tower upgrade UI in order to reset it
                        TowerUpgradeManager.Deactivate();
                    }

                    towerClicked = true;

                    if (towerSpot.TowerPurchaseLevel == TowerPurchaseLevel.Available)
                    {
                        TowerUpgradeManager.ActivateTowerPurchaseUIAtPosition(towerSpot);
                    }

                    if (towerSpot.TowerPurchaseLevel == TowerPurchaseLevel.Upgradable)
                    {
                        if (towerSpot.LinkedTower.TowerType != TowerType.MenAtArms)
                        {
                            towerSpot.ShowTowerRangeCircle();
                        }

                        TowerUpgradeManager.ActivateTowerUpgradeManagerAtPosition(towerSpot);
                    }
                }
            }

            else if (hit.collider != null && hit.collider.GetComponent<InteractableObject>())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    hit.collider.GetComponent<InteractableObject>().Interact();
                }
            }

            // If the mouse is not hovering over a tower spot
            else
            {
                // If the player did not select a tower spot
                if (!towerClicked)
                {
                    if (selectedTowerObj != null)
                    {
                        TowerSpot towerSpot = selectedTowerObj.GetComponent<TowerSpot>();

                        towerSpot.DeactivateHoverCircle();
                        towerSpot.HideRangeCircle();

                        selectedTowerObj = null;
                    }
                }

                // If the player is deselecting a tower spot           
                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                {
                    if (selectedTowerObj != null)
                    {
                        TowerSpot towerSpot = selectedTowerObj.GetComponent<TowerSpot>();

                        towerSpot.DeactivateHoverCircle();
                        towerSpot.HideRangeCircle();
                        selectedTowerObj = null;

                        towerClicked = false;

                        TowerUpgradeManager.Deactivate();
                    }
                }
            }
        }

        /// <summary>
        /// Set the selected tower's militia waypoint position to the mouse position
        /// </summary>
        private void DoMilitiaWaypointPositioning(Vector3 mousePosition)
        {
            // Do not do any waypoint positioning if the player is not currently setting a waypoint
            if (selectedTowerObj == null)
                return;

            if (positioningMilitiaWaypoint)
            {
                if (Input.GetMouseButton(0))
                {
                    selectedTowerObj.GetComponent<TowerSpot>().LinkedTower.GetComponent<MilitiaTower>().SetMilitiaWaypoint(mousePosition);
                    positioningMilitiaWaypoint = false;
                }

                if (Input.GetMouseButton(1))
                {
                    positioningMilitiaWaypoint = false;
                }

                TowerUpgradeManager.Disable();
                return;
            }
        }

    }
}