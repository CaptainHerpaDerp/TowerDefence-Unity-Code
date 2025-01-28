using Core;
using Enemies;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Management
{
    public class GameWindowManager : MonoBehaviour
    {
        public static GameWindowManager Instance;

        #region Serialized Fields
        // Buttons
        [SerializeField] private Button pauseButton, playButton;

        // Block Buttons (for disabling interactions)
        [SerializeField] private GameObject blockPauseButton;

        // A panel that can be enabled to disable mouse interactions with low-level ui elements
        [SerializeField] private GameObject mouseBlockerPanel;

        // Delays
        [SerializeField] private float windowOpenPauseDelay;
        [SerializeField] private float levelStartPauseDelay;

        // Key Bindings
        [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
        #endregion

        #region Private Fields
        private GameSettingsWindow gameSettingsWindow;
        private FadingPanelUI fadingPanel;

        private MagicPowerManager magicPowerManager;

        private EventBus eventBus;
        private NewEnemyDisplay newEnemyDisplay;

        private PostLevelInfoUI postLevelInfoUI;
        private GameLossUI gameLossUI;

        private bool pauseEnabled = false;
        private bool inTransition = false;

        private bool isEnemyInfoOpen = false;

        // If the game is over, no windows can be opened
        private bool isGameOver = false;

        private float transitionTime = 0.5f;
        #endregion  

        #region Unity Methods
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("More than one GameWindowManager instance in scene!");
                Destroy(this);
                return;
            }
        }

        private void Start()
        {
            StartCoroutine(WaitTransitionTime());

            // Retrieve the event bus instance
            eventBus = EventBus.Instance;

            // Subscribe to events
            SubscribeToEvents();

            // Retrieve instances of the GUI windows
            gameSettingsWindow = GameSettingsWindow.Instance;
            fadingPanel = FadingPanelUI.Instance;
            newEnemyDisplay = NewEnemyDisplay.Instance;
            magicPowerManager = MagicPowerManager.Instance;
            postLevelInfoUI = PostLevelInfoUI.Instance;
            gameLossUI = GameLossUI.Instance;

            // Assign button click events
            AssignButtonListeners();
        }

        private void Update()
        {
            // Pause the game when the pause key is pressed, or close any windows that are open
            if (Input.GetKeyDown(pauseKey))
            {
                if (isEnemyInfoOpen)
                {
                    CloseEnemyInfo();
                }
                else
                {
                    HandlePauseKey();
                }
            }
        }

        #endregion

        #region Event Subscription
        private void SubscribeToEvents()
        {
            eventBus.Subscribe("GameWindowClosed", ForceDisablePause);

            eventBus.Subscribe("EnemyDisplayedOff", () =>
            {
                // If the player pauses while the enemy info window is open, close the window and pause the game
                if (isEnemyInfoOpen)
                {
                    CloseEnemyInfo();
                }
            });

            eventBus.Subscribe("GamePaused", () =>
            {
                pauseEnabled = true;
            });

            eventBus.Subscribe("GameUnpaused", () =>
            {
                pauseEnabled = false;
            });
        }
        #endregion

        #region Button Listeners
        private void AssignButtonListeners()
        {
            pauseButton.onClick.AddListener(HandlePauseKey);
            playButton.onClick.AddListener(HandlePauseKey);
        }
        #endregion

        public void DisplayEnemyInfo(EnemyType enemyType)
        {    
            // The method will return true if the info panel has not been shown for the enemy type
            if (newEnemyDisplay.DisplayEnemyInfo(enemyType) == true)
            {
                // Set the bool to true so that the next time the escape key is pressed, the enemy info window will close instead of the pause menu opening
                isEnemyInfoOpen = true;

                magicPowerManager.DisableEffectButtons();
            }
        }

        public void DisplayLevelWonUI(int levelScore)
        {
            isGameOver = true;

            // Disable the pause button
            BlockPauseButton();

            magicPowerManager.DisableEffectButtons();

            mouseBlockerPanel.SetActive(true);

            fadingPanel.FadePanel(0.8f);

            eventBus.Publish("GameWindowOpened");

            postLevelInfoUI.DisplayLevelWonUI(levelScore);
        }

        public void DisplayLevelLostUI()
        {
            isGameOver = true;

            // Disable the pause button
            BlockPauseButton();

            magicPowerManager.DisableEffectButtons();

            mouseBlockerPanel.SetActive(true);

            fadingPanel.FadePanel(0.8f);

            eventBus.Publish("GameWindowOpened");

            gameLossUI.DisplayGameLossUI();
        }

        public void HideLevelLostUI()
        {
            isGameOver = false;

            // Enable the pause button
            EnablePauseButton();

            magicPowerManager.EnableEffectButtons();

            mouseBlockerPanel.SetActive(false);

            fadingPanel.UnfadePanelFromCurrent();

            eventBus.Publish("GameWindowClosed");

            gameLossUI.HideGameLossUI();
        }

        #region Helper Methods
        private void BlockPauseButton()
        {
            pauseButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);
            blockPauseButton.SetActive(true);
        }

        private void EnablePauseButton()
        {
            pauseButton.gameObject.SetActive(true);
            playButton.gameObject.SetActive(false);
            blockPauseButton.SetActive(false);
        }

        private void HandlePauseKey()
        {
            if (inTransition || isGameOver) return;

            // If the player pauses while the enemy info window is open, close the window and pause the game
            if (isEnemyInfoOpen)
            {
                CloseEnemyInfo();
            }

            // If the game isn't paused
            if (!pauseEnabled)
            {
                // Disable the effect buttons when the game is paused
                magicPowerManager.DisableEffectButtons();

                fadingPanel.FadePanel(0.6f, true);
                ForceEnablePause();
                gameSettingsWindow.OpenSettings();

                StartCoroutine(WaitTransitionTime());
            }

            // If the game is paused
            else
            {
                // Enable the effect buttons when the game is unpaused
                magicPowerManager.EnableEffectButtons();

                fadingPanel.UnfadePanelFromCurrent();
                gameSettingsWindow.CloseSettings();
                ForceDisablePause();
            }
        }

        private void CloseEnemyInfo()
        {
            magicPowerManager.EnableEffectButtons();
            newEnemyDisplay.CloseDisplayEnemyInfo();
            isEnemyInfoOpen = false;
            ForceDisablePause();
        }

        private void ForceEnablePause()
        {
            eventBus.Publish("GameWindowOpened");
            eventBus.Publish("ToggleGamePause");

            pauseButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
        }

        private void ForceDisablePause()
        {
            playButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(true);

            if (pauseEnabled)
            {
                // Enable the effect buttons when the game is unpaused
                magicPowerManager.EnableEffectButtons();

                fadingPanel.UnfadePanelFromCurrent();
                eventBus.Publish("ToggleGamePause");
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator WaitTransitionTime()
        {
            inTransition = true;

            pauseButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(false);

            blockPauseButton.SetActive(true);

            yield return new WaitForSecondsRealtime(transitionTime);

            pauseButton.gameObject.SetActive(true);
            playButton.gameObject.SetActive(false);
            blockPauseButton.SetActive(false);

            inTransition = false;
        }
        #endregion
    }
}
