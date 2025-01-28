using Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enemies;
using PlayerSpells;
using System.Linq;
using System.Collections;

namespace UI.Management
{
    public enum PowerType
    {
        Lightning,
        Barrier,
        Explosion,
        Spikes
    }

    public class MagicPowerManager : MonoBehaviour
    {
        #region Fields and Properties

        public static MagicPowerManager Instance;

        [Header("UI Elements")]
        [SerializeField] private PlayerPowerButton lightningButton;
        [SerializeField] private GameObject selectedButtonHighlight;
        [SerializeField] private float highlightFadeTime;
        private Image selectedButtonHighlightImage;

        [Header("Effect Prefabs")]
        [SerializeField] private GameObject lightningEffectPrefab;

        private bool isWaitingForInputTarget = false;
        private PowerType activeEffect;
        private EventBus eventBus;
        private Dictionary<PowerType, GameObject> effectPrefabs;
        private PowerType currentPowerType;
        private bool gameStarted = false;
        private bool enableUsage = false;

        [SerializeField] private LayerMask roadLayer;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private KeyCode cancelEffectKey;
       // [SerializeField] private GameObject lightningCloudShadow;
       // [SerializeField] private Animator cloudShadowAnimator;
       // [SerializeField] private Vector2 cloudShadowOffset;
        [SerializeField] private float cloudTravelSpeed;
        [SerializeField] private float stopThreshold;

