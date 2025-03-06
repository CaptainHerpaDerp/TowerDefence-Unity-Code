using System.Runtime.InteropServices;
using UnityEngine;
using Enemies;
using Core.Debugging;

namespace Saving
{
    public class WebGLSaveData : SaveData
    {
        [DllImport("__Internal")]
        private static extern void SaveGameData(string jsonData);

        [DllImport("__Internal")]
        private static extern void LoadGameDataGL();

        private bool loadedData = false;

        private WebGLScreenLog logger;
        public override void Initialize()
        { 
            logger = WebGLScreenLog.Instance;

            LoadGameData();
        }

        public void Update()
        {

            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Saving data");
                SaveGameData();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("Loading save data");
                LoadGameData();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("Clearing save data");
                InitializeSaveData();
            }
        }


        public override void LoadGameData()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                Debug.LogError("LoadGameDataGL called on non-WebGL platform!");
                return;
            }

            LoadGameDataGL();
        }

        public override void SaveGameData()
        {
            string json = JsonUtility.ToJson(gameData);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("WebGL SaveGame received EMPTY JSON! Skipping save.");
                return;
            }

            logger.Log("Saving WebGL Data");

            SaveGameData(json);
        }

        // Called from JavaScript when data is loaded
        public void OnLoadGameData(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                logger.Log("No WebGL save data found, creating new save.");
                InitializeSaveData();
                return;
            }

            try
            {
                logger.Log("Raw JSON Data Received: " + json);

                gameData = JsonUtility.FromJson<GameData>(json);

                // Debug: Check if levelData exists after parsing
                if (gameData == null)
                {
                    Debug.LogError("ERROR: Parsed GameData is NULL!");
                    InitializeSaveData();
                    return;
                }

                if (gameData.levelData == null)
                {
                    Debug.LogError("ERROR: levelData is NULL after parsing!");
                    InitializeSaveData();
                }
                else if (gameData.levelData.Length == 0)
                {
                    Debug.LogError("ERROR: levelData is empty after parsing!");
                    InitializeSaveData();
                }
                else
                {
                    logger.Log($"Loaded {gameData.levelData.Length} levels successfully.");

                    for (int i = 0; i < 3; i++)
                    {
                        logger.Log($"LevelData Loaded - Index: {gameData.levelData[i].levelIndex} | Score: {gameData.levelData[i].levelScore}");
                    }

                    loadedData = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("ERROR: Failed to parse save data! " + e.Message);
                InitializeSaveData();
            }
        }

        public override void InitializeSaveData()
        {
            logger.Log("Initializing WebGL save data...");

            gameData.InitializeLevelData();
            gameData.InitializeEnemySeenData();

            loadedData = true;

            SaveGameData();
        }

        #region Settings Data

        public override void SaveSettingsData()
        {
            string json = JsonUtility.ToJson(settingsData);
            SaveGameData(json);
        }

        public override void LoadSettingsData()
        {
            logger.Log("Loading WebGL settings data... (Using placeholder)");

            settingsData = new();
            settingsData.resolutionX = 1280;
            settingsData.resolutionY = 720;
            settingsData.isFullscreen = false;
            settingsData.soundEffectVolume = 0.5f;
            settingsData.musicVolume = 0.5f;

            SaveSettingsData();

            Screen.SetResolution(settingsData.resolutionX, settingsData.resolutionY, settingsData.isFullscreen);
        }

        #endregion

        #region Game Progression

        public override int GetLevelStars(int levelIndex)
        {
            if (!loadedData)
            {
                Debug.LogError("WebGLSave - GameData not loaded yet!");
                return 0;
            }

            if (levelIndex < 0)
            {
                Debug.LogError($"WebGLSave - Invalid level index {levelIndex}");
                return 0;
            }

            if (levelIndex >= gameData.levelData.Length)
            {
                Debug.LogError($"WebGLSave - Level index {levelIndex} is out of range");
                return 0;
            }

            if (gameData == null)
            {
                Debug.LogError("WebGLSave -GameData is NULL");
                return 0;
            }
    
            logger.Log($"WebGLSave - Level length: {gameData.levelData.Length}");
            return gameData.levelData[levelIndex].levelScore;
        }

        public override void SetLevelStars(int levelIndex, int stars)
        {
            if (levelIndex < 0 || levelIndex >= gameData.levelData.Length)
            {
                Debug.LogError($"Invalid level index {levelIndex}");
                return;
            }

            if (stars < gameData.levelData[levelIndex].levelScore)
            {
              logger.Log($"New star count {stars} is less than current star count {gameData.levelData[levelIndex].levelScore}");
                return;
            }

            logger.Log($"Setting level {levelIndex} stars to {stars}");

            if (stars > 0 && gameData.levelData[levelIndex].levelScore == 0)
            {
              logger.Log($"Level {levelIndex} has been newly completed");
                NewlyCompletedLevelIndex = levelIndex;
            }

            gameData.levelData[levelIndex].levelScore = stars;

            logger.Log($"Level score currently at {gameData.levelData[levelIndex].levelScore}");

            SaveGameData();
        }

        public override int GetCompletedLevelCount()
        {
            int completedLevelCount = 0;
            foreach (LevelData levelData in gameData.levelData)
            {
                if (levelData.levelScore > 0)
                {
                    completedLevelCount++;
                }
            }
            return completedLevelCount;
        }

        public override bool HasSeenInfoOf(EnemyType type)
        {
            return gameData.seenEnemyData[(int)type].seen;
        }

        public override void MarkEnemyTypeInfoAsSeen(EnemyType type)
        {
            if (gameData.seenEnemyData[(int)type] == null)
            {
                Debug.LogError($"Invalid enemy type {type}");
            }

            gameData.MarkEnemyTypeInfoAsSeen(type);
            SaveGameData();
        }

        #endregion
    }
}
