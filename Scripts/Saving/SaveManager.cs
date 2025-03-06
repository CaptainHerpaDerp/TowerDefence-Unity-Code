using Core;
using Enemies;
using UnityEngine;

namespace Saving
{

    public class SaveManager : PersistentSingleton<SaveManager>
    {
        public static SaveData SaveSystem { get; private set; }

        // Debug
        [SerializeField] private bool isWebGL;

        protected override void Awake()
        {
            base.Awake();

            // Choose the correct save system based on platform
            if (Application.platform == RuntimePlatform.WebGLPlayer || isWebGL)
            {
                SaveSystem = gameObject.AddComponent<WebGLSaveData>();

                SaveSystem.LoadGameData();

                gameObject.name = "WebGLSaveData"; 

                Debug.Log("Using WebGL Save System");
            }
            else
            {
                SaveSystem = gameObject.AddComponent<JsonSaveData>();
                Debug.Log("Using JSON Save System");

                SaveSystem.LoadGameData();
                SaveSystem.LoadSettingsData();
            }             
        }

        private void Start()
        {
            SaveSystem.Initialize();
        }

        public void SaveGame()
        {
            SaveSystem.SaveGameData();
        }

        public void LoadGame()
        {
            SaveSystem.LoadGameData();
        }

        public void SaveSettings()
        {
            SaveSystem.SaveSettingsData();
        }

        #region Game Data Getters

        public int GetLevelStars(int levelIndex) => SaveSystem.GetLevelStars(levelIndex);

        public bool HasSeenInfoOf(EnemyType type) => SaveSystem.HasSeenInfoOf(type);

        public int GetCompletedLevelCount() => SaveSystem.GetCompletedLevelCount();

        public int NewlyCompletedLevelIndex() => SaveSystem.NewlyCompletedLevelIndex;

        #endregion

        #region Game Data Setters

        public void MarkEnemyTypeInfoAsSeen(EnemyType type) => SaveSystem.MarkEnemyTypeInfoAsSeen(type);
        public void SetLevelStars(int levelIndex, int stars) => SaveSystem.SetLevelStars(levelIndex, stars);
        public void SetNewlyCompletedLevel(int levelIndex) => SaveSystem.NewlyCompletedLevelIndex = levelIndex;

        #endregion

        #region Volume Setters

        public void SetSoundEffectVolume(float volume)
        {
            SaveSystem.settingsData.soundEffectVolume = volume;
            SaveSystem.SaveSettingsData();
        }

        public void SetMusicVolume(float volume)
        {
            SaveSystem.settingsData.musicVolume = volume;
            SaveSystem.SaveSettingsData();
        }

        #endregion

        #region Volume Getters

        public float GetSoundEffectVolume()
        {
            return SaveSystem.settingsData.soundEffectVolume; 
        }

        public float GetMusicVolume()
        {
            return SaveSystem.settingsData.musicVolume;
        }

        #endregion

        #region Settings Getters

        public int NativeX => SaveSystem.NativeX;
        public int NativeY => SaveSystem.NativeY;

        public int ResolutionX => SaveSystem.settingsData.resolutionX;
        public int ResolutionY => SaveSystem.settingsData.resolutionY;

        public bool IsFullscreen => SaveSystem.settingsData.isFullscreen;

        #endregion

        #region Settings Setters

        public void SetResolution(int x, int y)
        {
            SaveSystem.settingsData.resolutionX = x;
            SaveSystem.settingsData.resolutionY = y;
            SaveSystem.SaveSettingsData();
        }

        public void SetFullscreen(bool isFullscreen)
        {
            SaveSystem.settingsData.isFullscreen = isFullscreen;
            SaveSystem.SaveSettingsData();
        }

        #endregion
    }
}