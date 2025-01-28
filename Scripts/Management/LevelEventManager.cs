using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Towers;
using LevelEvents;
using UI.Management;
using UI;
using Enemies;
using Saving;
using Core;
using UnityEngine.SceneManagement;
using Analytics;

namespace Management
{
    /// <summary>
    /// A level's main manager, responsible for handling the level's waves, spawning enemies, and managing the player's lives and gold
    /// </summary>
    public class LevelEventManager : MonoBehaviour
    {
        private const string TowerTag = "Tower";
        private const string EnemyTag = "Enemy";

        #region Fields

        public static LevelEventManager Instance;

        [SerializeField] private Level levelData;

        [SerializeField] private Transform enemyParentTransform;
        [SerializeField] private Transform projectilesParent;

        [SerializeField] private List<Transform> enemySpawnPoints = new();
        [SerializeField] private List<Transform> enemyEndPoints = new();

        [SerializeField] private int lives;

        private PurchaseManager purchaseManager;
        private GuiManager guiManager;
        private SoundEffectManager soundEffectManager;

        private AnalyticsManager analyticsManager;

        [SerializeField] private float fadeInTime;

        private bool hasInfiniteLives, hasInstantKill;

        public bool HasInfiniteLives
        {
            get { return hasInfiniteLives; }

            set
            {
                hasInfiniteLives = value;

                if (value)
                {
                    guiManager.UpdateLivesValue(99);
                }
                else
                {
                    guiManager.UpdateLivesValue(lives);
                }
            }
        }

        public bool HasInstantKill
        {
            get { return hasInstantKill; }

            set
            {
                for (int i = 0; i < enemyParentTransform.childCount; i++)
                {
                    enemyParentTransform.GetChild(i).GetComponent<Enemy>().OnInstantKillChanged();
                }

                hasInstantKill = value;

                eventBus.Publish("InstantKillChanged", null);
            }
        }

        [Header("Enemy Prefabs")]
        [SerializeField] private GameObject orcPrefab;
        [SerializeField] private GameObject wolfPrefab;
        [SerializeField] private GameObject slimePrefab;
        [SerializeField] private GameObject mountedOrcPrefab;
        [SerializeField] private GameObject spikedSlimePrefab;
        [SerializeField] private GameObject beePrefab;
        [SerializeField] private GameObject queenBeePrefab;
        [SerializeField] private GameObject squidPrefab;
        [SerializeField] private GameObject anglerPrefab;
        [SerializeField] private GameObject turtlePrefab;
        [SerializeField] private GameObject gullPrefab;
        [SerializeField] private GameObject kingAnglerPrefab;
        [SerializeField] private GameObject giantSquidPrefab;
        [SerializeField] private GameObject elderTurtlePrefab;
        [SerializeField] private GameObject larvaPrefab;
        [SerializeField] private GameObject witchPrefab;
        [SerializeField] private GameObject LizardPrefab;
        [SerializeField] private GameObject bombBatPrefab;
        [SerializeField] private GameObject giantLizardPrefab;
        [SerializeField] private GameObject queenLarvaPrefab;
        [SerializeField] private GameObject treemanPrefab;

        [Header("UI Elements")]
        private FadingPanelUI fadingPanel;

        private bool isPaused = false;
        public bool IsPaused { get { return isPaused; } }

        private bool gameOver = false;
        private int waveIndex = 0;

        GameWindowManager gameWindowManager;

        public bool GameStarted { get; private set; } = false;
        private bool nextWaveButtonClicked = false;

        // Value multiplied by remaining wait time between waves to determine the amount of money the player receives for starting the next wave early
        [SerializeField] private float timeMoneyMultiplier;
        [SerializeField] private int minTimeBetweenWaves = 5;

        // private Action OnGameOver, OnGameReset, OnGamePaused, OnGameUnpaused, OnInstantKillChanged;

        private Coroutine currentWaveRoutine;

        EventBus eventBus;

        //Debug
        float gameSpeed = 1;

        Dictionary<EnemyType, GameObject> enemyPrefabs = new();

        [Header("Music")]
        [SerializeField] private bool playLevelMusic = true;

        #endregion

