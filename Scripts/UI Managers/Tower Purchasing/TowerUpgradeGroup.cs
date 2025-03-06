using Sirenix.OdinInspector;
using TMPro;
using Towers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class TowerUpgradeGroup : TowerButtonGroup
{
    [BoxGroup("Component References"), SerializeField] private VerticalLayoutGroup elementVerticalLayout;

    [InfoBox("The upgrade button needs to be faded rather than disabled as to preserve the layout group's spacing")]
    [BoxGroup("Button References"), SerializeField] private CanvasGroup towerUpgradeButtonCanvasGroup;

    [field: SerializeField, BoxGroup("Button References")] public TowerPurchaseOption UpgradeButton { get; private set; }
    [field: SerializeField, BoxGroup("Button References")] public TowerPurchaseOption SellButton { get; private set; }
    [field: SerializeField, BoxGroup("Button References")] public TowerPurchaseOption MilitiaWaypointButton { get; private set; }

    [BoxGroup("Price Labels"), SerializeField]
    private TextMeshProUGUI
    upgradePriceLabel,
    sellPriceLabel;

    private const int minChildSpacing = 0;

    protected override void Start()
    {
        base.Start();

        if (elementVerticalLayout == null) { elementVerticalLayout = GetComponent<VerticalLayoutGroup>(); }

        defaultElementHeight = rectTransform.sizeDelta.y;
        defaultChildSpacing = elementVerticalLayout.spacing;

        defaultBottomPadding = elementVerticalLayout.padding.bottom;
        defaultTopPadding = elementVerticalLayout.padding.top;
    }

    /// <summary>
    /// Set the price labels for upgrading and selling a tower
    /// </summary>
    /// <param name="purchaseCosts"></param>
    public void SetPurchaseCosts(int upgradeCost, int sellCost)
    {
        upgradePriceLabel.text = upgradeCost.ToString();
        sellPriceLabel.text = sellCost.ToString();
    }

    /// <summary>
    /// Modify the tower info displayed in the upgrade group
    /// </summary>
    /// <param name="towerType"></param>
    /// <param name="towerLevel"></param>
    public void SetTowerInfo(TowerType towerType, int towerLevel)
    {
        // Enable the rally point button if the tower is a militia tower
        if (towerType == TowerType.MenAtArms)
        {
            MilitiaWaypointButton.gameObject.SetActive(true);
        }
        else
        {
            MilitiaWaypointButton.gameObject.SetActive(false);
        }

        // If the tower is at max level, disable the upgrade option
        if (towerLevel == 7)
        {
            Utils.DisableCanvasGroup(towerUpgradeButtonCanvasGroup);
        }
        else
        {
            Utils.EnableCanvasGroup(towerUpgradeButtonCanvasGroup);
        }
    }

    public override void KeepWindowInBoundaries()
    {
        Debug.Log("Boundary Check");

        // Reset the padding of the layout group
        elementVerticalLayout.padding.bottom = defaultBottomPadding;
        elementVerticalLayout.padding.top = defaultTopPadding;

        // Reset the spacing
        elementVerticalLayout.spacing = defaultChildSpacing;

        float parentY = parentRectTransform.localPosition.y;

        float canvasHeight = CanvasRefHeight();

        // If we are above the center of the canvas
        if (parentY > 0)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);

            rectTransform.localPosition = new Vector2(rectTransform.localPosition.x, defaultY + defaultElementHeight / 2);

            elementVerticalLayout.childAlignment = TextAnchor.UpperCenter;
        }
        else
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0);
            rectTransform.anchorMax = new Vector2(0.5f, 0);
            rectTransform.pivot = new Vector2(0.5f, 0);

            rectTransform.localPosition = new Vector2(rectTransform.localPosition.x, defaultY - defaultElementHeight / 2);

            elementVerticalLayout.childAlignment = TextAnchor.LowerCenter;
        }

        float thisRectY = rectTransform.localPosition.y;

        if (parentY + thisRectY < -(canvasHeight / 2))
        {
            float difference = Mathf.Abs(parentY + thisRectY) - (canvasHeight / 2);

            Debug.Log($"Below: {difference}");

            elementVerticalLayout.padding.bottom += (int)difference + screenEdgePadding;

            // The top needs padding in the opposite direction
            elementVerticalLayout.padding.top -= ((int)difference + screenEdgePadding);

            // Reduce the spacing between the elements by the difference
            float spacing = defaultChildSpacing - ((difference * 2) + screenEdgePadding);

            Debug.Log($"Spacing: {spacing}");

            spacing = Mathf.Clamp(spacing, minChildSpacing, defaultChildSpacing);

            elementVerticalLayout.spacing = spacing;
        }

        if (parentY + thisRectY > canvasHeight / 2)
        {
            float difference = Mathf.Abs(parentY + thisRectY) - (canvasHeight / 2);

            Debug.Log($"Above: {difference}");

            elementVerticalLayout.padding.top += (int)difference + screenEdgePadding;

            // Reduce the spacing between the elements by the difference
            float spacing = defaultChildSpacing - (difference + screenEdgePadding);

            Debug.Log($"Spacing: {spacing}");

            spacing = Mathf.Clamp(spacing, minChildSpacing, defaultChildSpacing);

            elementVerticalLayout.spacing = spacing;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    #region Utility Methods

    /// <summary>
    /// Returns true if any of the buttons in the group are selected
    /// </summary>
    /// <returns></returns>
    public bool IsAnyButtonSelected()
    {
        if (UpgradeButton.IsSelected || SellButton.IsSelected || MilitiaWaypointButton.IsSelected)
        {
            return true;
        }

        return false;
    }

    #endregion
}
