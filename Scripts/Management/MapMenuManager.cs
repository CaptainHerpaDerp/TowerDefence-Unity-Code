
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Core;
using UI;
using Saving;
using UI.Management;

namespace Management
{
    /// <summary>
    /// Manages the main menu map
    /// </summary>
    public class MapMenuManager : MonoBehaviour
    {
        private enum OpenMenu
        {
            None,
            Settings,
            Quit
        }

        [SerializeField] WaypointGroup ForestPathWaypoint, FarmlandWaypoint, RiverCrossingWaypoint;

        [SerializeField] private List<WaypoinTrail> levelWaypointTrails = new();

        private List<WaypointGroup> waypointList;

        [SerializeField] private MenuLevelInfoUI MenuLevelInfoUI;
        [SerializeField] private FadingPanelUI fadingPanelUI;
        [SerializeField] private float fadeSpeed;

        [Header("The button attached to the level information window that will start the level")]
        [SerializeField] private Button startLevelButton;

        [Header("The button attached to the level information window that will return to the level selection menu")]
        [SerializeField] private Button returnLevelButton;

        //private int completedLevels = 0;

        string selectedLevel;
        private bool displayingLevelInfo = false;

        private GameSettingsWindow gameSettingsWindow;
        private SoundEffectManager soundEffectManager;
        private EventBus eventBus;
        private SaveData saveData;

        [SerializeField] private bool showAllLevels = false;

        [SerializeField] private KeyCode exitKey = KeyCode.Escape;

        // UI 
        [Header("Buttons")]
        [SerializeField] private Button exitButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private GameObject blockExitButton;
        [SerializeField] private GameObject blockSettingsButton;

        private QuitConfirmation quitConfirmation;
        private OpenMenu openMenu = OpenMenu.None;

        private bool levelStarted = false;


        private void Start()
        {
            // Create the level list and add the levels to it
            waypointList = new List<WaypointGroup> { ForestPathWaypoint, FarmlandWaypoint, RiverCrossingWaypoint };

            // Retreive the save data
            saveData = SaveData.Instance;
            eventBus = EventBus.Instance;

            // Retreive references to the windows
            quitConfirmation = QuitConfirmation.Instance;
            gameSettingsWindow = GameSettingsWindow.Instance;

            // Add listeners to the level waypoints
            ForestPathWaypoint.buttonPressed += (DisplayForestPathLevelUI);
            FarmlandWaypoint.buttonPressed += (DisplayFarmlandLevelUI);
            RiverCrossingWaypoint.buttonPressed +=(DisplayRiverCrossingLevelUI);

            startLevelButton.onClick.AddListener(StartSelectedLevel);

            returnLevelButton.onClick.AddListener(() =>
            {
                    CloseSelectedLevel();
            });

            soundEffectManager = SoundEffectManager.Instance;

            exitButton.onClick.AddListener(OpenQuitMenu);
            settingsButton.onClick.AddListener(OpenSettingsMenu);

            // The cancel button on the quit menu is not connected to this script, therefore an event is used to close the menu
            eventBus.Subscribe("OnQuitDeclined", () =>
            {
                // Enable the exit and settings buttons
                blockExitButton.SetActive(false);
                blockSettingsButton.SetActive(false);

                exitButton.gameObject.SetActive(true);
                settingsButton.gameObject.SetActive(true);

                fadingPanelUI.UnfadePanelFromCurrent();
                openMenu = OpenMenu.None;
            });

            // In this case, this event is specific to the game settings window, so we know if it is called, the settings window is wishing to close
            eventBus.Subscribe("GameWindowClosed", () =>
            {
                print("listened to GameWindowClosed");

                fadingPanelUI.UnfadePanelFromCurrent();
                gameSettingsWindow.CloseSettings();
                openMenu = OpenMenu.None;
                return;
            });

            // Get the number of completed levels from the save data
            int completedLevels = saveData.GetCompletedLevelCount();

            // Disable all levels
            foreach (WaypointGroup level in waypointList)
            {
                level.Hide();
            }

            if (!showAllLevels)
            {

                // Only enable levels when the player has completed the previous of
                for (int i = 0; i < completedLevels + 1; i++)
                {
                    if (i >= waypointList.Count)
                        break;

                    waypointList[i].Enable();
                }
            }
            else
            {
                // Cancel if the next level doesnt exist
                if (completedLevels >= waypointList.Count)
                    return;

                for (int i = 0; i < waypointList.Count; i++)
                {
                    if (i >= waypointList.Count)
                        break;

                    waypointList[i].Enable();
                }
            }

            // Enable all the waypoint trails for the completed levels
            for (int i = 0; i < completedLevels; i++)
            {
                if (i >= levelWaypointTrails.Count)
                    break;

                levelWaypointTrails[i].EnableTrail();
            }

            print($"completed levels {completedLevels}");

            if (saveData.NewlyCompletedLevelIndex != -1)
            {
                Debug.Log("Newly completed level index: " + saveData.NewlyCompletedLevelIndex);

                WaypoinTrail nextWaypointTrail = levelWaypointTrails[saveData.NewlyCompletedLevelIndex];

                if (nextWaypointTrail == null || !nextWaypointTrail.gameObject.activeInHierarchy)
                    return;

                nextWaypointTrail.AnimateTrail();

                saveData.NewlyCompletedLevelIndex = -1;
            }

        }

        private void OnEnable()
        {
            fadingPanelUI.UnfadePanel(1);
        }

