using UnityEngine;
using System;
using System.IO;
using Enemies;
using Core;

namespace Saving
{
    /// <summary>
    /// Saves and loads game progression data to and from a JSON file
    /// </summary>
    public class SaveData : MonoBehaviour
    {
        public static SaveData Instance { get; private set; }

        public GameData gameData = new();
        public SettingsData settingsData = new();

        private SoundEffectManager soundEffectManager;

        // If a player has just completed a level, this will be set to the index of the newly completed level.
        public int NewlyCompletedLevelIndex = -1;

        public int NativeX { get; private set; }
        public int NativeY { get; private set; }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogWarning("More than one SaveData instance found in the scene, destroying the new one");
                Destroy(gameObject);
            }

            LoadGameDataFromJson();
            LoadSettingsDataFromJson();
        }

        public void Start()
        {
            soundEffectManager = SoundEffectManager.Instance;

            // Apply the soundeffect manager volume settings
            soundEffectManager.SoundEffectVolume = settingsData.soundEffectVolume;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Saving data");
                SaveGameDataToJson();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("Loading save data");
                SaveGameDataToJson();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("Clearing save data");
                ClearGameSaveData();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("Marking all enemies as seen settings data");
                MarkAllEnemiesAsSeen();
            }
        }

        #region Game Data

        public void SaveGameDataToJson()
        {
            string json = JsonUtility.ToJson(gameData);
            string filePath = Application.persistentDataPath + "/gameData.json";
            //Debug.Log(filePath);

            File.WriteAllText(filePath, json);
        }

        public void LoadGameDataFromJson()
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

                gameData = new();
                gameData.levelData = new LevelData[3];

                for (int i = 0; i < 3; i++)
                {
                    gameData.levelData[i] = new LevelData();
                    gameData.levelData[i].levelIndex = i;
                    gameData.levelData[i].levelScore = 0;
                }

                SaveGameDataToJson();
            }
        }

        public void ClearGameSaveData()
        {
            gameData = new();

            // Add 0 stars to the first level:
            gameData.levelData = new LevelData[3];

            for (int i = 0; i < gameData.levelData.Length; i++)
            {
                gameData.levelData[i] = new LevelData();
                gameData.levelData[i].levelIndex = i;
                gameData.levelData[i].levelScore = 0;
            }

            SaveGameDataToJson();
        }

        private void MarkAllEnemiesAsSeen()
        {
            foreach (EnemyType type in Enum.GetValues(typeof(EnemyType)))
            {
                MarkEnemyTypeInfoAsSeen(type);
            }
        }

        #endregion

        #region Graphics Data

        public void SaveSettingsDataToJson()
        {
            string json = JsonUtility.ToJson(settingsData);
            string filePath = Application.persistentDataPath + "/settingsData.json";
            //Debug.Log(filePath);

            File.WriteAllText(filePath, json);
        }

        public void LoadSettingsDataFromJson()
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

                SaveSettingsDataToJson();
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
                Camera.main.transform.rotation = Quaternion.Euler(0.1f, 0.001f, 0);
            }
        }

        public void ClearSettingsData()
        {
            settingsData = new();

            settingsData.resolutionX = 1920;
            settingsData.resolutionY = 1080;
            settingsData.isFullscreen = false;

            SaveGameDataToJson();
        }

        #endregion

        public int GetLevelStars(int levelIndex)
        {
            return gameData.levelData[levelIndex].levelScore;
        }

        public void SetLevelStars(int levelIndex, int stars)
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
                NewlyCompletedLevelIndex = levelIndex;
            }

            gameData.levelData[levelIndex].levelScore = stars;


            SaveGameDataToJson();
        }

        public int GetCompletedLevelCount()
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


        public bool HasSeenInfoOf(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Orc:
                    return gameData.hasSeenOrcInfo;
                case EnemyType.Wolf:
                    return gameData.hasSeenWolfInfo;
                case EnemyType.Slime:
                    return gameData.hasSeenSlimeInfo;
                case EnemyType.SpikedSlime:
                    return gameData.hasSeenSpikedSlimeInfo;
                case EnemyType.MountedOrc:
                    return gameData.hasSeenMountedOrcInfo;
                case EnemyType.Bee:
                    return gameData.hasSeenBeeInfo;
                case EnemyType.QueenBee:
                    return gameData.hasSeenQueenBeeInfo;
                case EnemyType.Squid:
                    return gameData.hasSeenSquidInfo;
                case EnemyType.Angler:
                    return gameData.hasSeenAnglerInfo;
                case EnemyType.Turtle:
                    return gameData.hasSeenTurtleInfo;
                case EnemyType.Gull:
                    return gameData.hasSeenGullInfo;
                case EnemyType.KingAngler:
                    return gameData.hasSeenKingAnglerInfo;
                case EnemyType.GiantSquid:
                    return gameData.hasSeenGiantSquidInfo;
                case EnemyType.ElderTurtle:
                    return gameData.hasSeenElderTurtleInfo;
                case EnemyType.Larva:
                    return gameData.hasSeenLarvaInfo;
                case EnemyType.Witch:
                    return gameData.hasSeenWitchInfo;
                case EnemyType.Lizard:
                    return gameData.hasSeenLizardInfo;
                case EnemyType.BombBat:
                    return gameData.hasSeenBombBatInfo;
                case EnemyType.GiantLizard:
                    return gameData.hasSeenGiantLizardInfo;
                case EnemyType.QueenLarva:
                    return gameData.hasSeenQueenLarvaInfo;
                case EnemyType.Treeman:
                    return gameData.hasSeenTreemanInfo;
                default:
                    Debug.LogError($"Invalid enemy type {type}");
                    return false;
            }
        }

        public void MarkEnemyTypeInfoAsSeen(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Orc:
                    gameData.hasSeenOrcInfo = true;
                    break;
                case EnemyType.Wolf:
                    gameData.hasSeenWolfInfo = true;
                    break;
                case EnemyType.Slime:
                    gameData.hasSeenSlimeInfo = true;
                    break;
                case EnemyType.SpikedSlime:
                    gameData.hasSeenSpikedSlimeInfo = true;
                    break;
                case EnemyType.MountedOrc:
                    gameData.hasSeenMountedOrcInfo = true;
                    break;
                case EnemyType.Bee:
                    gameData.hasSeenBeeInfo = true;
                    break;
                case EnemyType.QueenBee:
                    gameData.hasSeenQueenBeeInfo = true;
                    break;
                case EnemyType.Squid:
                    gameData.hasSeenSquidInfo = true;
                    break;
                case EnemyType.Angler:
                    gameData.hasSeenAnglerInfo = true;
                    break;
                case EnemyType.Turtle:
                    gameData.hasSeenTurtleInfo = true;
                    break;
                case EnemyType.Gull:
                    gameData.hasSeenGullInfo = true;
                    break;
                case EnemyType.KingAngler:
                    gameData.hasSeenKingAnglerInfo = true;
                    break;
                case EnemyType.GiantSquid:
                    gameData.hasSeenGiantSquidInfo = true;
                    break;
                case EnemyType.ElderTurtle:
                    gameData.hasSeenElderTurtleInfo = true;
                    break;
                case EnemyType.Larva:
                    gameData.hasSeenLarvaInfo = true;
                    break;
                case EnemyType.Witch:
                    gameData.hasSeenWitchInfo = true;
                    break;
                case EnemyType.Lizard:
                    gameData.hasSeenLizardInfo = true;
                    break;
                case EnemyType.BombBat:
                    gameData.hasSeenBombBatInfo = true;
                    break;
                case EnemyType.GiantLizard:
                    gameData.hasSeenGiantLizardInfo = true;
                    break;
                case EnemyType.QueenLarva:
                    gameData.hasSeenQueenLarvaInfo = true;
                    break;
                case EnemyType.Treeman:
                    gameData.hasSeenTreemanInfo = true;
                    break;

                default:
                    Debug.LogError($"Invalid enemy type {type}");
                    break;
            }

            SaveGameDataToJson();
        }
    }

    [Serializable]
    public class LevelData
    {
        public int levelIndex;
        public int levelScore;
    }

    [Serializable]
    public class GameData
    {
        public LevelData[] levelData;

        public bool hasSeenOrcInfo = false, hasSeenWolfInfo, hasSeenSlimeInfo, hasSeenSpikedSlimeInfo, hasSeenMountedOrcInfo, hasSeenBeeInfo, hasSeenQueenBeeInfo,
            hasSeenSquidInfo, hasSeenAnglerInfo, hasSeenTurtleInfo, hasSeenGullInfo, hasSeenKingAnglerInfo, hasSeenGiantSquidInfo, hasSeenElderTurtleInfo,
            hasSeenLarvaInfo, hasSeenWitchInfo, hasSeenLizardInfo, hasSeenBombBatInfo, hasSeenGiantLizardInfo, hasSeenQueenLarvaInfo, hasSeenTreemanInfo;
    }

    [Serializable]
    public class SettingsData
    {
        public int resolutionX, resolutionY;
        public bool isFullscreen;
        public float soundEffectVolume;
        public float musicVolume;
    }
}

