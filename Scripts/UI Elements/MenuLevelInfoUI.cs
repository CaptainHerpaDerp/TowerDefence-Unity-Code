
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Saving;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace UIElements
{
    /// <summary>
    /// A UI element that displays information about a level in the map menu
    /// </summary>
    public class MenuLevelInfoUI : LevelInfoUI
    {
        [BoxGroup("Component References"), SerializeField] private TextMeshProUGUI levelNameTextComponent;
        [BoxGroup("Component References"), SerializeField] private ExpandingScrollHorizontal expandingScrollHorizontal;

        [SerializeField] private Sprite[] levelImageSprites;
        [SerializeField] private Image levelImagePreview; 
        private SaveManager saveManager => SaveManager.Instance;

        protected virtual void Start()
        {
            // Hide all elements at the start
            expandingScrollHorizontal.QuickDisableScroll();
        }

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
            expandingScrollHorizontal.EnableScroll();
            levelNameTextComponent.text = name;

            fadingPanelUI.FadePanel(0.8f);

            int stars = saveManager.GetLevelStars(levelIndex - 1);

            ShowStars(stars);

            levelImagePreview.sprite = levelImageSprites[levelIndex - 1];
        }

        public void HideUI()
        {
            expandingScrollHorizontal.DisableScroll();
        }
    }
}
