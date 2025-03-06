using Enemies;
using UnityEngine;

namespace Saving
{
    /// <summary>
    /// Saves and loads game progression data to and from a JSON file
    /// </summary>

    public abstract class SaveData : MonoBehaviour
    {
        public GameData gameData = new();
        public SettingsData settingsData = new();

        public int NativeX { get; protected set; }
        public int NativeY { get; protected set; }

        // If a player has just completed a level, this will be set to the index of the newly completed level.
        public int NewlyCompletedLevelIndex = -1;

        /// <summary>
        /// Called on start
        /// </summary>
        public abstract void Initialize();

        #region Game Save Data


        public abstract void SaveGameData();

        public abstract void LoadGameData();

        public abstract void InitializeSaveData();

        #endregion

        #region Graphics Save Data

        public abstract void SaveSettingsData();

        public abstract void LoadSettingsData();

        #endregion

        public abstract int GetLevelStars(int levelIndex);

        public abstract void SetLevelStars(int levelIndex, int stars);

        public abstract int GetCompletedLevelCount();

        public abstract bool HasSeenInfoOf(EnemyType type);

        public abstract void MarkEnemyTypeInfoAsSeen(EnemyType type);
    }
}

