
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameManagement
{
    /// <summary>
    /// Manages the debug menu and it's buttons
    /// </summary>
    public class DebugMenuManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject DebugMenuParent;

        [Header("Buttons")]
        [SerializeField] private Button NextWaveButton;

        [SerializeField] private Button InfiniteMoneyButton;
        private TextMeshProUGUI infiniteMoneyButtonTextComponent;

        [SerializeField] private Button InfiniteLivesButton;
        private TextMeshProUGUI infiniteLivesButtonTextComponent;

        [SerializeField] private Button InstantKillButton;
        private TextMeshProUGUI instantKillButtonTextComponent;

        [SerializeField] private Button GameWinButton, GameLossButton;

        bool infiniteMoney = false, infiniteLives = false;

        private void Start()
        {
            NextWaveButton.onClick.AddListener(() => SkipWave());

            InfiniteMoneyButton.onClick.AddListener(() => ToggleInfiniteMoney());
            infiniteMoneyButtonTextComponent = InfiniteMoneyButton.GetComponentInChildren<TextMeshProUGUI>();

            InfiniteLivesButton.onClick.AddListener(() => ToggleInfiniteLives());
            infiniteLivesButtonTextComponent = InfiniteLivesButton.GetComponentInChildren<TextMeshProUGUI>();

            InstantKillButton.onClick.AddListener(() => ToggleInstantKill());
            instantKillButtonTextComponent = InstantKillButton.GetComponentInChildren<TextMeshProUGUI>();

            GameWinButton.onClick.AddListener(() => LevelEventManager.Instance.WinLevel(1));
            GameLossButton.onClick.AddListener(() => LevelEventManager.Instance.LoseLevel());
        }

        void Update()
        {
            // Detect if the user pressed the "ALT" and "D" keys
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.D))
            {
                // If the user pressed the keys, then show the debug menu
                ToggleDebugMenu();
            }
        }

        private void ToggleDebugMenu()
        {
            DebugMenuParent.SetActive(!DebugMenuParent.activeInHierarchy);
        }

        private void SkipWave()
        {
            LevelEventManager.Instance.SkipWave();
        }

        private void ToggleInfiniteMoney()
        {
            // Get the purchase manager
            PurchaseManager.Instance.HasInfiniteMoney = !infiniteMoney;
            infiniteMoney = !infiniteMoney;

            if (infiniteMoney)
            {
                infiniteMoneyButtonTextComponent.text = "ON";
            }
            else
            {
                infiniteMoneyButtonTextComponent.text = "OFF";
            }
        }

        private void ToggleInfiniteLives()
        {
            // Get the game manager
            LevelEventManager.Instance.HasInfiniteLives = !infiniteLives;
            infiniteLives = !infiniteLives;

            if (infiniteLives)
            {
                infiniteLivesButtonTextComponent.text = "ON";
            }
            else
            {
                infiniteLivesButtonTextComponent.text = "OFF";
            }
        }

        private void ToggleInstantKill()
        {
            // Get the game manager
            LevelEventManager.Instance.HasInstantKill = !LevelEventManager.Instance.HasInstantKill;

            if (LevelEventManager.Instance.HasInstantKill)
            {
                instantKillButtonTextComponent.text = "ON";
            }
            else
            {
                instantKillButtonTextComponent.text = "OFF";
            }
        }
    }
}
