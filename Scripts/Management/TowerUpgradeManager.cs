using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Towers;
using UI.Management;

namespace Management
{
    /// <summary>
    /// Displays the tower purchase and upgrade options when a tower spot is selected
    /// </summary>
    public class TowerUpgradeManager : MonoBehaviour
    {
        public static TowerUpgradeManager Instance;

        private PurchaseManager purchaseManager;
        private TowerInfoUI towerInfoUI;

        private RectTransform rectTransform;

        [Header("Tower Purchase Options")]
        [SerializeField] private GameObject towerPurchaseOptionParent;

        [SerializeField] private GameObject mageTowerOption;
        [SerializeField] private GameObject archerTowerOption;
        [SerializeField] private GameObject menAtArmsTowerOption;
        [SerializeField] private GameObject bomberTowerOption;

        [Header("Hover Underlays")]
        [SerializeField] private GameObject priceOptionUnderlay;
        [SerializeField] private GameObject regularOptionUnderlay;
        [SerializeField] private Vector2 optionUnderlayOffset;

        [Header("Tower Purchase Price Labels")]
        [SerializeField] private TMPro.TextMeshProUGUI mageTowerPriceLabel;
        [SerializeField] private TMPro.TextMeshProUGUI archerTowerPriceLabel;
        [SerializeField] private TMPro.TextMeshProUGUI menAtArmsTowerPriceLabel;
        [SerializeField] private TMPro.TextMeshProUGUI bomberTowerPriceLabel;

        public GameObject MageTowerOption => mageTowerOption;
        public GameObject ArcherTowerOption => archerTowerOption;
        public GameObject MenAtArmsTowerOption => menAtArmsTowerOption;
        public GameObject BomberTowerOption => bomberTowerOption;

        [Header("Tower Upgrade Options")]
        [SerializeField] private GameObject towerUpgradeOptionParent;

        [SerializeField] private GameObject upgradeTowerOption;
        [SerializeField] private GameObject sellTowerOption;
        [SerializeField] private GameObject militiaWaypointOption;

        [Header("Tower Upgrade Price Labels")]
        [SerializeField] private TMPro.TextMeshProUGUI upgradeTowerPriceLabel;
        [SerializeField] private TMPro.TextMeshProUGUI sellTowerPriceLabel;

        [Header("Fading Options")]
        [SerializeField] private float fadeTime;

        [Header("Tower Prefabs")]
        [SerializeField] private ArcherTower archerTower;
        [SerializeField] private MageTower mageTower;
        [SerializeField] private MilitiaTower militiaTower;
        [SerializeField] private CatapultTower catapultTower;

        public GameObject UpgradeTowerOption => upgradeTowerOption;
        public GameObject SellTowerOption => sellTowerOption;
        public GameObject MilitiaWaypointOption => militiaWaypointOption;

        private bool isFadingOut = false;
        private TowerSpot selectedTowerSpot;

        private bool isActive = false;

        [SerializeField] private Vector2 UIPurchaseOffsetPosition;

        [Header("The minimum and maximum Y values the purchase ui group can be at")]
        [SerializeField] private float purchaseUIMaxY;
        [SerializeField] private float purchaseUIMinY;

        [Header("The minimum and maximum Y values the upgrade ui group can be at")]
        [SerializeField] private float upgradeUIMaxY;
        [SerializeField] private float upgradeUIMinY;

        [SerializeField] private GridLayoutGroup purchaseLayoutGroupComponent;
        [SerializeField] private VerticalLayoutGroup upgradeLayoutGroupComponent;
        private RectTransform upgradeLayoutGroupRectTransform;
        private float upgradeGroupDefaultHeight;

        [SerializeField] private float upgradeGroupSpacingVariable, upgradeGroupMovementVariable;
        [SerializeField] private float purchaseGroupSpacingVariable;

