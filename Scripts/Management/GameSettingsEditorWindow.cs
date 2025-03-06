#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using GameManagement;

public class GameSettingsEditorWindow : OdinEditorWindow
{
    public static GameSettings CurrentRealAsset { get; private set; }

    [MenuItem("Window/Game Settings Editor")]
    private static void OpenWindow()
    {
        GetWindow<GameSettingsEditorWindow>().Show();
    }

    // The actual asset stored on disk.
    [SerializeField, HideInInspector]
    private GameSettings realGameSettings;

    // The working copy that you edit in the inspector.
    [InlineEditor(InlineEditorModes.FullEditor)]
    [LabelText("Game Settings (Working Copy)")]
    public GameSettings gameSettings;
    
    private bool gameSettingsLoaded => gameSettings != null;

    protected override void OnEnable()
    {
        base.OnEnable();

        // If no real asset is loaded, create a temporary default asset.
        if (realGameSettings == null)
        {
            realGameSettings = CreateInstance<GameSettings>();
        }

        // Always create a working copy from the real asset.
        CreateWorkingCopy();
    }

    /// <summary>
    /// Creates a working copy from the real asset.
    /// </summary>
    private void CreateWorkingCopy()
    {
        if (realGameSettings != null)
        {
            // Instantiate a working copy from the real asset.
            gameSettings = Instantiate(realGameSettings);
            // Prevent the working copy from being saved with the scene/project.
            gameSettings.hideFlags = HideFlags.DontSave;

            CurrentRealAsset = realGameSettings;
        }
    }

    [PropertyOrder(-1)]
    [HorizontalGroup("ButtonGroup")]
    [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
    private void CreateGameSettings()
    {
        string path = EditorUtility.SaveFilePanel("Create Game Settings", "Assets/GameSettings", "NewGameSettings", "asset");

        if (!string.IsNullOrEmpty(path))
        {
            string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
            var newSettings = CreateInstance<GameSettings>();
            AssetDatabase.CreateAsset(newSettings, relativePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Set the newly created asset as the real asset and update the working copy.
            realGameSettings = newSettings;
            CreateWorkingCopy();
        }
    }

    [PropertyOrder(-1)]
    [HorizontalGroup("ButtonGroup")]
    [Button(ButtonSizes.Large), GUIColor(0.4f, 1f, 0.4f)]
    private void LoadGameSettings()
    {
        string path = EditorUtility.OpenFilePanel("Load Game Settings", "Assets/GameSettings", "asset");

        if (!string.IsNullOrEmpty(path))
        {
            string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
            var loadedSettings = AssetDatabase.LoadAssetAtPath<GameSettings>(relativePath);
            if (loadedSettings != null)
            {
                // Set the loaded asset as the real asset and update the working copy.
                realGameSettings = loadedSettings;
                CreateWorkingCopy();
            }
            else
            {
                Debug.LogError($"Failed to load GameSettings at path: {relativePath}");
            }
        }
    }

    [PropertyOrder(-1)]
    [HorizontalGroup("ButtonGroup")]
    [Button(ButtonSizes.Large), EnableIf("gameSettingsLoaded"), GUIColor(1f, 0.85f, 0.4f)]
    private void SaveGameSettings()
    {
        if (gameSettings == null || realGameSettings == null)
            return;

        // Copy all serialized data from the working copy to the real asset.
        EditorUtility.CopySerialized(gameSettings, realGameSettings);
        EditorUtility.SetDirty(realGameSettings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("GameSettings saved successfully!");
    }

    [PropertyOrder(-1)]
    [HorizontalGroup("ButtonGroup")]
    [Button(ButtonSizes.Large), EnableIf("gameSettingsLoaded"), GUIColor(1f, 0.5f, 0.5f)]
    private void ApplyGameSetting()
    {
        // First, ensure changes from the working copy are saved to the asset.
        SaveGameSettings();
        SettingsApplier.Instance.ApplySettings(realGameSettings);
        GameDifficultyCalculator.Instance.DoDifficultyRatingCalculations(realGameSettings);
        Debug.Log("Applied current GameSettings to the game.");
    }
}
#endif
