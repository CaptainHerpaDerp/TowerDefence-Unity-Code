using UnityEngine;
using UnityEngine.UI;
using UIElements;

using AudioManagement;
using Sirenix.OdinInspector;

using Saving;

namespace UIManagement {

    public class MainMenuManager : MonoBehaviour
    {
        [BoxGroup("Buttons"), SerializeField] private Button startButton;
        [BoxGroup("Buttons"), SerializeField] private Button aboutButton;
        [BoxGroup("Buttons"), SerializeField] private Button quitButton;

        [BoxGroup("Buttons"), SerializeField] private Button returnAboutButton;

        [SerializeField] private FadingPanelUI fadingPanelUI;

        [SerializeField] private float startFadeTime;

        [Header("The main menu content to be hidden when the 'about menu' is opened")]
        [SerializeField] private GameObject mainContent;

        [SerializeField] private GameObject aboutContent;
        [SerializeField] private ExpandingScrollVertical aboutScroll;

        [SerializeField] private KeyCode returnKey;
        private bool aboutMenuOpen = false;

        // Singleton
        private AudioManager audioManager;
        private FMODEvents fmodEvents;

        private void Start()
        {
            // Singleton Assignments
            audioManager = AudioManager.Instance;
            fmodEvents = FMODEvents.Instance;

            startButton.onClick.AddListener(StartGame);
            quitButton.onClick.AddListener(QuitGame);
            aboutButton.onClick.AddListener(AboutGame);

            returnAboutButton.onClick.AddListener(CloseAboutMenu);
            aboutButton.onClick.AddListener(ShowAboutMenu);

            DoWebGLPrefs();
        }

        private void Update()
        {
            if (Input.GetKeyDown(returnKey) && aboutMenuOpen)
            {
                CloseAboutMenu();
            }
        }

        private void OnEnable()
        {
            fadingPanelUI.GetComponent<Image>().color = new Color(0, 0, 0, 1);
            fadingPanelUI.UnfadePanel(startFadeTime);

        }

        public void ShowAboutMenu()
        {
            mainContent.SetActive(false);
            aboutContent.SetActive(true);
            aboutScroll.EnableScroll();

            aboutMenuOpen = true;
        }

        public void CloseAboutMenu()
        {
            audioManager.PlayOneShot(fmodEvents.scrollCloseSound, Vector2.zero);

            mainContent.SetActive(true);
            aboutScroll.DisableScroll();

            aboutMenuOpen = false;
        }

        private void StartGame()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                audioManager.PlayOneShot(fmodEvents.gameStartSound, Vector2.zero);
            }

            fadingPanelUI.FadePanelAndLoad("Map Menu");

            // disable the buttons
            startButton.interactable = false;
            quitButton.interactable = false;
            aboutButton.interactable = false;
        }

        private void QuitGame()
        {
            Application.Quit();
        }

        private void AboutGame()
        {
        }

        private void DoWebGLPrefs()
        {
            // Disable the quit button if we are in WebGL
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                quitButton.gameObject.SetActive(false);
            }
        }
    }
}