        private float purchaseGroupSpacing;
        private float upgradeGroupSpacing;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {

                Debug.LogError("There are multiple TowerUpgradeManager instances in the scene!");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            upgradeLayoutGroupRectTransform = upgradeLayoutGroupComponent.GetComponent<RectTransform>();
            upgradeGroupDefaultHeight = upgradeLayoutGroupRectTransform.sizeDelta.y;

            purchaseManager = PurchaseManager.Instance;
            towerInfoUI = TowerInfoUI.Instance;

            priceOptionUnderlay.SetActive(false);
            regularOptionUnderlay.SetActive(false);

            // Define the default layout group spacing
            purchaseGroupSpacing = purchaseLayoutGroupComponent.spacing.y;
            upgradeGroupSpacing = upgradeLayoutGroupComponent.spacing;
        }

        private void Update()
        {
            if (!IsActive() || isFadingOut)
            {
                return;
            }

            // Deactivate the range circle every frame, if it is active
            DeactivatePreviewRangeCircle();
            CheckMouseOverOptions();
        }

        private void ActivatePreviewRangeCircle(float radius)
        {
            selectedTowerSpot.ShowTowerRangeCircleWithRadius(radius);
        }

        public void DeactivatePreviewRangeCircle()
        {
            if (selectedTowerSpot != null && towerPurchaseOptionParent.activeInHierarchy)
                selectedTowerSpot.HideRangeCircle();
        }

        private void CheckMouseOverOptions()
        {
            bool isMouseOverUIElement = false;
            // Check if the mouse is over tower purchase options
            if (IsMouseOverUIElement(mageTowerOption) || IsMouseOverUIElement(archerTowerOption) ||
                IsMouseOverUIElement(menAtArmsTowerOption) || IsMouseOverUIElement(bomberTowerOption))
            {

                isMouseOverUIElement = true;
            }

            // Check if the mouse is over tower upgrade options
            //if (IsMouseOverUIElement(upgradeTowerOption) || IsMouseOverUIElement(sellTowerOption) ||

            if (IsMouseOverUIElement(militiaWaypointOption))
            {
                // towerInfoUI.DeactivateTowerPurchaseInfo();
                isMouseOverUIElement = true;
            }

            if (IsMouseOverUIElement(upgradeTowerOption))
            {
                isMouseOverUIElement = true;
            }

            if (IsMouseOverUIElement(sellTowerOption))
            {
                //   towerInfoUI.DeactivateTowerPurchaseInfo();
                isMouseOverUIElement = true;
            }

            if (!isMouseOverUIElement)
            {
                /* If the tower upgrade option parent is active, this means the ui is suited for tower upgrade options.
                 * If no ui elements are being hovered over, we want to show the default tower info for the currently selected tower spot
                 */

                //if (towerUpgradeOptionParent.activeInHierarchy)
                //{
                //    towerInfoUI.ActivateTowerInfo(transform.position, selectedTowerSpot.LinkedTower.TowerType, selectedTowerSpot.TowerLevel);
                //}

                priceOptionUnderlay.SetActive(false);
                regularOptionUnderlay.SetActive(false);

                towerInfoUI.DeactivateTowerPurchaseInfo();
            }
        }

