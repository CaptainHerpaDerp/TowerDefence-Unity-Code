using TMPro;
using UnityEngine;
using Core;

namespace UI.Management
{
    /// <summary>
    /// Represents the GUI showing the player's lives, gold, and wave number
    /// </summary>
    public class GuiManager : MonoBehaviour
    {
        public static GuiManager Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI livesText, goldText, waveText;
        [SerializeField] private ExpandingScrollHorizontal scrollObject;
        [SerializeField] private float scrollStartWidth, scrollTargetWidth, scrollExpandSpeed, fadeSpeed;

        private EventBus eventBus;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("More than one instance of GuiManager found in the scene");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // In the case of the player pausing before the scroll is fully expanded, we need to expand it instantly and enable the GUI elements
            eventBus = EventBus.Instance;
            eventBus.Subscribe("GamePaused", QuickExpandScroll);
        }

        public void QuickExpandScroll()
        {
            if (!scrollObject.IsScrollExpanded())
            {
                scrollObject.QuickEnableScroll();
            }
        }

        /// <summary>
        /// Diables the GUI elements
        /// </summary>
        public void HideElements()
        {
            scrollObject.DisableScroll();
        }

        /// <summary>
        /// Expands the scroll and enables the GUI elements
        /// </summary>
        public void EnableNumbersGUI()
        {
            if (scrollObject == null)
            {
                Debug.LogWarning("No scroll object assigned to GuiManager");
                return;
            }

            // Do not expand the scroll if it is already expanded
            if (scrollObject.IsScrollExpanded())
            {
                return;
            }
         
            scrollObject.EnableScroll();     
        }

        public void UpdateLivesValue(int currentLives)
        {
            livesText.text = $"{currentLives}";
        }

        public void UpdateGoldValue(int currentGold)
        {
            goldText.text = $"{currentGold}";
        }

        public void UpdateWaveValue(int currentWave, int totalWaves)
        {
            waveText.text = $"{currentWave}/{totalWaves}";
        }
    }

}
