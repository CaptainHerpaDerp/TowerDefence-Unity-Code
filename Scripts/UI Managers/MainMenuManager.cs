using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UI;
using System.Collections;
using Core;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button aboutButton;

    [SerializeField] private Button returnAboutButton;

    [SerializeField] private FadingPanelUI fadingPanelUI;

    [SerializeField] private float startFadeTime;

    [Header("The main menu content to be hidden when the 'about menu' is opened")]
    [SerializeField] private GameObject mainContent;

    [SerializeField] private GameObject aboutContent;
    [SerializeField] private ExpandingScrollVectical aboutScroll;

    [SerializeField] private KeyCode returnKey;
    private bool aboutMenuOpen = false;

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
        aboutButton.onClick.AddListener(AboutGame);

        returnAboutButton.onClick.AddListener(CloseAboutMenu);
        aboutButton.onClick.AddListener(ShowAboutMenu);
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
        SoundEffectManager.Instance.PlayScrollCloseSound();

        mainContent.SetActive(true);
        aboutScroll.DisableScroll();

        aboutMenuOpen = false;
    }


    private void StartGame()
    {
        SoundEffectManager.Instance.PlayGameStartSound();

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
}
