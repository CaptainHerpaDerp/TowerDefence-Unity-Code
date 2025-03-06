using System;

namespace Saving
{
    [Serializable]
    public class SettingsData
    {
        public int resolutionX, resolutionY;
        public bool isFullscreen;
        public float soundEffectVolume;
        public float musicVolume;
    }
}