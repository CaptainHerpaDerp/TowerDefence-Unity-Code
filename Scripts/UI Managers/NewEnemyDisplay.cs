using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Enemies;
using UI;
using Saving;
using Core;

namespace UI.Management
{
    /*
     * Note: This is a bit of a messy situation. The display's size varies with key points, therefore the expanding scroll component cannot be called until all the key points have been added.
     * Furthermore, there are some issues with the content size fitter where it only works when you disable and re-enable it. This is why the display is disabled and re-enabled in the coroutine.
     * When a new enemy enters, the final window needs to be displayed with all the elements enabled. Either all the element's alpha is set to 0, then the scroll expanded or we finalize the display at another 
     * Y position and then move it to the final position with the scroll expansion.
    */

    /// <summary>
    /// A UI element that displays information about a new enemy type when it is first encountered.
    /// </summary>
    public class NewEnemyDisplay : MonoBehaviour
    {
        public static NewEnemyDisplay Instance;

        [Header("Enemy Images")]
        [SerializeField] private Sprite orcSprite;
        [SerializeField]
        private Sprite wolfSprite, mountedOrcSprite,
            slimeSprite, spikedSlimeSprite, beeSprite, queenBeeSprite,
            squidSprite, giantSquidSprite, gullSprite, turtleSprite, elderTurtleSprite, anglerSprite, kingAnglerSprite;

        [SerializeField] private Image unitImage;

        [Header("UI Elements")]
        [SerializeField] private Transform elementsGroup;
        [SerializeField] private GameObject keyPointsPrefab;
        [SerializeField] private Transform keyPointsParent;
        [SerializeField] private Button closeButton;
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private GameObject mouseBlockerPanel;

        SaveData saveData;
        EventBus eventBus;

        [SerializeField] private KeyCode closeKey = KeyCode.Escape;

        // In some cases, the player can press escape as the scroll is being loaded. this causes problems.
        private bool ableToClose = true;

        private bool isOpen = false;

        [SerializeField] private float defaultYPos;

        [SerializeField] private NewEnemyExpandingScrollVertical scrollComponent;

