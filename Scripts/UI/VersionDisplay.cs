using System.IO;
using UnityEngine;

namespace UI
{
    public class VersionDisplay : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI versionTextComponent;

        void Start()
        {
            string version = GetVersion();
            versionTextComponent.text = version;
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
