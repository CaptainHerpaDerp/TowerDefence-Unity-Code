
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Saving;

namespace UI
{
    /// <summary>
    /// A UI element that displays information about a level in the map menu
    /// </summary>
    public class MenuLevelInfoUI : LevelInfoUI
    {
        [SerializeField] private TextMeshProUGUI levelNameTextComponent;
        [SerializeField] private ExpandingScrollHorizontal expandingScrollHorizontal;
        [SerializeField] private GameObject clickableArea;
        [SerializeField] private Sprite[] levelImageSprites;

        // "Image" field wont show in inspector?? need to find in gameobject instead...
        [SerializeField] private Image levelImagePreview; 

        public GameObject MainPanel => clickableArea;

        private SaveData saveData => SaveData.Instance;

        // Since the level info in the menu can show 0 stars, switch to 0-based indexing
        protected override void ShowStars(int starCount)
        {
            foreach (GameObject starGroup in starGroups)
            {
                starGroup.SetActive(false);
            }

            starGroups[starCount].SetActive(true);
        }

        public void DisplayUI(int levelIndex, string name)
        {
            clickableArea.SetActive(true);
            expandingScrollHorizontal.EnableScroll();
            levelNameTextComponent.text = name;

            fadingPanelUI.FadePanel(0.8f);

            int stars = saveData.GetLevelStars(levelIndex - 1);
            ShowStars(stars);

            levelImagePreview.sprite = levelImageSprites[levelIndex - 1];
        }

        public void HideUI()
        {
            clickableArea.SetActive(false);
            expandingScrollHorizontal.FadeOutScroll();
        }
    }
}