        private bool IsMouseOverUIElement(GameObject uiElement)
        {
            // Use the event system to check if the mouse is over a UI element
            PointerEventData pointerEventData = new(EventSystem.current);

            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            foreach (RaycastResult raycastResult in raycastResults)
            {
                // Check if the raycast result is one of the elements within the uiElement
                if (raycastResult.gameObject == uiElement || raycastResult.gameObject.transform.IsChildOf(uiElement.transform))
                {
                    // Set the position of the underlay to the position of the uiElement
                    if (uiElement == militiaWaypointOption)
                    {
                        ActivateDefaultUnderlayAtPosition(uiElement.transform.position);
                    }
                    // Otherwise set the purchase underlay to the position of the uiElement
                    else
                    {
                        ActivatePuchaseUnderlayAtPosition(uiElement.transform.position);
                    }

                    // Check if the uiElement is a tower purchase option
                    if (uiElement == archerTowerOption)
                    {
                        ActivatePreviewRangeCircle(archerTower.StartingRange * 2);
                        towerInfoUI.ActivateTowerPurchaseInfo(transform.position, TowerType.Archer);
                    }

                    if (uiElement == mageTowerOption)
                    {
                        ActivatePreviewRangeCircle(mageTower.StartingRange * 2);
                        towerInfoUI.ActivateTowerPurchaseInfo(transform.position, TowerType.Mage);
                    }

                    if (uiElement == menAtArmsTowerOption)
                    {
                        ActivatePreviewRangeCircle(militiaTower.StartingRange * 2);
                        towerInfoUI.ActivateTowerPurchaseInfo(transform.position, TowerType.MenAtArms);
                    }

                    if (uiElement == bomberTowerOption)
                    {
                        ActivatePreviewRangeCircle(catapultTower.StartingRange * 2);
                        towerInfoUI.ActivateTowerPurchaseInfo(transform.position, TowerType.Bomber);
                    }

                    if (selectedTowerSpot != null)
                    {
                        if (uiElement == upgradeTowerOption)
                        {
                            // Display the upgraded range circle preview
                            selectedTowerSpot.ShowTowerUpgradeRangeCircle();

                            if (selectedTowerSpot.LinkedTower == null)
                            {
                                Debug.LogError("Selected tower spot has no linked tower, cannot display tower upgrade info!");
                            }

                            else
                            {
                                towerInfoUI.ActivateTowerUpgradeInfo(transform.position, selectedTowerSpot.LinkedTower.TowerType, selectedTowerSpot.TowerLevel);
                            }
                        }
                    }

                    return true;
                }
            }

            /* In case the selected tower's spot is displaying an upgraded range and the player is no longer selecting a ui element,
               revert the tower's range circle back to the default range circle
             */
            if (selectedTowerSpot != null && raycastResults.Count == 0)
            {
                selectedTowerSpot.HideTowerUpgradeRangeCircle();
            }

            return false;
        }

        private void DisableHoverUnderlays()
        {
            priceOptionUnderlay.SetActive(false);
            regularOptionUnderlay.SetActive(false);
        }

        private void ActivatePuchaseUnderlayAtPosition(Vector3 position)
        {
            priceOptionUnderlay.SetActive(true);

            Vector2 actualOffset = new Vector3(optionUnderlayOffset.x * ((float)Screen.width / 1920.0f), optionUnderlayOffset.y * ((float)Screen.height / 1080.0f));

            priceOptionUnderlay.transform.position = (Vector2)position + actualOffset;
        }

        private void ActivateDefaultUnderlayAtPosition(Vector3 position)
        {
            regularOptionUnderlay.SetActive(true);
            regularOptionUnderlay.transform.position = position;
        }

        public bool IsActive()
        {
            if (towerPurchaseOptionParent.activeInHierarchy || towerUpgradeOptionParent.activeInHierarchy)
            {
                return true;
            }

            return false;
        }

        #region Activation Methods
        public void ActivateTowerPurchaseUIAtPosition(TowerSpot towerSpot)
        {
            isActive = true;
            isFadingOut = false;

            // Disable the underlays of the buttons
            DisableHoverUnderlays();

            // Disable the tower upgrade options as well as the militia waypoint option
            militiaWaypointOption.SetActive(false);
            towerUpgradeOptionParent.SetActive(false);

            StopAllCoroutines();

            transform.position = Camera.main.WorldToScreenPoint(towerSpot.GetTowerCenter());

            selectedTowerSpot = towerSpot;

            // Activate the tower purchase options and fade them in
            towerPurchaseOptionParent.SetActive(true);
            StartCoroutine(FadeInTowerPurchaseOptions());

            // Set all the price labels to the correct values
            mageTowerPriceLabel.text = purchaseManager.MageTowerPurchaseCost.ToString();
            archerTowerPriceLabel.text = purchaseManager.ArcherTowerPurchaseCost.ToString();
            menAtArmsTowerPriceLabel.text = purchaseManager.MilitiaTowerPurchaseCost.ToString();
            bomberTowerPriceLabel.text = purchaseManager.CatapultTowerPurchaseCost.ToString();

            // If the rect transform is above or below the maximum or minimum Y values, adjust the layout group spacing so that the purchase options are still visible
            Vector2 rectTransformPosition = rectTransform.localPosition;

            if (rectTransformPosition.y > purchaseUIMaxY)
            {
                float difference = rectTransformPosition.y - purchaseUIMaxY;

                purchaseLayoutGroupComponent.spacing = new Vector2(purchaseLayoutGroupComponent.spacing.x, purchaseGroupSpacing - (difference * purchaseGroupSpacingVariable));
            }
            else if (rectTransformPosition.y < purchaseUIMinY)
            {
                float difference = purchaseUIMinY - rectTransformPosition.y;

                purchaseLayoutGroupComponent.spacing = new Vector2(purchaseLayoutGroupComponent.spacing.x, purchaseGroupSpacing + (difference * -purchaseGroupSpacingVariable));
            }
            else
            {
                purchaseLayoutGroupComponent.spacing = new Vector2(purchaseLayoutGroupComponent.spacing.x, purchaseGroupSpacing);
            }

        }

