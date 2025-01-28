using System.Collections.Generic;
using UnityEngine;

namespace LevelEvents
{
    /// <summary>
    /// Contains the information for a level as well as a list of waves
    /// </summary>
    [System.Serializable]
    public class Level : ScriptableObject
    {
        public int LevelIndex;
        public int StartingLives = 10;
        public int StartingGold;

        public List<Wave> waves = new();
    }
}