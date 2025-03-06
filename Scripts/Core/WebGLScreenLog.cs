using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Core.Debugging
{
    public class WebGLScreenLog : PersistentSingleton<WebGLScreenLog>
    {
        [BoxGroup("Settings"), SerializeField] private int maxLogs;

        private int currentLogs;

        [BoxGroup("Components"), SerializeField] private TextMeshProUGUI logText;
        [BoxGroup("Components"), SerializeField] private Button clearLogButton;

        [BoxGroup("Debug"), SerializeField] private bool debugMode;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // Destroy if we're not in development mode

            if (Application.platform == RuntimePlatform.WebGLPlayer || debugMode)
            {
                Log("WebGL Detected - Enabling Screen Log");
                clearLogButton.onClick.AddListener(ClearLog);
            }
            else
            {
                Debug.Log("WebGL Not Detected - Screen Log Disabled");
                Destroy(gameObject);
            }
        }

        private void ClearLog()
        {
            logText.text = "";
            currentLogs = 0;
        }

        public void Log(string message)
        {
            if (currentLogs >= maxLogs)
            {
                // Remove the first line
                logText.text = logText.text.Substring(logText.text.IndexOf("\n") + 1);
            }

            logText.text += message + "\n";
            currentLogs++;
        }

        [Button("Test Log")]
        private void TestLog()
        {
            Log("Test Log Message");
        }
    }

}