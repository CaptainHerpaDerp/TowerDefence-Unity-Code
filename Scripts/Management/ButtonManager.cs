using UI.Management;
using UnityEngine;
using UnityEngine.UI;

namespace Management
{
    /// <summary>
    /// Manages the contol button UI elements in the game.
    /// </summary>
    public class ButtonManager : MonoBehaviour
    {
        [SerializeField] Button restartButton, pauseButton, playButton, quitButton, settingsButton;
        LevelEventManager levelEventManager;
        ResetConfirmation resetConfirmation;
        QuitConfirmation quitConfirmation;
        GameSettingsWindow gameSettingsWindow;

        GameWindowManager gameWindowManager;

        private void Start()
        {
            levelEventManager = LevelEventManager.Instance;
            resetConfirmation = ResetConfirmation.Instance;
            quitConfirmation = QuitConfirmation.Instance;
            gameSettingsWindow = GameSettingsWindow.Instance;

            restartButton.onClick.AddListener(resetConfirmation.EnableConfirmation);
            quitButton.onClick.AddListener(quitConfirmation.EnableConfirmation);
            settingsButton.onClick.AddListener(gameSettingsWindow.OpenSettings);
            pauseButton.onClick.AddListener(OnTogglePause);
            playButton.onClick.AddListener(OnTogglePause);
        }

        /// <summary>
        /// Called when the pause or play button is clicked.
        /// </summary>
        void OnTogglePause()
        {
            if (levelEventManager == null)
            {
                levelEventManager = LevelEventManager.Instance;
            }


            if (levelEventManager.IsPaused)
            {
                // Close the enemy info display, if it is open
                //NewEnemyDisplay.Instance.ForceCloseEnemyInfo();

                levelEventManager.PauseGame();
                playButton.gameObject.SetActive(false);
                pauseButton.gameObject.SetActive(true);
            }
            else
            {
                levelEventManager.PauseGame();
                playButton.gameObject.SetActive(true);
                pauseButton.gameObject.SetActive(false);
            }
        }
    }
}