        #region Unity Functions

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("New Enemy Display Instance already exists");
                Destroy(gameObject);
            }
        }

        void Start()
        {
            eventBus = EventBus.Instance;
            saveData = SaveData.Instance;

            if (saveData == null)
            {
                Debug.LogError("Save Data is null");
            }

            closeButton.onClick.AddListener(CloseDisplayEnemyInfo);
        }

        private void Update()
        {
            // if the window is open and the player presses the close key, close the window

            if (isOpen && ableToClose && Input.GetKeyDown(closeKey))
            {               
                CloseDisplayEnemyInfo();
            }
        }

        #endregion

        public bool DisplayEnemyInfo(EnemyType type)
        {
            // Do not display the enemy info if it has already been seen
            if (saveData.HasSeenInfoOf(type))
            {
                return false;
            }

            Debug.Log("Displaying Enemy Info");

            saveData.MarkEnemyTypeInfoAsSeen(type);

            isOpen = true;
            StartCoroutine(DisplayEnemyInfoCR(type));

            mouseBlockerPanel.SetActive(true);

            return true;
        }

        public void CloseDisplayEnemyInfo()
        {
            if (!isOpen)
            {
                return;
            }

            isOpen = false;

            eventBus.Publish("ToggleGamePause");
            eventBus.Publish("EnableMouseUsage");
            eventBus.Publish("EnemyDisplayedOff");

            scrollComponent.FadeOutScroll();

            mouseBlockerPanel.SetActive(false);
        }

        public void ForceCloseEnemyInfo()
        {
            isOpen = false;
            scrollComponent.DisableScroll();

            mouseBlockerPanel.SetActive(false);
        }

        /// <summary>
        /// Set the alpha of all elements to 0
        /// </summary>
        private void HideAllElements()
        {
            Image elementsGroupImage = elementsGroup.GetComponent<Image>();
            if (elementsGroupImage != null)
            {
                elementsGroupImage.color = new Color(elementsGroupImage.color.r, elementsGroupImage.color.g, elementsGroupImage.color.b, 0);
            }

            foreach (Transform child in elementsGroup)
            {
                // Try to get the image component of the child
                if (child.TryGetComponent<Image>(out Image drctImg))
                {
                    drctImg.color = new Color(drctImg.color.r, drctImg.color.g, drctImg.color.b, 0);
                }

                // Try to get the text component of the child
                if (child.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI drctText))
                {
                    drctText.color = new Color(drctText.color.r, drctText.color.g, drctText.color.b, 0);
                }

                // Get all images in the child
                foreach (Image image in child.GetComponentsInChildren<Image>())
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
                }

                // Get all text components in the child
                foreach (TextMeshProUGUI text in child.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
                }              
            }
        }

        private IEnumerator DisplayEnemyInfoCR(EnemyType type)
        {
            ableToClose = false;

            yield return new WaitForSecondsRealtime(1);

            // If the window is closed before the coroutine finishes, stop the coroutine
            if (!isOpen)
                yield break;

            elementsGroup.gameObject.SetActive(true);
            HideAllElements();

            /// elementsGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(elementsGroup.GetComponent<RectTransform>().sizeDelta.x, baseElementsGroupHeight);

            // scroll.scrollTargetHeight = baseScrollHeight;

            elementsGroup.GetComponent<ContentSizeFitter>().enabled = false;

            // Custom scroll height for certain enemies
            switch (type)
            {
                case EnemyType.QueenBee:
                case EnemyType.MountedOrc:

                    // Set the height of the elements group to the base height
                   // elementsGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(elementsGroup.GetComponent<RectTransform>().sizeDelta.x, 410f);

                    //scroll.scrollTargetHeight = 240f;
                    break;

            }

            if (keyPointsPrefab == null)
            {
                Debug.LogError("Key Points Prefab is null");
                yield break;
            }

            foreach (Transform child in keyPointsParent)
            {
                if (child.gameObject != keyPointsPrefab)
                    Destroy(child.gameObject);
            }

            switch (type)
            {
                case EnemyType.Orc:
                    Debug.Log("Displaying Orc Info");
                    unitImage.sprite = orcSprite;
                    unitNameText.text = "Orc";
                    GameObject orcKeyPoint1 = Instantiate(keyPointsPrefab, keyPointsParent);
                    orcKeyPoint1.GetComponentInChildren<TextMeshProUGUI>().text = "Low Speed";
                    GameObject orcKeyPoint3 = Instantiate(keyPointsPrefab, keyPointsParent);
                    orcKeyPoint3.GetComponentInChildren<TextMeshProUGUI>().text = "Low Damage";
                    GameObject orcKeyPoint2 = Instantiate(keyPointsPrefab, keyPointsParent);
                    orcKeyPoint2.GetComponentInChildren<TextMeshProUGUI>().text = "High Health";
                    break;

                case EnemyType.Wolf:
                    unitImage.sprite = wolfSprite;
                    unitNameText.text = "Wolf";
                    GameObject wolfKeyPoint1 = Instantiate(keyPointsPrefab, keyPointsParent);
                    wolfKeyPoint1.GetComponentInChildren<TextMeshProUGUI>().text = "High Speed";
                    GameObject wolfKeyPoint2 = Instantiate(keyPointsPrefab, keyPointsParent);
                    wolfKeyPoint2.GetComponentInChildren<TextMeshProUGUI>().text = "Low Health";
                    break;

                case EnemyType.Slime:
                    unitImage.sprite = slimeSprite;
                    unitNameText.text = "Slime";
                    GameObject slimeKeyPoint1 = Instantiate(keyPointsPrefab, keyPointsParent);
                    slimeKeyPoint1.GetComponentInChildren<TextMeshProUGUI>().text = "High Speed";
                    GameObject slimeKeyPoint3 = Instantiate(keyPointsPrefab, keyPointsParent);
                    slimeKeyPoint3.GetComponentInChildren<TextMeshProUGUI>().text = "Low Damage";
                    GameObject slimeKeyPoint4 = Instantiate(keyPointsPrefab, keyPointsParent);
                    slimeKeyPoint4.GetComponentInChildren<TextMeshProUGUI>().text = "Can Split Once";
                    break;

                case EnemyType.SpikedSlime:
                    unitImage.sprite = spikedSlimeSprite;
                    unitNameText.text = "Spiked Slime";
                    GameObject spikedSlimeKeyPoint1 = Instantiate(keyPointsPrefab, keyPointsParent);
                    spikedSlimeKeyPoint1.GetComponentInChildren<TextMeshProUGUI>().text = "Deals AOE Damage";
                    GameObject spikedSlimeKeyPoint2 = Instantiate(keyPointsPrefab, keyPointsParent);
                    spikedSlimeKeyPoint2.GetComponentInChildren<TextMeshProUGUI>().text = "High Health";
                    GameObject spikedSlimeKeyPoint3 = Instantiate(keyPointsPrefab, keyPointsParent);
                    spikedSlimeKeyPoint3.GetComponentInChildren<TextMeshProUGUI>().text = "Converts Nearby Slime when Killed";
                    break;

                case EnemyType.MountedOrc:
                    unitImage.sprite = mountedOrcSprite;
                    unitNameText.text = "Mounted Orc";

                    GameObject mountedOrcKeyPoint1 = Instantiate(keyPointsPrefab, keyPointsParent);
                    mountedOrcKeyPoint1.GetComponentInChildren<TextMeshProUGUI>().text = "High Damage";
                    GameObject mountedOrcKeyPoint2 = Instantiate(keyPointsPrefab, keyPointsParent);
                    mountedOrcKeyPoint2.GetComponentInChildren<TextMeshProUGUI>().text = "High Charge Speed";
                    GameObject mountedOrcKeyPoint3 = Instantiate(keyPointsPrefab, keyPointsParent);
                    mountedOrcKeyPoint3.GetComponentInChildren<TextMeshProUGUI>().text = "High Charge Damage to Struck Enemy";
                    break;

                case EnemyType.Bee:
                    unitImage.sprite = beeSprite;
                    unitNameText.text = "Bee";

                    GameObject beeKeyPoint1 = Instantiate(keyPointsPrefab, keyPointsParent);
                    beeKeyPoint1.GetComponentInChildren<TextMeshProUGUI>().text = "High Speed";
                    GameObject beeKeyPoint2 = Instantiate(keyPointsPrefab, keyPointsParent);
                    beeKeyPoint2.GetComponentInChildren<TextMeshProUGUI>().text = "Low Health";
                    GameObject beeKeyPoint3 = Instantiate(keyPointsPrefab, keyPointsParent);
                    beeKeyPoint3.GetComponentInChildren<TextMeshProUGUI>().text = "Does Not Engage In Combat";
                    break;

                case EnemyType.QueenBee:
                    unitImage.sprite = queenBeeSprite;
                    unitNameText.text = "Queen Bee";

                    GameObject queenBeeKeyPoint1 = Instantiate(keyPointsPrefab, keyPointsParent);
                    queenBeeKeyPoint1.GetComponentInChildren<TextMeshProUGUI>().text = "High Health";
                    GameObject queenBeeKeyPoint2 = Instantiate(keyPointsPrefab, keyPointsParent);
                    queenBeeKeyPoint2.GetComponentInChildren<TextMeshProUGUI>().text = "High Damage";
                    GameObject queenBeeKeyPoint3 = Instantiate(keyPointsPrefab, keyPointsParent);
                    queenBeeKeyPoint3.GetComponentInChildren<TextMeshProUGUI>().text = "Places Hives, which Spawn Bees Over Time";
                    break;

                case EnemyType.Squid:
                    unitImage.sprite = squidSprite;
                    unitNameText.text = "Squid";

                    GameObject squidKeyPoint1 = Instantiate(keyPointsPrefab, keyPointsParent);
                    squidKeyPoint1.GetComponentInChildren<TextMeshProUGUI>().text = "Does Not Engage In Combat";
                    break;

                case EnemyType.Angler:
                    unitImage.sprite = anglerSprite;
                    unitNameText.text = "Angler";

                    GameObject anglerKeyPoint1 = Instantiate(keyPointsPrefab, keyPointsParent);
                    anglerKeyPoint1.GetComponentInChildren<TextMeshProUGUI>().text = "High Health";
                    GameObject anglerKeyPoint2 = Instantiate(keyPointsPrefab, keyPointsParent);
                    anglerKeyPoint2.GetComponentInChildren<TextMeshProUGUI>().text = "High Damage";
                    GameObject anglerKeyPoint3 = Instantiate(keyPointsPrefab, keyPointsParent);
                    anglerKeyPoint3.GetComponentInChildren<TextMeshProUGUI>().text = "Can Emerge From Water";
                    break;

                case EnemyType.Turtle:
                    unitImage.sprite = turtleSprite;
                    unitNameText.text = "Turtle";

                    GameObject turtleKeyPoint1 = Instantiate(keyPointsPrefab, keyPointsParent);
                    turtleKeyPoint1.GetComponentInChildren<TextMeshProUGUI>().text = "High Health";
                    GameObject turtleKeyPoint3 = Instantiate(keyPointsPrefab, keyPointsParent);
                    turtleKeyPoint3.GetComponentInChildren<TextMeshProUGUI>().text = "Low Damage";
                    GameObject turtleKeyPoint2 = Instantiate(keyPointsPrefab, keyPointsParent);
                    turtleKeyPoint2.GetComponentInChildren<TextMeshProUGUI>().text = "Very Low Speed";
                    break;

                case EnemyType.Gull:
                    unitImage.sprite = gullSprite;
                    unitNameText.text = "Gull";

                    GameObject gullKeyPoint1 = Instantiate(keyPointsPrefab, keyPointsParent);
                    gullKeyPoint1.GetComponentInChildren<TextMeshProUGUI>().text = "Very Low Health";
                    GameObject gullKeyPoint2 = Instantiate(keyPointsPrefab, keyPointsParent);
                    gullKeyPoint2.GetComponentInChildren<TextMeshProUGUI>().text = "Extremely High Speed";
                    GameObject gullKeyPoint3 = Instantiate(keyPointsPrefab, keyPointsParent);
                    gullKeyPoint3.GetComponentInChildren<TextMeshProUGUI>().text = "Does Not Engage In Combat";
                    break;

                case EnemyType.ElderTurtle:
                    unitImage.sprite = elderTurtleSprite;
                    unitNameText.text = "Elder Turtle";

                    GameObject elderTurtleKeyPoint1 = Instantiate(keyPointsPrefab, keyPointsParent);
                    elderTurtleKeyPoint1.GetComponentInChildren<TextMeshProUGUI>().text = "Extremely High Health";
                    GameObject elderTurtleKeyPoint3 = Instantiate(keyPointsPrefab, keyPointsParent);
                    elderTurtleKeyPoint3.GetComponentInChildren<TextMeshProUGUI>().text = "Very Low Speed";
                    GameObject elderTurtleKeyPoint4 = Instantiate(keyPointsPrefab, keyPointsParent);
                    elderTurtleKeyPoint4.GetComponentInChildren<TextMeshProUGUI>().text = "Can Become Invulnerable";
                    break;

                case EnemyType.GiantSquid:
                    unitImage.sprite = giantSquidSprite;
                    unitNameText.text = "Giant Squid";

                    GameObject giantSquidKeyPoint1 = Instantiate(keyPointsPrefab, keyPointsParent);
                    giantSquidKeyPoint1.GetComponentInChildren<TextMeshProUGUI>().text = "High Health";
                    GameObject giantSquidKeyPoint2 = Instantiate(keyPointsPrefab, keyPointsParent);
                    giantSquidKeyPoint2.GetComponentInChildren<TextMeshProUGUI>().text = "Can Preform Long Dash";
                    break;
            }

            // Wait for the elements to be set up
            yield return new WaitForSecondsRealtime(0.1f);

            elementsGroup.GetComponent<ContentSizeFitter>().enabled = true;

            yield return new WaitForSecondsRealtime(0.1f);

            if (!isOpen)
                yield break;

            scrollComponent.scrollTargetHeight = elementsGroup.GetComponent<RectTransform>().sizeDelta.y;

            elementsGroup.GetComponent<ContentSizeFitter>().enabled = false;

            scrollComponent.EnableScroll();

            ableToClose = true;

            // Pause the game
            eventBus.Publish("ToggleGamePause");

            //eventBus.Publish("EnemyDisplayedOn");
            //eventBus.Publish("ToggleGamePause");
            //eventBus.Publish("DisableMouseUsage");
            yield break;
        }
    }
}