using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using Towers;

namespace Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
        public static AnalyticsManager Instance { get; private set; }

        [SerializeField] private bool sendAnalyticsInEditor = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Another instance of AnalyticsManager already exists. Destroying this instance.");
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        async void Start()
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
        }

        public void SendLevelRestartEvent(int levelIndex)
        {
            if (!sendAnalyticsInEditor && Application.isEditor)
            {
                Debug.LogWarning("Analytics are disabled in the editor.");
                return;
            }

            LevelRestartEvent newLevelRestartEvent = new()
            {
                LevelIndex = levelIndex + 1,
                WaveIndex = 1
            };

            AnalyticsService.Instance.RecordEvent(newLevelRestartEvent);

            Debug.Log("Level restart event sent!");
        }

        public void SendLevelLossEvent(int levelIndex, int waveIndex)
        {
            if (!sendAnalyticsInEditor && Application.isEditor)
            {
                Debug.LogWarning("Analytics are disabled in the editor.");
                return;
            }

            LevelLossEvent newLevelLossEvent = new()
            {
                LevelIndex = levelIndex + 1,
                WaveIndex = waveIndex + 1
            };

            AnalyticsService.Instance.RecordEvent(newLevelLossEvent);

            Debug.Log("Level loss event sent!");
        }

        public void SendLevelWonEvent(int levelIndex, int starCount)
        {
            if (!sendAnalyticsInEditor && Application.isEditor)
            {
                Debug.LogWarning("Analytics are disabled in the editor.");
                return;
            }

            LevelWonEvent newLevelWonEvent = new()
            {
                LevelIndex = levelIndex + 1,
                StarCount = starCount + 1
            };

            AnalyticsService.Instance.RecordEvent(newLevelWonEvent);

            Debug.Log("Level won event sent!");
        }

        public void SentTowerTypeConstructed(TowerType towerType)
        {
            if (!sendAnalyticsInEditor && Application.isEditor)
            {
             //   Debug.LogWarning("Analytics are disabled in the editor.");
                return;
            }

            TowerConstructedEvent towerConstructedEvent = new()
            {
                TowerType = towerType.ToString()
            };

            AnalyticsService.Instance.RecordEvent(towerConstructedEvent);

            Debug.Log("Tower constructed event sent!");
        }
    }

    public class LevelRestartEvent : Unity.Services.Analytics.Event
    {
        public LevelRestartEvent() : base("levelRestarted")
        {
        }

        public int LevelIndex { set { SetParameter("levelIndex", value); } }
        public int WaveIndex { set { SetParameter("waveIndex", value); } }
    }

    public class LevelLossEvent : Unity.Services.Analytics.Event
    {
        public LevelLossEvent() : base("levelLost")
        {
        }

        public int LevelIndex { set { SetParameter("levelIndex", value); } }
        public int WaveIndex { set { SetParameter("waveIndex", value); } }
    }

    public class LevelWonEvent : Unity.Services.Analytics.Event
    {
        public LevelWonEvent() : base("levelWon")
        {
        }

        public int LevelIndex { set { SetParameter("levelIndex", value); } }
        public int StarCount { set { SetParameter("starCount", value); } }
    }

    public class TowerConstructedEvent : Unity.Services.Analytics.Event
    {
        public TowerConstructedEvent() : base("towerConstructed")
        {
        }

        public string TowerType { set { SetParameter("towerType", value); } }
    }
}