        public void ActivateTowerUpgradeManagerAtPosition(TowerSpot towerSpot)
        {
            isActive = true;
            isFadingOut = false;

            // Disable the underlays of the buttons
            DisableHoverUnderlays();

            // Disable the tower upgrade options as well as the militia waypoint option
            militiaWaypointOption.SetActive(false);
            towerPurchaseOptionParent.SetActive(false);

            StopAllCoroutines();

            TowerType type = towerSpot.LinkedTower.TowerType;
            int currentTowerLevel = towerSpot.TowerLevel;
            Vector3 UIPosition = towerSpot.GetTowerCenter() + (Vector3)UIPurchaseOffsetPosition;

            int sellValue = purchaseManager.GetRefundPercentage(towerSpot.MoneySpentOnTower);

            selectedTowerSpot = towerSpot;

            towerInfoUI.ActivateTowerUpgradeInfo(transform.position, type, currentTowerLevel);

            transform.position = Camera.main.WorldToScreenPoint(UIPosition);

            // Enable all buttons relative to the upgrade manager
            towerUpgradeOptionParent.SetActive(true);
            upgradeTowerOption.SetActive(true);
            sellTowerOption.SetActive(true);

            // Set the price labels to the correct values
            upgradeTowerPriceLabel.text = purchaseManager.GetUpgradeCost(type, currentTowerLevel).ToString();
            sellTowerPriceLabel.text = sellValue.ToString();

            // Enable the rally point button if the tower is a militia tower
            if (type == TowerType.MenAtArms)
            {
                militiaWaypointOption.SetActive(true);
            }

            if (currentTowerLevel == 7)
            {
                upgradeTowerOption.SetActive(false);

                // Since only the sell option remains, we need to align it to the bottom of the layout group
                upgradeLayoutGroupComponent.childAlignment = TextAnchor.LowerCenter;

                // set the uypgrade layout's rect transform height to upgradeGroupDefaultHeight as normal
                rectTransform.sizeDelta = new Vector2(upgradeLayoutGroupComponent.GetComponent<RectTransform>().sizeDelta.x, 460);
            }

            else
            {
                // Otherwise, align the child elements to the center as usual
                upgradeLayoutGroupComponent.childAlignment = TextAnchor.MiddleCenter;

                // set the uypgrade layout's rect transform height to 460
                rectTransform.sizeDelta = new Vector2(upgradeLayoutGroupComponent.GetComponent<RectTransform>().sizeDelta.x, upgradeGroupDefaultHeight);
            }

            StartCoroutine(FadeInTowerUpgradeOptions());

            Vector2 rectTransformPosition = rectTransform.localPosition;

            //Debug.Log("Position: " + rectTransformPosition);

            // Only do this if the tower can be upgraded, otherwise the sell button will always be at the bottom
            if (rectTransformPosition.y > upgradeUIMaxY)
            {
                //Debug.Log("Position is over the max Y value!");

                float difference = rectTransformPosition.y - upgradeUIMaxY;

                //Debug.Log("Difference: " + difference);

                if (currentTowerLevel == 7)
                {
                    // Set child alignment to lower center
                    upgradeLayoutGroupComponent.childAlignment = TextAnchor.LowerCenter;

                    // Remove spacing
                    upgradeLayoutGroupComponent.spacing = 0;

                    // Set the rectransform height
                    upgradeLayoutGroupRectTransform.sizeDelta = new Vector2(upgradeLayoutGroupRectTransform.sizeDelta.x, upgradeGroupDefaultHeight - ((difference * upgradeGroupMovementVariable) / 2));

                    //Debug.Log("Tower at max level");
                }
                else
                {
                    // set the rectransform height
                    upgradeLayoutGroupRectTransform.sizeDelta = new Vector2(upgradeLayoutGroupRectTransform.sizeDelta.x, upgradeGroupDefaultHeight - (difference * upgradeGroupMovementVariable));

                    // Change the child alignment
                    upgradeLayoutGroupComponent.childAlignment = TextAnchor.UpperCenter;

                    // Set the default spacing
                    upgradeLayoutGroupComponent.spacing = upgradeGroupSpacing;
                }
            }

            else if (rectTransformPosition.y < upgradeUIMinY)
            {
                //Debug.Log("Position is under the min Y value!");

                float difference = upgradeUIMinY - rectTransformPosition.y;

                if (currentTowerLevel == 7)
                {
                    // Set child alignment to lower center
                    upgradeLayoutGroupComponent.childAlignment = TextAnchor.LowerCenter;

                    // Remove spacing
                    upgradeLayoutGroupComponent.spacing = 0;

                    // Set the rectransform height
                    upgradeLayoutGroupRectTransform.sizeDelta = new Vector2(upgradeLayoutGroupRectTransform.sizeDelta.x, upgradeGroupDefaultHeight - ((difference * upgradeGroupMovementVariable)));

                    //Debug.Log("Tower at max level");
                }
                else
                {
                    upgradeLayoutGroupComponent.childAlignment = TextAnchor.UpperLeft;

                    upgradeLayoutGroupComponent.spacing = (difference * upgradeGroupSpacingVariable);

                    upgradeLayoutGroupRectTransform.sizeDelta = new Vector2(upgradeLayoutGroupRectTransform.sizeDelta.x, upgradeGroupDefaultHeight - ((difference * upgradeGroupMovementVariable) / 2));
                }
            }

            // Otherwise, set the default spacing
            else
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.GetComponent<RectTransform>().sizeDelta.x, upgradeGroupDefaultHeight);

