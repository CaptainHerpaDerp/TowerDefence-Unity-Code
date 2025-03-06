
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;
using Saving;
using UIManagement;
using UIElements;
using AudioManagement;
using Sirenix.OdinInspector;

namespace GameManagement
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

        [BoxGroup("Waypoints"), SerializeField] WaypointGroup ForestPathWaypoint, FarmlandWaypoint, RiverCrossingWaypoint;
        [BoxGroup("Waypoints"), SerializeField] private List<WaypoinTrail> levelWaypointTrails = new();

        [BoxGroup("UI Panels"), SerializeField] private MenuLevelInfoUI MenuLevelInfoUI;

        [Header("The button attached to the level information window that will start the level")]
        [BoxGroup("Buttons"), SerializeField] private Button startLevelButton;

        [Header("The button attached to the level information window that will return to the level selection menu")]
        [BoxGroup("Buttons"), SerializeField] private Button returnLevelButton;

        [BoxGroup("Buttons"), SerializeField] private Button exitButton;
        [BoxGroup("Buttons"), SerializeField] private Button settingsButton;
        [BoxGroup("Buttons"), SerializeField] private GameObject blockExitButton;
        [BoxGroup("Buttons"), SerializeField] private GameObject blockSettingsButton;

        [BoxGroup("Keys"), SerializeField] private KeyCode exitKey = KeyCode.Escape;

        [BoxGroup("Debug"), SerializeField] private bool showAllLevels = false;

        private OpenMenu openMenu = OpenMenu.None;

        string selectedLevel;
        private List<WaypointGroup> waypointList;

        private bool levelStarted = false;
        private bool displayingLevelInfo = false;

        // Singletons
        private FadingPanelUI fadingPanelUI;
        private QuitConfirmation quitConfirmation;
        private GameSettingsWindow gameSettingsWindow;
        private AudioManager audioManager;
        private FMODEvents fmodEvents;
        private EventBus eventBus;
        private SaveManager saveManager;

        private void Start()
        {
            // Singleton Assignments
            fadingPanelUI = FadingPanelUI.Instance;
            quitConfirmation = QuitConfirmation.Instance;
            gameSettingsWindow = GameSettingsWindow.Instance;
            audioManager = AudioManager.Instance;
            fmodEvents = FMODEvents.Instance;
            saveManager = SaveManager.Instance;
            eventBus = EventBus.Instance;

            // Create the level list and add the levels to it
            waypointList = new List<WaypointGroup> { ForestPathWaypoint, FarmlandWaypoint, RiverCrossingWaypoint };

            // Add listeners to the level waypoints
            ForestPathWaypoint.buttonPressed += (DisplayForestPathLevelUI);
            FarmlandWaypoint.buttonPressed += (DisplayFarmlandLevelUI);
            RiverCrossingWaypoint.buttonPressed +=(DisplayRiverCrossingLevelUI);

            startLevelButton.onClick.AddListener(StartSelectedLevel);

            returnLevelButton.onClick.AddListener(() =>
            {
                    CloseSelectedLevel();
            });

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
            int completedLevels = saveManager.GetCompletedLevelCount();

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

            int newlyCompletedLevel = saveManager.NewlyCompletedLevelIndex();

            if (newlyCompletedLevel != -1)
            {
                Debug.Log("Newly completed level index: " + newlyCompletedLevel);

                WaypoinTrail nextWaypointTrail = levelWaypointTrails[newlyCompletedLevel];

                if (nextWaypointTrail == null || !nextWaypointTrail.gameObject.activeInHierarchy)
                    return;

                nextWaypointTrail.AnimateTrail();

                saveManager.SetNewlyCompletedLevel(-1);
            }
            
            DoWebGLPrefs();
        }

        private void OnEnable()
        {
            FadingPanelUI.Instance.UnfadePanel(1);

            // Save the game when the player returns from a level
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                SaveManager.Instance.SaveGame();
            }
        }

        private void Update()
        {
            // Check to see if the player presses the exit key
            CheckPlayerExitButton();
        }

        private void CheckPlayerExitButton()
        {
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
                    // If we are in WebGL, open the settings menu instead of the quit menu (can't quit in WebGL)
                    if (Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        OpenSettingsMenu();
                    }
                    else
                    {
                        OpenQuitMenu();
                    }
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

                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    audioManager.PlayOneShot(fmodEvents.gameStartSound, Vector2.zero);
                }

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
            audioManager.PlayOneShot(fmodEvents.hoverSound, Vector2.zero);
        }

        private void DoWebGLPrefs()
        {
            // Disable the quit button if we are in WebGL
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                exitButton.gameObject.SetActive(false);   
            }
        }
    }
}