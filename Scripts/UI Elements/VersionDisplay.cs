using System.IO;
using UnityEngine;
using TMPro;
using Core;

namespace UIElements
{
    public class VersionDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI versionTextComponent;

        [SerializeField] private TextMeshProUGUI targResolutionComponent;

        void Start()
        {
            string version = GetVersion();
            versionTextComponent.text = version;

            EventBus.Instance.Subscribe("TargResolution", SetTargResolution);
        }

        private void SetTargResolution(object resolutionObj)
        {
            Vector2 resolution = (Vector2)resolutionObj;

            if (resolution != null)
            {
                targResolutionComponent.text = resolution.x + " : " + resolution.y;
            }
            else
            {
                targResolutionComponent.text = "Unknown";
            }
        }

        string GetVersion()
        {
            string versionFilePath = "version";
            TextAsset versionFile = Resources.Load<TextAsset>(versionFilePath);

            if (versionFile != null)
            {
                // Read only the first line of the file
                StringReader stringReader = new StringReader(versionFile.text);
                string version = stringReader.ReadLine();
                return version;
            }
            else
            {
                Debug.LogError("Version file not found");
                return "Unknown";
            }
        }
    }
}
