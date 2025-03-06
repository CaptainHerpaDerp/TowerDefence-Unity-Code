using Enemies;
using System;

namespace Saving
{
    [Serializable]
    public class SeenEnemyData
    {
        public EnemyType enemyType;
        public bool seen;
    }
}