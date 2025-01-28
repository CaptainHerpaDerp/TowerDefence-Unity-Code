using System.Collections.Generic;

namespace LevelEvents
{
    /// <summary>
    /// A collection of wave events
    /// </summary>
    [System.Serializable]
    public class Wave
    {
        // Initialize the list to avoid null reference
        public List<WaveEvent> waveEvents = new();

        public int nextWaveDelay = 0;
    }
}