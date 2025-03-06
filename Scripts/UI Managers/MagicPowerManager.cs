using Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Enemies;
using PlayerSpells;
using System.Linq;
using System.Collections;
using UIElements;


namespace UIManagement
{
    public enum PowerType
    {
        Lightning,
        Barrier,
        Explosion,
        Spikes
    }

    public class MagicPowerManager : Singleton<MagicPowerManager>
    {
        #region Fields and Properties

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

        private List<LayerMask> roadLayers;
        private LayerMask enemyLayer;

        private HashSet<GameObject> highlightedEnemies = new HashSet<GameObject>();

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

        private void Start()
        {
            enemyLayer = GamePrefs.Instance.EnemyLayer;

            roadLayers = new List<LayerMask> { GamePrefs.Instance.RoadLayer, GamePrefs.Instance.WaterLayer };

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
           // selectedButtonHighlight.transform.position = lightningButton.transform.position;
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
                LightningStrike lightningStrike = Instantiate(effectPrefabs[activeEffect], gamePos, Quaternion.identity).GetComponent<LightningStrike>();
                lightningStrike.DoLightningStrike(highlightedEnemies.ToList());
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

        private IEnumerator HandleEnemySelection()
        {
            highlightedEnemies = new HashSet<GameObject>();

            while (true)
            {
                if (!isWaitingForInputTarget)
                {
                    ClearSelectedEnemies();
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                HighlightSelectedEnemies();
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void ClearSelectedEnemies()
        {
            if (highlightedEnemies.Count > 0)
            {
                foreach (GameObject enemy in highlightedEnemies)
                {
                    enemy.GetComponent<Enemy>().DisableSelectionRing();
                }

                highlightedEnemies.Clear();
            }
        }

        private void HighlightSelectedEnemies()
        {
            List<GameObject> selectedEnemies = GetSelectedEnemies();

            Debug.Log("Selected enemies: " + selectedEnemies.Count);

            // Go through each of the enemies in the new selection
            foreach (GameObject enemyObj in selectedEnemies)
            {
                // If the highlighted enemies list does not contain the enemy, add it
                if (!highlightedEnemies.Contains(enemyObj))
                {
                    highlightedEnemies.Add(enemyObj);
                }
            }

            // Go through each of the enemies in the highlighted enemies list
            foreach (GameObject enemy in highlightedEnemies.ToList())
            {
                // If the enemy is null, remove it from the list
                if (enemy == null)
                {
                    highlightedEnemies.Remove(enemy);
                    continue;
                }

                // If the selected enemies list does not contain the enemy, remove it from the highlighted enemies list
                if (!selectedEnemies.Contains(enemy))
                {
                    highlightedEnemies.Remove(enemy);

                    // Disable the selection ring of the enemy
                    enemy.GetComponent<Enemy>().DisableSelectionRing();
                }
            }

            // Enable the selection ring of the enemies in the highlighted enemies list
            foreach (GameObject enemy in highlightedEnemies)
            {
                enemy.GetComponent<Enemy>().EnableSelectionRing();
            }
        }

        private List<GameObject> GetSelectedEnemies()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), spellRadius, enemyLayer);

            List<GameObject> selectedEnemies = new();

            foreach (Collider2D collider in colliders)
            {
                selectedEnemies.Add(collider.gameObject);
            }

            return selectedEnemies;
        }

        private bool IsMouseOverRoad()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Go through each of the road layers and check if the ray intersects with any of them
            foreach (LayerMask roadLayer in roadLayers)
            {
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, roadLayer);
                if (hit.collider != null)
                {
                    return true;
                }
            }

            return false;
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
