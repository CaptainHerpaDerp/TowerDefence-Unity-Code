using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using Towers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UIManagers
{
    public class TowerPurchaseGroup : TowerButtonGroup
    {
        [BoxGroup("Component References"), SerializeField] private GridLayoutGroup elementGridLayout;

        [BoxGroup("Price Labels"), SerializeField]
        private TextMeshProUGUI
            archerPriceLabel,
            magePriceLabel,
            menAtArmsPriceLabel,
            catapultPriceLabel;

        [BoxGroup("Button References"), SerializeField]
        private TowerPurchaseOption
            archerPurchaseOption,
            magePurchaseOption,
            menAtArmsPurchaseOption,
            catapultPurchaseOption;

        private Dictionary<TowerType, TextMeshProUGUI> priceLabels;
        public Dictionary<TowerType, TowerPurchaseOption> PurchaseOptions { get; private set; }

        private const int minChildSpacing = 36;

        private void Awake()
        {
            InitializeDictionaries();
        }

        protected override void Start()
        {
            base.Start();

            if (elementGridLayout == null) { elementGridLayout = GetComponent<GridLayoutGroup>(); }

            defaultElementHeight = rectTransform.sizeDelta.y;
            defaultChildSpacing = elementGridLayout.spacing.y;

            defaultBottomPadding = elementGridLayout.padding.bottom;
            defaultTopPadding = elementGridLayout.padding.top;
        }

        private void InitializeDictionaries()
        {
            priceLabels = new()
            {
                { TowerType.Archer, archerPriceLabel },
                { TowerType.Mage, magePriceLabel },
                { TowerType.MenAtArms, menAtArmsPriceLabel },
                { TowerType.Bomber, catapultPriceLabel }
            };

            PurchaseOptions = new()
            {
                { TowerType.Archer, archerPurchaseOption },
                { TowerType.Mage, magePurchaseOption },
                { TowerType.MenAtArms, menAtArmsPurchaseOption },
                { TowerType.Bomber, catapultPurchaseOption }
            };
        }

        /// <summary>
        /// Set the purchase costs labels for each tower type
        /// </summary>
        /// <param name="purchaseCosts"></param>
        public void SetPurchaseCosts(Dictionary<TowerType, int> purchaseCosts)
        {
            foreach (var purchaseCost in purchaseCosts)
            {
                priceLabels[purchaseCost.Key].text = purchaseCost.Value.ToString();
            }
        }

        public override void KeepWindowInBoundaries()
        {
            // Reset the padding of the layout group
            elementGridLayout.padding.bottom = defaultBottomPadding;
            elementGridLayout.padding.top = defaultTopPadding;

            // Reset the spacing
            elementGridLayout.spacing = new Vector2(elementGridLayout.spacing.x, defaultChildSpacing);

            float parentY = parentRectTransform.localPosition.y;

            float canvasHeight = CanvasRefHeight();

            // If we are above the center of the canvas
            if (parentY > 0)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 1);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                rectTransform.pivot = new Vector2(0.5f, 1);

                rectTransform.localPosition = new Vector2(rectTransform.localPosition.x, defaultY + defaultElementHeight / 2);

                elementGridLayout.childAlignment = TextAnchor.UpperCenter;
            }
            else
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 0);
                rectTransform.pivot = new Vector2(0.5f, 0);

                rectTransform.localPosition = new Vector2(rectTransform.localPosition.x, defaultY - defaultElementHeight / 2);

                elementGridLayout.childAlignment = TextAnchor.LowerCenter;
            }

            float thisRectY = rectTransform.localPosition.y;

            if (parentY + thisRectY < -(canvasHeight / 2))
            {
                float difference = Mathf.Abs(parentY + thisRectY) - (canvasHeight / 2);

                Debug.Log($"Below: {difference}");

                elementGridLayout.padding.bottom += (int)difference + screenEdgePadding;

                // Reduce the spacing between the elements by the difference
                float spacing = defaultChildSpacing - difference;
                spacing = Mathf.Clamp(spacing, minChildSpacing, defaultChildSpacing);

                elementGridLayout.spacing = new Vector2(elementGridLayout.spacing.x, spacing);
            }

            if (parentY + thisRectY > canvasHeight / 2)
            {
                float difference = Mathf.Abs(parentY + thisRectY) - (canvasHeight / 2);

                Debug.Log($"Above: {difference}");

                elementGridLayout.padding.top += (int)difference + screenEdgePadding;

                // Reduce the spacing between the elements by the difference
                float spacing = defaultChildSpacing - difference;
                spacing = Mathf.Clamp(spacing, minChildSpacing, defaultChildSpacing);

                elementGridLayout.spacing = new Vector2(elementGridLayout.spacing.x, spacing);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
}
