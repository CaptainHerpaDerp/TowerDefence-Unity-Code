using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Towers;
using Interactables;
using Core;
using Sirenix.OdinInspector;

namespace GameManagement
{
    /// <summary>
    /// Detects mouse clicks and hovers over objects in the game world
    /// </summary>
    public class MouseClickDetection : Singleton<MouseClickDetection>
    {
        private bool towerClicked = false;
        private bool disableClicks = false;
        private bool isGameWindowOpen = false;

        // Singletons 
        private EventBus eventBus;
        private TowerUpgradeManager towerUpgradeManager;

        // Hover tower spot tracks which tower spot the player is hovering their mouse over
        [ShowInInspector, ReadOnly] private TowerSpot hoveredTowerSpot;

        // Selected tower spot tracks which tower the player has opened the purchase/upgrade UI for
        [ShowInInspector, ReadOnly] private TowerSpot selectedTowerSpot;

        private bool isPositioningRallyPoint;

        private void Start()
        {
            // Singleton Assignment
            eventBus = EventBus.Instance;
            towerUpgradeManager = TowerUpgradeManager.Instance;

            eventBus.Subscribe("GameOver", DisableMouseUsage);
            eventBus.Subscribe("GameReset", EnableMouseUsage);

            eventBus.Subscribe("GamePaused", DisableMouseUsage);
            eventBus.Subscribe("GameUnpaused", EnableMouseUsage);

            eventBus.Subscribe("DisableMouseUsage", DisableMouseUsage);
            eventBus.Subscribe("EnableMouseUsage", EnableMouseUsage);

            eventBus.Subscribe("GameWindowOpened", () =>
            {
                // If positioning the rally point, stop positioning it
                isPositioningRallyPoint = false;

                isGameWindowOpen = true;
            });

            eventBus.Subscribe("GameWindowClosed", () =>
            {
                isGameWindowOpen = false;
            });

            eventBus.Subscribe("PositioningRallyPoint", () =>
            {
                isPositioningRallyPoint = true;
            });


            eventBus.Subscribe("MouseEnterTowerSpot", OnTowerHovered);
            eventBus.Subscribe("MouseExitTowerSpot", OnTowerSpotExitHover);

            eventBus.Subscribe("TowerPurchasePerformed", () =>
            {
                if (selectedTowerSpot != null)
                {
                    selectedTowerSpot = null;
                }
            });
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

            if (selectedTowerSpot != null)
            {
                selectedTowerSpot.HideTowerUI();
            }

            selectedTowerSpot = null;

            towerClicked = false;

            // If visible, deactivate the tower upgrade UI
            towerUpgradeManager.Deactivate();
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
                // Cancel militia placement
                if (isPositioningRallyPoint && selectedTowerSpot != null)
                {
                    selectedTowerSpot.HideTowerUI();
                    selectedTowerSpot = null;
                    isPositioningRallyPoint = false;
                }

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

        #region Tower Interaction

        private void OnTowerHovered(object towerSpotObject)
        {
            // Do not allow the the player to hover over any towers if the're repositioning the militia rally point
            if (isPositioningRallyPoint)
            {
                return;
            }

            // Try to cast the object to a TowerSpot
            TowerSpot towerSpot = towerSpotObject as TowerSpot;
            if (towerSpot == null)
            {
                Debug.LogError("TowerSpot object is null");
                return;
            }

            // Highlight the tower spot
            towerSpot.ActivateHoverCircle();

            // Set the hovered tower spot as the selected tower spot if no tower spot is already selected
            // (This allows a tower spot to be hovered over even if we're actively viewing the purchasing UI for another tower spot)
            hoveredTowerSpot = towerSpot;
        }

        private void OnTowerSpotExitHover(object towerSpotObject)
        {
            // Try to cast the object to a TowerSpot
            TowerSpot towerSpot = towerSpotObject as TowerSpot;
            if (towerSpot == null)
            {
                Debug.LogError("TowerSpot object is null");
                return;
            }

            towerSpot.DeactivateHoverCircle();

            if (hoveredTowerSpot == towerSpot)
            {
                hoveredTowerSpot = null;
            }
        }

        #endregion

        void CheckMouseOverClickableObjects()
        {
            // Check to see if the player presses the left mouse button
            if (Input.GetMouseButtonDown(0))
            {
                // If the player is hovering over a tower spot, activate the tower purchase/upgrade UI
                if (hoveredTowerSpot != null)
                {
                    // If we're clicking on the same spot that is already selected, return
                    if (hoveredTowerSpot == selectedTowerSpot)
                    {
                        return;
                    }

                    // If we are currently selecting a tower spot, we need to hide any ui before reassigning it
                    if (selectedTowerSpot != null)
                    {
                        selectedTowerSpot.HideTowerUI();
                    }

                    // Since we are opening the upgrade UI, set the selected tower spot to the hovered tower spot
                    selectedTowerSpot = hoveredTowerSpot;

                    // Now activate the relevant UI for the tower spot
                    if (selectedTowerSpot.TowerPurchaseLevel == TowerPurchaseLevel.Available)
                    {
                        towerUpgradeManager.ActivateTowerPurchaseUIAtPosition(selectedTowerSpot);
                    }

                    if (selectedTowerSpot.TowerPurchaseLevel == TowerPurchaseLevel.Upgradable)
                    {
                        if (selectedTowerSpot.LinkedTower.TowerType != TowerType.MenAtArms)
                        {
                            selectedTowerSpot.ShowTowerRangeCircle();
                        }

                        towerUpgradeManager.ActivateTowerUpgradeManagerAtPosition(selectedTowerSpot);
                    }
                }

                // Otherwise, the player is not hovering over a tower
                else
                {
                    /* If the player isn't hovering over a button in the tower upgrade UI,
                     we can hide the UI altogether as the player isnt hovering over anything relevant */
                    if (!towerUpgradeManager.IsAnyButtonSelected())
                    {
                        // Deactivate the tower upgrade UI
                        towerUpgradeManager.Deactivate();

                        // Deactivate any tower spot UI
                        if (selectedTowerSpot != null)
                        {
                            selectedTowerSpot.HideTowerUI();
                        }

                        // The player may be 'repositioning the militia tower rally point, in this case all previous logic applies, and we need to set the rally point
                        DoMilitiaWaypointPositioning();

                        // Finally, nullify the selected tower spot
                        selectedTowerSpot = null;
                    }
                }
            }
        }

        /// <summary>
        /// Set the selected tower's militia waypoint position to the mouse position
        /// </summary>
        private void DoMilitiaWaypointPositioning()
        {
            // Do not do any waypoint positioning if the player is not currently selecting a tower
            if (selectedTowerSpot == null)
            {
                isPositioningRallyPoint = false;
                return;
            }

            if (isPositioningRallyPoint)
            {
                if (Input.GetMouseButton(0))
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    selectedTowerSpot.LinkedTower.GetComponent<MilitiaTower>().SetMilitiaWaypoint(mousePosition);
                    isPositioningRallyPoint = false;

                    selectedTowerSpot = null;
                }

                if (Input.GetMouseButton(1))
                {
                    isPositioningRallyPoint = false;

                    selectedTowerSpot = null;
                }

                towerUpgradeManager.Disable();
                return;
            }
        }
    }
}