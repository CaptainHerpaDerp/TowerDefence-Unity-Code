using Core;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Management
{
    /// <summary>
    /// Displays the game loss UI when the player loses the game
    /// </summary>
    public class GameLossUI : MonoBehaviour
    {
        public static GameLossUI Instance { get; private set; }

        [SerializeField] private Button RetryButton, ExitButton, AboutButton;

        [SerializeField] ExpandingScroll expandingScroll;

        [SerializeField] float targetBackgroundAlpha, backgroundFadeSpeed;

        private EventBus eventBus;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("GameLossUI instance already exists, destroying duplicate");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            eventBus = EventBus.Instance;

            RetryButton.onClick.AddListener( () => eventBus.Publish("ResetLevel"));
            ExitButton.onClick.AddListener( () => SceneManager.LoadScene("Map Menu") );
        }

        public void HideGameLossUI()
        {
            expandingScroll.DisableScroll();
        }

        public void DisplayGameLossUI()
        {
            expandingScroll.EnableScroll();
        }
    }
}