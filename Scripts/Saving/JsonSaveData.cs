using UnityEngine;
using System.IO;
using Enemies;
using AudioManagement;
using Core.Debugging;

namespace Saving
{
    public class JsonSaveData : SaveData
    {
        //protected override void Awake()
        //{
        //    base.Awake();

        //    LoadGameData();
        //    LoadSettingsData();
        //}

        public override void Initialize()
        {
            AudioManager.Instance.SetMasterVolume(settingsData.soundEffectVolume);
        }

#if UNITY_EDITOR
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
#endif

        public override void SaveGameData()
        {
            string json = JsonUtility.ToJson(gameData);
            string filePath = Application.persistentDataPath + "/gameData.json";
            File.WriteAllText(filePath, json);

            Debug.Log("Saving: " + json);
        }

        public override void LoadGameData()
        {
            string filePath = Application.persistentDataPath + "/gameData.json";

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                gameData = JsonUtility.FromJson<GameData>(json);
            }
            else
            {
                Debug.Log("No save data found, creating new save data");
                InitializeSaveData();
            }
        }

        public override void InitializeSaveData()
        {
            gameData = new GameData();

            gameData.InitializeLevelData(); 
            gameData.InitializeEnemySeenData();

            SaveGameData();
        }

        #region Settings Data

        public override void SaveSettingsData()
        {
            string json = JsonUtility.ToJson(settingsData);
            string filePath = Application.persistentDataPath + "/settingsData.json";

            File.WriteAllText(filePath, json);
        }

        public override void LoadSettingsData()
        {
            string filePath = Application.persistentDataPath + "/settingsData.json";

            NativeX = Screen.currentResolution.width;
            NativeY = Screen.currentResolution.height;

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                settingsData = JsonUtility.FromJson<SettingsData>(json);
            }
            else
            {
                Debug.Log("No settings data found, creating new settings data");

                settingsData = new();
                settingsData.resolutionX = 1920;
                settingsData.resolutionY = 1080;
                settingsData.isFullscreen = false;
                settingsData.soundEffectVolume = 0.5f;
                settingsData.musicVolume = 0.5f;

                SaveSettingsData();
            }

            if (settingsData.resolutionX < 1920 || settingsData.resolutionY < 1080)
            {
                settingsData.resolutionX = 1920;
                settingsData.resolutionY = 1080;
            }

            Screen.SetResolution(settingsData.resolutionX, settingsData.resolutionY, settingsData.isFullscreen);

            // For whatever hell of a reason, i need to fiddle with the rotation of the main camera when the resolution is at 1080p
            if (settingsData.resolutionX == 1920 && settingsData.resolutionY == 1080)
            {
                Camera.main.transform.rotation = Quaternion.Euler(0.1f, 0.01f, 0);
            }
        }

        #endregion



        public override int GetLevelStars(int levelIndex)
        {
            return gameData.levelData[levelIndex].levelScore;           
        }

        public override void SetLevelStars(int levelIndex, int stars)
        {      
            // check if the given level index is valid
            if (levelIndex < 0 || levelIndex >= gameData.levelData.Length)
            {
                Debug.LogError($"Invalid level index {levelIndex}");
                return;
            }

            // If the new star count is less than the current star count, don't update the star count.
            if (stars < gameData.levelData[levelIndex].levelScore)
            {
                Debug.Log($"New star count {stars} is less than current star count {gameData.levelData[levelIndex].levelScore}");
                return;
            }

            Debug.Log($"Setting level {levelIndex} stars to {stars}");

            // If the star count is greater than 0, the level has been completed, thus we should set the newly completed level index.
            if (stars > 0 && gameData.levelData[levelIndex].levelScore == 0)
            {
                Debug.Log($"Level {levelIndex} has been newly completed");
                NewlyCompletedLevelIndex = levelIndex;
            }
            else
            {
                Debug.Log($"Level score currently at {gameData.levelData[levelIndex].levelScore}");
            }

            gameData.levelData[levelIndex].levelScore = stars;

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
    }
}