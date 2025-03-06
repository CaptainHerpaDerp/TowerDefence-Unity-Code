using Enemies;
using System;

namespace Saving
{
    [Serializable]
    public class GameData
    {
        public LevelData[] levelData;

        public SeenEnemyData[] seenEnemyData;

        //public SerializableDictionary<EnemyType, bool> seenEnemyInfo;

        public void InitializeLevelData()
        {
            // Change to however many levels we have
            levelData = new LevelData[3]
            {
                new LevelData { levelIndex = 0, levelScore = 0  },
                new LevelData { levelIndex = 1, levelScore = 0 },
                new LevelData { levelIndex = 2, levelScore = 0 }
            };
        }

        public void InitializeEnemySeenData()
        {
            seenEnemyData = new SeenEnemyData[Enum.GetValues(typeof(EnemyType)).Length];

            for (int i = 0; i < seenEnemyData.Length; i++)
            {
                seenEnemyData[i] = new SeenEnemyData { enemyType = (EnemyType)i, seen = false };
            }
        }

        public void MarkEnemyTypeInfoAsSeen(EnemyType type)
        {
            seenEnemyData[(int)type].seen = true;
        }
    }
}