                // Set to the default spacing
                upgradeLayoutGroupComponent.spacing = upgradeGroupSpacing;

                if (currentTowerLevel == 7)
                {
                    // Set default child alignment
                    upgradeLayoutGroupComponent.childAlignment = TextAnchor.LowerCenter;

                    upgradeLayoutGroupRectTransform.sizeDelta = new Vector2(upgradeLayoutGroupRectTransform.sizeDelta.x, 500);
                }
                else
                {
                    // Set default child alignment
                    upgradeLayoutGroupComponent.childAlignment = TextAnchor.MiddleCenter;

                    upgradeLayoutGroupRectTransform.sizeDelta = new Vector2(upgradeLayoutGroupRectTransform.sizeDelta.x, upgradeGroupDefaultHeight);
                }
            }
        }

        public void Deactivate()
        {
            if (!isActive)
            {
                return;
            }

            isActive = false;
            isFadingOut = true;

            DisableHoverUnderlays();
            DeactivatePreviewRangeCircle();

            if (towerPurchaseOptionParent.activeInHierarchy)
            {
                StartCoroutine(FadeOutTowerPurchaseOptions());
            }

            if (towerUpgradeOptionParent.activeInHierarchy)
            {
                StartCoroutine(FadeOutTowerUpgradeOptions());
            }

            towerInfoUI.DeactivateTowerPurchaseInfo();
        }

        public void Disable()
        {
            towerPurchaseOptionParent.SetActive(false);
            towerUpgradeOptionParent.SetActive(false);

            isActive = false;
        }

        #endregion

        #region Fading Coroutines
        private IEnumerator FadeInTowerPurchaseOptions()
        {
            // Enable all price labels
            mageTowerPriceLabel.gameObject.SetActive(true);
            archerTowerPriceLabel.gameObject.SetActive(true);
            bomberTowerPriceLabel.gameObject.SetActive(true);
            menAtArmsTowerPriceLabel.gameObject.SetActive(true);

            // Create a list of images
            List<Image> images = new List<Image>();

            // Create a list of child transforms
            List<Transform> childPool = new List<Transform>()
            {
                {mageTowerOption.transform.GetChild(0)},
                {archerTowerOption.transform.GetChild(0)},
                {menAtArmsTowerOption.transform.GetChild(0)},
                {bomberTowerOption.transform.GetChild(0)}
            };

            foreach (Transform child in childPool)
            {
                // Get the image component of the child
                if (child.TryGetComponent<Image>(out Image drctImg))
                {
                    images.Add(drctImg);
                }

                // For each child, get the grandchild and great grandchild images
                foreach (Transform grandChild in child)
                {
                    if (grandChild.TryGetComponent<Image>(out Image grndDrctImg))
                    {
                        images.Add(grndDrctImg);
                    }

                    foreach (Transform greatGrandChild in grandChild)
                    {
                        if (greatGrandChild.TryGetComponent<Image>(out Image grtGrndDrctImg))
                        {
                            images.Add(grtGrndDrctImg);
                        }
                    }
                }
            }

            // Set the alpha of all images to 0
            foreach (Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            }

            float time = 0;

            while (time <= fadeTime)
            {
                float alpha = Mathf.Lerp(0, 1, time / fadeTime);

                foreach (Image image in images)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                }

                Color textColor = new(mageTowerPriceLabel.color.r, mageTowerPriceLabel.color.g, mageTowerPriceLabel.color.b, alpha);
                mageTowerPriceLabel.color = textColor;
                archerTowerPriceLabel.color = textColor;
                bomberTowerPriceLabel.color = textColor;
                menAtArmsTowerPriceLabel.color = textColor;

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            // Fully fade in the tower purchase options
            foreach (Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            }

            // Fully fade in the text labels
            Color fullOpactity = new(mageTowerPriceLabel.color.r, mageTowerPriceLabel.color.g, mageTowerPriceLabel.color.b, 1);
            mageTowerPriceLabel.color = fullOpactity;
            archerTowerPriceLabel.color = fullOpactity;
            bomberTowerPriceLabel.color = fullOpactity;
            menAtArmsTowerPriceLabel.color = fullOpactity;
        }

        private IEnumerator FadeOutTowerPurchaseOptions()
        {
            // Create a list of images
            List<Image> images = new List<Image>();

            // Create a list of child transforms
            List<Transform> childPool = new List<Transform>()
            {
                {mageTowerOption.transform.GetChild(0)},
                {archerTowerOption.transform.GetChild(0)},
                {menAtArmsTowerOption.transform.GetChild(0)},
                {bomberTowerOption.transform.GetChild(0)}
            };

            foreach (Transform child in childPool)
            {
                // Get the image component of the child
                if (child.TryGetComponent<Image>(out Image drctImg))
                {
                    images.Add(drctImg);
                }

                // For each child, get the grandchild and great grandchild images
                foreach (Transform grandChild in child)
                {
                    if (grandChild.TryGetComponent<Image>(out Image grndDrctImg))
                    {
                        images.Add(grndDrctImg);
                    }

                    foreach (Transform greatGrandChild in grandChild)
                    {
                        if (greatGrandChild.TryGetComponent<Image>(out Image grtGrndDrctImg))
                        {
                            images.Add(grtGrndDrctImg);
                        }
                    }
                }
            }

            if (images.Count == 0)
                Debug.LogError("No images found in tower purchase options!");

            float time = 0;

            while (time <= fadeTime)
            {
                float alpha = Mathf.Lerp(1, 0, time / fadeTime);

                foreach (Image image in images)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                }

                Color textColor = new(mageTowerPriceLabel.color.r, mageTowerPriceLabel.color.g, mageTowerPriceLabel.color.b, alpha);
                mageTowerPriceLabel.color = textColor;
                archerTowerPriceLabel.color = textColor;
                bomberTowerPriceLabel.color = textColor;
                menAtArmsTowerPriceLabel.color = textColor;

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            // Fully fade out the tower purchase options
            foreach (Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            }

            // Fully fade out the text labels
            Color minOpcacity = new(mageTowerPriceLabel.color.r, mageTowerPriceLabel.color.g, mageTowerPriceLabel.color.b, 0);
            mageTowerPriceLabel.color = minOpcacity;
            archerTowerPriceLabel.color = minOpcacity;
            bomberTowerPriceLabel.color = minOpcacity;
            menAtArmsTowerPriceLabel.color = minOpcacity;

            // Disable all price labels
            mageTowerPriceLabel.gameObject.SetActive(false);
            archerTowerPriceLabel.gameObject.SetActive(false);
            bomberTowerPriceLabel.gameObject.SetActive(false);
            menAtArmsTowerPriceLabel.gameObject.SetActive(false);

            towerPurchaseOptionParent.SetActive(false);
        }

        private IEnumerator FadeInTowerUpgradeOptions()
        {
            // Enable all price labels
            upgradeTowerPriceLabel.gameObject.SetActive(true);
            sellTowerPriceLabel.gameObject.SetActive(true);

            float time = 0;

            while (time <= fadeTime)
            {
                float alpha = Mathf.Lerp(0, 1, time / fadeTime);

                // Iterate through all the images within the tower purchase options and set their alpha

                if (upgradeTowerOption.activeInHierarchy)
                    foreach (Image image in upgradeTowerOption.GetComponentsInChildren<Image>())
                    {
                        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                    }

                if (sellTowerOption.activeInHierarchy)
                    foreach (Image image in sellTowerOption.GetComponentsInChildren<Image>())
                    {
                        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                    }

                if (militiaWaypointOption.activeInHierarchy)
                    foreach (Image image in militiaWaypointOption.GetComponentsInChildren<Image>())
                    {
                        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                    }


                Color textColor = new(upgradeTowerPriceLabel.color.r, upgradeTowerPriceLabel.color.g, upgradeTowerPriceLabel.color.b, alpha);
                upgradeTowerPriceLabel.color = textColor;
                sellTowerPriceLabel.color = textColor;

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            // fully fade in the tower purchase options
            if (upgradeTowerOption.activeInHierarchy)
                foreach (Image image in upgradeTowerOption.GetComponentsInChildren<Image>())
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
                }

            if (sellTowerOption.activeInHierarchy)
                foreach (Image image in sellTowerOption.GetComponentsInChildren<Image>())
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
                }

            if (militiaWaypointOption.activeInHierarchy)
                foreach (Image image in militiaWaypointOption.GetComponentsInChildren<Image>())
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
                }

            // Full fade in the text labels

            Color fullOpactity = new(upgradeTowerPriceLabel.color.r, upgradeTowerPriceLabel.color.g, upgradeTowerPriceLabel.color.b, 1);
            upgradeTowerPriceLabel.color = fullOpactity;
            sellTowerPriceLabel.color = fullOpactity;
        }

        private IEnumerator FadeOutTowerUpgradeOptions()
        {
            float time = 0;

            while (time <= fadeTime)
            {
                float alpha = Mathf.Lerp(1, 0, time / fadeTime);

                // Iterate through all the images within the tower purchase options and set their alpha
                foreach (Image image in upgradeTowerOption.GetComponentsInChildren<Image>())
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                }

                foreach (Image image in sellTowerOption.GetComponentsInChildren<Image>())
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                }

                if (militiaWaypointOption.activeInHierarchy)
                    foreach (Image image in militiaWaypointOption.GetComponentsInChildren<Image>())
                    {
                        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                    }

                Color textColor = new(upgradeTowerPriceLabel.color.r, upgradeTowerPriceLabel.color.g, upgradeTowerPriceLabel.color.b, alpha);
                upgradeTowerPriceLabel.color = textColor;
                sellTowerPriceLabel.color = textColor;

                time += Time.unscaledDeltaTime;
                yield return null;
            }


            // Disable all price labels
            upgradeTowerPriceLabel.gameObject.SetActive(false);
            sellTowerPriceLabel.gameObject.SetActive(false);

            // Fully fade out the tower purchase options
            foreach (Image image in upgradeTowerOption.GetComponentsInChildren<Image>())
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            }

            towerUpgradeOptionParent.SetActive(false);
        }

        #endregion
    }

}