        public bool InfiniteClick;
        private float spellRadius;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("More than one MagicPowerManager instance in scene!");
                Destroy(this);
                return;
            }
        }

        private void Start()
        {
            InitializeEventBus();
            InitializeEffectPrefabs();
            InitializeUIElements();
            SubscribeToEvents();

            lightningButton.OnPress += () => OnEffectButtonTriggered(PowerType.Lightning);
            StartCoroutine(HandleEnemySelection());
        }

        private void Update()
        {
            HandleInput();
            if (isWaitingForInputTarget)
            {
                //MoveCloudToMousePosition();
            }
        }

        #endregion

        #region Initialization Methods

        private void InitializeEventBus()
        {
            eventBus = EventBus.Instance;
        }

        private void InitializeEffectPrefabs()
        {
            effectPrefabs = new Dictionary<PowerType, GameObject>
            {
                { PowerType.Lightning, lightningEffectPrefab }
            };
        }

        private void InitializeUIElements()
        {
            if (lightningButton == null)
            {
                Debug.LogError("Lightning Button Not Assigned!");
                return;
            }

            if (selectedButtonHighlight == null)
            {
                Debug.LogError("Selected Button Highlight Not Assigned!");
                return;
            }

            selectedButtonHighlightImage = selectedButtonHighlight.GetComponent<Image>();
        }

        private void SubscribeToEvents()
        {
            eventBus.Subscribe("MousePressAnyState", OnInputGiven);
            eventBus.Subscribe("RightMousePressAnyState", OnRightMousePress);
            eventBus.Subscribe("ResetLevel", OnLevelReset);
            eventBus.Subscribe("GameStarted", OnGameStarted);
            eventBus.Subscribe("GameWindowOpened", OnGameWindowOpened);
        }

        #endregion

        #region Event Handlers

        private void OnLevelReset()
        {
            lightningButton.DoMaxCooldownFill();
            gameStarted = false;
        }

        private void OnGameStarted()
        {
            gameStarted = true;
            EnableEffectButtons();
        }

        private void OnGameWindowOpened()
        {
            if (isWaitingForInputTarget)
            {
                //cloudShadowAnimator.SetTrigger("DisableCloud");
            }

            isWaitingForInputTarget = false;
            selectedButtonHighlight.SetActive(false);
        }

        private void OnInputGiven()
        {
            if (!IsMouseOverRoad() || (!InfiniteClick && !isWaitingForInputTarget))
            {
                return;
            }

            ApplyEffect();
        }

        private void OnRightMousePress()
        {
            eventBus.Publish("EnableMouseUsage");
            if (isWaitingForInputTarget)
            //cloudShadowAnimator.SetTrigger("DisableCloud");
            isWaitingForInputTarget = false;
            selectedButtonHighlight.SetActive(false);
        }

        #endregion

        #region Input Handling

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                OnEffectButtonTriggered(PowerType.Lightning);
            }

            if (Input.GetKeyDown(cancelEffectKey))
            {
                CancelEffect();
            }
        }

        private void CancelEffect()
        {
            eventBus.Publish("EnableMouseUsage");
            isWaitingForInputTarget = false;
            selectedButtonHighlight.SetActive(false);
        }

        #endregion

        #region Effect Handling

        private void OnEffectButtonTriggered(PowerType powerType)
        {
            if (!enableUsage || !gameStarted || lightningButton.OnCooldown)
            {
                return;
            }

            if (isWaitingForInputTarget)
            {
                CancelEffect();
                return;
            }

            PrepareEffect(powerType);
        }

        private void PrepareEffect(PowerType powerType)
        {
            selectedButtonHighlight.transform.position = lightningButton.transform.position;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //lightningCloudShadow.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
            //cloudShadowAnimator.SetTrigger("EnableCloud");

            selectedButtonHighlight.SetActive(true);
            currentPowerType = powerType;
            isWaitingForInputTarget = true;
            eventBus.Publish("DisableMouseUsage");
            activeEffect = powerType;
            spellRadius = lightningEffectPrefab.GetComponent<LightningStrike>().Radius;
        }

        private void ApplyEffect()
        {
            lightningButton.DoCooldown();
            selectedButtonHighlight.SetActive(false);
            eventBus.Publish("EnableMouseUsage");

            Vector3 gamePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gamePos.z = 0;

            if (effectPrefabs[activeEffect] != null)
            {
                Instantiate(effectPrefabs[activeEffect], gamePos, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Effect Prefab is null. Unable to instantiate effect.");
            }

            if (!InfiniteClick)
            {
                isWaitingForInputTarget = false;
                //cloudShadowAnimator.SetTrigger("DisableCloud");
            }
        }

        #endregion

        #region Utility Methods

        //private void MoveCloudToMousePosition()
        //{
        //    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    mousePosition.z = 0;

        //    float distance = Vector3.Distance(lightningCloudShadow.transform.position + (Vector3)cloudShadowOffset, mousePosition);

        //    if (distance > stopThreshold)
        //    {
        //        Vector3 newPosition = Vector3.Lerp(lightningCloudShadow.transform.position + (Vector3)cloudShadowOffset, mousePosition, cloudTravelSpeed * Time.deltaTime);
        //        lightningCloudShadow.transform.position = new Vector3(newPosition.x, newPosition.y, 0);
        //    }
        //}

        private IEnumerator HandleEnemySelection()
        {
            HashSet<GameObject> selectedEnemies = new HashSet<GameObject>();

            while (true)
            {
                if (!isWaitingForInputTarget)
                {
                    ClearSelectedEnemies(selectedEnemies);
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                HighlightSelectedEnemies(selectedEnemies);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void ClearSelectedEnemies(HashSet<GameObject> selectedEnemies)
        {
            if (selectedEnemies.Count > 0)
            {
                foreach (GameObject enemy in selectedEnemies)
                {
                    enemy.GetComponent<Enemy>().DisableSelectionRing();
                }

                selectedEnemies.Clear();
            }
        }

        private void HighlightSelectedEnemies(HashSet<GameObject> selectedEnemies)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), spellRadius, enemyLayer);

            foreach (Collider2D collider in colliders)
            {
                if (!selectedEnemies.Contains(collider.gameObject))
                {
                    selectedEnemies.Add(collider.gameObject);
                }
            }

            foreach (GameObject enemy in selectedEnemies.ToList())
            {
                if (enemy == null)
                {
                    selectedEnemies.Remove(enemy);
                    continue;
                }

                if (!colliders.Contains(enemy.GetComponent<Collider2D>()))
                {
                    selectedEnemies.Remove(enemy);
                    enemy.GetComponent<Enemy>().DisableSelectionRing();
                }
            }

            foreach (GameObject enemy in selectedEnemies)
            {
                enemy.GetComponent<Enemy>().EnableSelectionRing();
            }
        }

        private bool IsMouseOverRoad()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, roadLayer);
            return hit.collider != null;
        }

        #endregion

        #region UI Management

        public void EnableEffectButtons()
        {
            if (gameStarted)
            {
                lightningButton.HideCooldownFill();
                enableUsage = true;
            }
        }

        public void DisableEffectButtons()
        {
            isWaitingForInputTarget = false;
            selectedButtonHighlight.SetActive(false);

            lightningButton.DoMaxCooldownFill();
            enableUsage = false;
        }

        #endregion
    }
}
