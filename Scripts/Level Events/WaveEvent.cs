using Enemies;

namespace LevelEvents
{
    /// <summary>
    /// Represents the type of wave event
    /// </summary>
    public enum WaveEventType
    {
        SpawnEnemy,
        WaitDuration
    }

    /// <summary>
    /// A wave event is contained within a wave and represents a single action that occurs during the wave
    /// </summary>
    [System.Serializable]
    public class WaveEvent
    {
        // Type of the wave event
        public WaveEventType eventType;

        public int spawnLocation = 0;

        public int endLocation = 0;

        // For spawning enemies
        public EnemyType enemyType;

        public int spawnQuantity = 0;

        public float spawnInterval = 0;

        // For waiting duration
        public float waitDuration = 0f;
    }
}