        private void Update()
        {
            //if (Input.GetMouseButtonDown(0) && displayingLevelInfo && !levelStarted)
            //{
            //    // Use the event system to check if the mouse is over a UI element
            //    PointerEventData pointerEventData = new PointerEventData(EventSystem.current);

            //    pointerEventData.position = Input.mousePosition;

            //    List<RaycastResult> raycastResults = new List<RaycastResult>();

            //    EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            //    bool hovering = false;
            //    foreach (RaycastResult raycastResult in raycastResults)
            //    {
            //        if (raycastResult.gameObject == MenuLevelInfoUI.MainPanel)
            //        {
            //            hovering = true;
            //            break;
            //        }
            //    }

            //    if (!hovering)
            //    {
            //        fadingPanelUI.UnfadePanelFromCurrent();
            //        MenuLevelInfoUI.HideUI();
            //        displayingLevelInfo = false;
            //    }
            //}

            if (Input.GetKeyDown(exitKey) && !levelStarted)
            {
                // If the player presses exit key while level info is displayed, close the level info
                if (displayingLevelInfo)
                {
                    CloseSelectedLevel();
                    return;
                }

                if (openMenu == OpenMenu.Settings)
                {
                    fadingPanelUI.UnfadePanelFromCurrent();
                    gameSettingsWindow.CloseSettings();
                    openMenu = OpenMenu.None;
                    return;
                }

                if (openMenu == OpenMenu.Quit)
                {
                    // Enable the exit and settings buttons
                    blockExitButton.SetActive(false);
                    blockSettingsButton.SetActive(false);

                    exitButton.gameObject.SetActive(true);
                    settingsButton.gameObject.SetActive(true);

                    quitConfirmation.DisableConfirmation();

                    fadingPanelUI.UnfadePanelFromCurrent();

                    openMenu = OpenMenu.None;
                }
                else
                {
                    OpenQuitMenu();
                }
            }
        }

        private void OpenSettingsMenu()
        {
            if (displayingLevelInfo)
            {
                CloseSelectedLevel();
            }

            // If the settings menu is already open, close it
            if (openMenu == OpenMenu.Settings)
            {
                fadingPanelUI.UnfadePanelFromCurrent();
                gameSettingsWindow.CloseSettings();
                openMenu = OpenMenu.None;
                return;
            }

            fadingPanelUI.FadePanel(0.8f, replaceDarkness: false);

            gameSettingsWindow.OpenSettings();
            openMenu = OpenMenu.Settings;
        }

        private void OpenQuitMenu()
        {
            if (displayingLevelInfo)
            {
                CloseSelectedLevel();
            }

            // If the settings menu is open, close it
            if (openMenu == OpenMenu.Settings)
            {
                gameSettingsWindow.QuickCloseSettings();
            }

            fadingPanelUI.FadePanel(0.8f, replaceDarkness: false);

            // Block the exit and settings buttons
            blockExitButton.SetActive(true);
            blockSettingsButton.SetActive(true);

            exitButton.gameObject.SetActive(false);
            settingsButton.gameObject.SetActive(false);

            quitConfirmation.EnableConfirmation();
            openMenu = OpenMenu.Quit;
        }

        private void CloseQuitMenu()
        {
            // Enable the exit and settings buttons
            blockExitButton.SetActive(false);
            blockSettingsButton.SetActive(false);

            exitButton.gameObject.SetActive(true);
            settingsButton.gameObject.SetActive(true);

            quitConfirmation.DisableConfirmation();

            fadingPanelUI.UnfadePanelFromCurrent();

            openMenu = OpenMenu.None;
        }

        private void DisplayLevelInfo()
        {
            // Enable the level info return button
            returnLevelButton.interactable = true;

            displayingLevelInfo = true;
        }

        private void DisplayForestPathLevelUI()
        {
            if (displayingLevelInfo)
                return;

            DisplayLevelInfo();

            MenuLevelInfoUI.DisplayUI(1, "Forest Path");
            selectedLevel = "Level 1";
            //  StartCoroutine(FadeAndLoad("Level 1"));
        }

        private void DisplayFarmlandLevelUI()
        {
            if (displayingLevelInfo)
                return;

            DisplayLevelInfo();

            MenuLevelInfoUI.DisplayUI(2, "Farmland");
            selectedLevel = "Level 2";
            //  StartCoroutine(FadeAndLoad("Level 2"));
        }

        private void DisplayRiverCrossingLevelUI()
        {
            if (displayingLevelInfo)
                return;

            DisplayLevelInfo();

            MenuLevelInfoUI.DisplayUI(3, "River Crossing");
            selectedLevel = "Level 3";
            //  StartCoroutine(FadeAndLoad("Level 3"));
        }

        private void StartSelectedLevel()
        {
            if (selectedLevel == "")
                return;

            else
            {
                // Stop the player from being able to click the start button again
                startLevelButton.interactable = false;
                levelStarted = true;


                soundEffectManager.PlayGameStartSound();
                MenuLevelInfoUI.HideUI();
                fadingPanelUI.FadePanelAndLoad(selectedLevel, false);
            }
        }

        /// <summary>
        /// Close the level information window
        /// </summary>
        private void CloseSelectedLevel()
        {
            fadingPanelUI.UnfadePanelFromCurrent();
            MenuLevelInfoUI.HideUI();
            displayingLevelInfo = false;
            returnLevelButton.interactable = false;
        }

        public void OnWaypointHover()
        {
            soundEffectManager.PlayHoverSound();
        }
    }
}