        #region Unity Callbacks
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Warning in " + this + ": More than one LevelEventManager instance in scene!");
                Destroy(this);
            }
        }

        private void Start()
        {
            purchaseManager = PurchaseManager.Instance;
            guiManager = GuiManager.Instance;
            eventBus = EventBus.Instance;
            gameWindowManager = GameWindowManager.Instance;
            fadingPanel = FadingPanelUI.Instance;
            soundEffectManager = SoundEffectManager.Instance;
            analyticsManager = AnalyticsManager.Instance;

            //eventBus.Publish("GameOver", null);
            //eventBus.Publish("GameReset", null);
            //eventBus.Publish("GamePaused", null);
            //eventBus.Publish("GameUnpaused", null);
            //eventBus.Publish("InstantKillChanged", null);

            eventBus.Subscribe("ToggleGamePause", PauseGame);

            eventBus.Subscribe("PauseGame", () => PauseGame(true));
            eventBus.Subscribe("UnpauseGame", () => PauseGame(false));

            eventBus.Subscribe("EnemyEndPointReached", LoseLife);
            eventBus.Subscribe("EnemyKilled", OnEnemyKilled);

            eventBus.Subscribe("ResetLevel", ResetLevel);
            eventBus.Subscribe("QuitLevel", () => SceneManager.LoadScene("Map Menu"));

            eventBus.Subscribe("NextWaveButtonPressed", NextWaveButtonPress);

            PauseGame(false);

            #region Null Checks
            if (guiManager == null)
            {
                Debug.LogWarning("Warning in " + this + ": No GUI Manager found in scene");
            }

            if (purchaseManager == null)
            {
                Debug.LogWarning("Warning in " + this + ": No Purchase Manager found in scene");
            }

            if (enemyParentTransform == null)
            {
                Debug.LogWarning("Warning in " + this + ": No enemy parent transform assigned");
            }
            #endregion

            if (levelData != null && levelData.waves.Count != 0)
            {
                lives = levelData.StartingLives;
                guiManager.UpdateLivesValue(lives);
                purchaseManager.Gold = levelData.StartingGold;
                StartWaveRoutine(true);
            }

            enemyPrefabs = new()
        {
            { EnemyType.Orc, orcPrefab },
            { EnemyType.Wolf, wolfPrefab },
            { EnemyType.Slime, slimePrefab },
            { EnemyType.MountedOrc, mountedOrcPrefab },
            { EnemyType.SpikedSlime, spikedSlimePrefab },
            { EnemyType.Bee, beePrefab },
            { EnemyType.QueenBee, queenBeePrefab },
            { EnemyType.Squid, squidPrefab },
            { EnemyType.Angler, anglerPrefab },
            { EnemyType.Turtle, turtlePrefab },
            { EnemyType.Gull, gullPrefab },
            { EnemyType.KingAngler, kingAnglerPrefab },
            { EnemyType.GiantSquid, giantSquidPrefab },
            { EnemyType.ElderTurtle, elderTurtlePrefab },
            { EnemyType.Larva, larvaPrefab },
            { EnemyType.Witch, witchPrefab },
            { EnemyType.Lizard, LizardPrefab },
            { EnemyType.BombBat, bombBatPrefab },
            { EnemyType.GiantLizard, giantLizardPrefab },
            { EnemyType.QueenLarva, queenLarvaPrefab },
            { EnemyType.Treeman, treemanPrefab }

        };

            fadingPanel.UnfadePanel(fadeInTime);

            StartCoroutine(FadeInElements());
        }

        #endregion

        #region Wave Handling

        /// <summary>
        /// Starts the level's wave routine, can optionally wait for player input to start the first wave
        /// </summary>
        /// <param name="waitForInput"></param>
        private void StartWaveRoutine(bool waitForInput = false)
        {
            if (currentWaveRoutine != null)
            {
                StopCoroutine(currentWaveRoutine);
                currentWaveRoutine = null;
            }

            guiManager.UpdateWaveValue(waveIndex + 1, levelData.waves.Count);

            currentWaveRoutine = StartCoroutine(HandleWave(levelData.waves[waveIndex], waitForInput));
        }

        private void NextWaveButtonPress()
        {
            nextWaveButtonClicked = true;
            DisableNextWaveButtons();

            soundEffectManager.PlayWaveStartSound();
        }

        public void ForceWaveStart()
        {
            nextWaveButtonClicked = true;
        }

        /// <summary>
        /// For each of the wave's events, execute the corresponding event and wait for given durations in between each. Can optionally wait for player input to start the first wave.
        /// </summary>
        /// <param name="wave"></param>
        /// <param name="waitForInput"></param>
        /// <returns></returns>
        private IEnumerator HandleWave(Wave wave, bool waitForInput = false)
        {
            yield return new WaitForFixedUpdate();

            // Wait for player input to begin the next wave
            if (waitForInput)
            {
                while (!nextWaveButtonClicked)
                {
                    yield return null;
                }

                if (playLevelMusic)
                    soundEffectManager.PlayLevelSong(levelData.LevelIndex);

                // When the game has officially started, publish the GameStarted event
                eventBus.Publish("GameStarted");
            }

            GameStarted = true;
            nextWaveButtonClicked = false;

            if (wave != null)
            {
                // Iterate through wave events
                foreach (var waveEvent in wave.waveEvents)
                {
                    // Execute the corresponding event based on the event type
                    switch (waveEvent.eventType)
                    {
                        case WaveEventType.SpawnEnemy:

                            StartCoroutine(SpawnEnemies(waveEvent.enemyType, waveEvent.spawnQuantity, waveEvent.spawnInterval, enemySpawnPoints[waveEvent.spawnLocation], enemyEndPoints[waveEvent.endLocation]));
                            break;

                        case WaveEventType.WaitDuration:
                            yield return new WaitForSeconds(waveEvent.waitDuration);
                            break;
                    }
                }

                // If there are no more waves, check if there are any enemies remaining
                if (waveIndex >= levelData.waves.Count - 1)
                {
                    // Continuously check if there are any enemies remaining, if not then the player has won           
                    while (true)
                    {
                        if (enemyParentTransform.childCount == 0)
                        {
                            WinLevel();
                            SaveData.Instance.SetLevelStars(levelData.LevelIndex, GetLevelScore());
                            yield break;
                        }

                        yield return new WaitForFixedUpdate();
                    }
                }

                // Wait for the next wave delay and activate the timer on the next wave button
                else
                {
                    // Starting next wave

                    // Wait for the minimum time between waves before allowing the player to start the next wave early
                    int totalTime = (int)levelData.waves[waveIndex].nextWaveDelay;

                    nextWaveButtonClicked = false;

                    StartCoroutine(WaitForNextWave(totalTime));

                    if (minTimeBetweenWaves < totalTime)
                    {
                        yield return new WaitForSeconds(minTimeBetweenWaves);
                        totalTime -= minTimeBetweenWaves;
                    }

                    SetNextWaveButonsTimer(totalTime);
                }
                yield break;
            }
        }

        /// <summary>
        /// Wait for the duration of the wave delay or until the player clicks the next wave button early
        /// </summary>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        private IEnumerator WaitForNextWave(int waitTime)
        {
            while (waitTime > 0)
            {
                if (gameOver)
                {
                    yield break;
                }

                if (nextWaveButtonClicked)
                {
                    // Adjust the wave index before starting the new wave
                    waveIndex++;
                    StartWaveRoutine();

                    purchaseManager.Gold += Mathf.RoundToInt(waitTime * timeMoneyMultiplier);

                    nextWaveButtonClicked = false; // Reset the flag

                    yield break;
                }

                waitTime--;

                yield return new WaitForSeconds(1);
            }

            if (gameOver)
            {
                yield break;
            }

            DisableNextWaveButtons();
            Debug.Log("Starting next wave");
            waveIndex++;
            StartWaveRoutine();
            nextWaveButtonClicked = false; // Reset the flag
        }

        private void EnableNextWaveButtons()
        {
            eventBus.Publish("NextWaveButtonEnabled");
        }

        private void SetNextWaveButonsTimer(int waitTime)
        {
            eventBus.Publish("NextWaveButtonTimerSet", waitTime);
        }

        private void DisableNextWaveButtonCountdowns()
        {
            eventBus.Publish("NextWaveButtonTimerDisabled");
        }

        private void DisableNextWaveButtons()
        {
            eventBus.Publish("NextWaveButtonDisabled");
        }

        #endregion

        #region Enemy Spawning

        /// <summary>
        /// Spawn the given quantity of enemies of the given type at the given spawn point, with the given interval between each spawn
        /// </summary>
        /// <param name="enemyType"></param>
        /// <param name="quantity"></param>
        /// <param name="spawnInterval"></param>
        /// <param name="spawnPoint"></param>
        /// <returns></returns>
        private IEnumerator SpawnEnemies(EnemyType enemyType, int quantity, float spawnInterval, Transform spawnPoint, Transform endPoint)
        {
            while (quantity > 0)
            {
                if (gameOver)
                {
                    yield break;
                }

                // Displays the enemy info if it is the first time the enemy has been seen by the player
                gameWindowManager.DisplayEnemyInfo(enemyType);

                GameObject enemyPrefab = enemyPrefabs[enemyType];

                if (enemyPrefab != null)
                {
                    float radius = spawnPoint.localScale.y / 2;
                    Vector3 position = spawnPoint.position + new Vector3(0, UnityEngine.Random.Range(-radius, radius), 0);

                    // Assign the end point to the enemy
                    Enemy enemy = enemyPrefab.GetComponent<Enemy>();

                    // We need to check if the start and end points are valid for the enemy's traversal type
                    TraverseType enemyTraversal = enemy.TraverseType;

                    // The start and end points have tags that correspond to their traversal type

                    // Check the start point
                    string startPointTag = spawnPoint.tag;
                    if (enemyTraversal == TraverseType.Road && startPointTag != "RoadStart" || enemyTraversal == TraverseType.Water && startPointTag != "WaterStart")
                    {
                        Debug.LogWarning("Error in " + this + ": Enemy traversal type does not correspond to their spawn point's type!");
                    }

                    // Check the end point
                    string endPointTag = endPoint.tag;
                    if (enemyTraversal == TraverseType.Road && endPointTag != "EndPoint" || enemyTraversal == TraverseType.Water && endPointTag != "WaterEndPoint")
                    {
                        Debug.LogWarning("Error in " + this + ": Enemy traversal type does not correspond to their end point's type!");
                    }

                    enemy.EndPoint = endPoint;

                    Enemy newEnemy = Instantiate(enemy, position, Quaternion.identity, enemyParentTransform).GetComponent<Enemy>();
                    newEnemy.SetInstantKill(hasInstantKill);
                    // ListenToEnemyEvents(enemy);
                    // enemy.OnEndpointReached += LoseLife;
                    //enemy.OnEnemyCreated += ListenToEnemyEvents;
                    // enemy.OnKilled += () => purchaseManager.Gold += enemy.carriedMoney;                 
                }

                quantity--;

                yield return new WaitForSeconds(spawnInterval);

                if (gameOver)
                {
                    yield break;
                }
            }
        }

        #endregion

        #region Life Management

        private void LoseLife()
        {
            if (hasInfiniteLives || lives <= 0 || gameOver) return;

            lives--;
            guiManager.UpdateLivesValue(lives);

            if (lives <= 0)
            {
                LoseLevel();
            }
        }

        public void WinLevel(int starCount = -1)
        {
            // Don't win the level if the player has already lost
            if (gameOver) return;

            analyticsManager.SendLevelWonEvent(levelData.LevelIndex, GetLevelScore());

            DestroyAllEnemies();

            gameOver = true;
            eventBus.Publish("GameOver", null);

            if (starCount == -1)
            {
                gameWindowManager.DisplayLevelWonUI(GetLevelScore());
                SaveData.Instance.SetLevelStars(levelData.LevelIndex, GetLevelScore());
            }
            else
            {
                gameWindowManager.DisplayLevelWonUI(starCount);
                SaveData.Instance.SetLevelStars(levelData.LevelIndex, starCount);            
            }
        }

        public void LoseLevel()
        {
            // Don't lose the level if the player has already won
            if (gameOver) return;

            analyticsManager.SendLevelLossEvent(levelData.LevelIndex, waveIndex);

            gameOver = true;

            eventBus.Publish("GameOver", null);

            gameWindowManager.DisplayLevelLostUI();
            DisableAllTowers();
        }

        /// <summary>
        /// Returns the score for the level based on the number of lives remaining
        /// </summary>
        /// <returns></returns>
        private int GetLevelScore()
        {
            int lifeDifference = levelData.StartingLives - lives;

            if (lifeDifference == 0)
            {
                return 3;
            }
            else if (lifeDifference < 5)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        #endregion

        #region Level Reset

        public void ResetLevel()
        {
            // If the player resets the level, send a level restart event
            // Only send the event if the game has started, has not already ended and the lives is less than the starting lives
            if (!gameOver && GameStarted && lives < levelData.StartingLives)
            analyticsManager.SendLevelRestartEvent(levelData.LevelIndex);

            eventBus.Publish("GameReset", null);

            StopAllCoroutines();

            waveIndex = 0;
            gameOver = false;
            lives = levelData.StartingLives;
            guiManager.UpdateLivesValue(lives);
            purchaseManager.Gold = levelData.StartingGold;
            nextWaveButtonClicked = false;
            GameStarted = false;

            DisableNextWaveButtonCountdowns();
            EnableNextWaveButtons();

            StartWaveRoutine(true);

            // Find all objects with the Tower tag and reset them
            ResetAllTowers();

            // Find all objects with the Enemy tag and destroy them
            DestroyAllEnemies();

            // Find all object with the Projectile tag and destroy them
            DestroyAllProjectiles();

            gameWindowManager.HideLevelLostUI();
        }

        public void QuitLevel()
        {
            // Unpause the game if it is paused
            PauseGame(false);          
            SceneManager.LoadScene("Map Menu");
        }

        private void ResetAllTowers()
        {
            GameObject[] towers = GameObject.FindGameObjectsWithTag(TowerTag);

            foreach (var tower in towers)
            {
                TowerSpot towerSpot = tower.GetComponent<TowerSpot>();
                if (towerSpot != null)
                    towerSpot.ResetTower();
            }
        }

        private void DisableAllTowers()
        {
            GameObject[] towers = GameObject.FindGameObjectsWithTag(TowerTag);

            foreach (var tower in towers)
            {
                TowerSpot towerSpot = tower.GetComponent<TowerSpot>();
                if (towerSpot != null && towerSpot.LinkedTower != null)
                    towerSpot.LinkedTower.DisableTowerAttacks();
            }
        }

        private void DestroyAllEnemies()
        {
            for (int i = 0; i < enemyParentTransform.childCount; i++)
            {
                Destroy(enemyParentTransform.GetChild(i).gameObject);
            }

            //GameObject[] enemies = GameObject.FindGameObjectsWithTag(EnemyTag);

            //foreach (var enemy in enemies)
            //{
            //    Destroy(enemy);
            //}
        }

        private void DestroyAllProjectiles()
        {
            for (int i = 0; i < projectilesParent.childCount; i++)
            {
                Destroy(projectilesParent.GetChild(i).gameObject);
            }
        }

        #endregion

        private void OnEnemyKilled(object character)
        {
            Enemy enemy = character as Enemy;

            if (enemy != null)
            {
                purchaseManager.Gold += enemy.carriedMoney;
            }
            else
            {
                Debug.LogError("Character is not an enemy");
            }
        }

        public Level GetLevelData()
        {
            return levelData;
        }

        /// <summary>
        /// Called when the level is loaded to fade in the dark panel and show the numbers gui
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeInElements()
        {
            yield return new WaitForSeconds(fadeInTime);

            // Show the numbers gui when the dark panel has faded out
            guiManager.EnableNumbersGUI();

            yield break;
        }

        public void PauseGame()
        {
            if (isPaused)
            {
                for (int i = 0; i < enemyParentTransform.childCount; i++)
                {
                    enemyParentTransform.GetChild(i).GetComponent<Enemy>().OnGameUnPaused();
                }

                eventBus.Publish("GameUnpaused", null);

                Time.timeScale = 1;
                isPaused = false;
            }
            else
            {
                for (int i = 0; i < enemyParentTransform.childCount; i++)
                {
                    enemyParentTransform.GetChild(i).GetComponent<Enemy>().OnGamePaused();
                }

                eventBus.Publish("GamePaused", null);

                Time.timeScale = 0;
                isPaused = true;
            }
        }

        public void PauseGame(bool pause)
        {
            if (pause)
            {
                for (int i = 0; i < enemyParentTransform.childCount; i++)
                {
                    enemyParentTransform.GetChild(i).GetComponent<Enemy>().OnGamePaused();
                }

                eventBus.Publish("GamePaused", null);

                Time.timeScale = 0;
                isPaused = true;
            }
            else
            {
                for (int i = 0; i < enemyParentTransform.childCount; i++)
                {
                    enemyParentTransform.GetChild(i).GetComponent<Enemy>().OnGameUnPaused();
                }

                eventBus.Publish("GameUnpaused", null);

                Time.timeScale = 1;
                isPaused = false;
            }
        }

        /// <summary>
        /// Destroy all current enemies and skip to the next wave
        /// </summary>
        public void SkipWave()
        {
            if (currentWaveRoutine != null)
            {
                StopCoroutine(currentWaveRoutine);
                currentWaveRoutine = null;
            }

            StopAllCoroutines();

            DestroyAllEnemies();

            waveIndex++;
            currentWaveRoutine = StartCoroutine(HandleWave(levelData.waves[waveIndex], false));
            guiManager.UpdateWaveValue(waveIndex + 1, levelData.waves.Count);
        }

        // Nullify the instance when the object is destroyed
        private void OnDestroy()
        {
            Instance = null;
        }

#if UNITY_EDITOR
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (gameSpeed > 0)
                {
                    gameSpeed -= 0.5f;
                    Time.timeScale = gameSpeed;
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (gameSpeed < 100)
                {
                    gameSpeed += 0.5f;
                    Time.timeScale = gameSpeed;
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                gameSpeed = 1;
                Time.timeScale = gameSpeed;
            }
        }

        public Enemy GetEnemyAtIndex(int index)
        {
            return enemyParentTransform.GetChild(index).GetComponent<Enemy>();
        }
#endif
    }
}