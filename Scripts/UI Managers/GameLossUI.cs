using Core;
using UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UIManagement
{
    /// <summary>
    /// Displays the game loss UI when the player loses the game
    /// </summary>
    public class GameLossUI : Singleton<GameLossUI>
    {
        [SerializeField] private Button RetryButton, ExitButton, AboutButton;

        [SerializeField] ExpandingScroll expandingScroll;

        [SerializeField] float targetBackgroundAlpha, backgroundFadeSpeed;

        private EventBus eventBus;

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