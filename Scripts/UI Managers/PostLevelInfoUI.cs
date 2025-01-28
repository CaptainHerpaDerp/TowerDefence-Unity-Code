using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Management
{
    public class PostLevelInfoUI : LevelInfoUI
    {
        public static PostLevelInfoUI Instance { get; private set; }

        [SerializeField] private ExpandingScrollVectical expandingScrollVectical;
        [SerializeField] private Button returnButton;
        [SerializeField] private string mapLoadPath;

        [SerializeField] private KeyCode exitKey = KeyCode.Escape;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Multiple PostLevelInfoUI instances detected, destroying the newest instance");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            //towerUpgradeUI = TowerUpgradeUI.Instance;
            returnButton.onClick.AddListener(ExitToMap);
        }

        public void DisplayLevelWonUI(int starCount)
        {
            expandingScrollVectical.EnableScroll();
            ShowStars(starCount);
        }

        private void ExitToMap()
        {
            // Disable the return button to prevent multiple clicks
            returnButton.interactable = false;
            
            expandingScrollVectical.FadeOutScroll();

            fadingPanelUI.FadePanelAndLoad(mapLoadPath);
        }
    }
}

