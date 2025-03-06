using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

public class FixPPUPositioningEditor : EditorWindow
{
    private const float oldPPU = 100f; // Your original PPU
    private const float newPPU = 32f;  // Your new PPU
    private const float scaleFactor = oldPPU / newPPU; // Scaling factor

    [MenuItem("Tools/Fix PPU Positioning")]
    public static void FixPositions()
    {
        if (!EditorUtility.DisplayDialog("Fix PPU Positioning",
            "This will permanently adjust the positions of all objects in the scene. Are you sure?", "Fix", "Cancel"))
        {
            return;
        }

        Transform[] allObjects = FindObjectsByType<Transform>(FindObjectsSortMode.None);

        Undo.RecordObjects(allObjects, "Fix PPU Positioning");  

        foreach (Transform obj in allObjects)
        {
            
                obj.position = new Vector3(
                    obj.position.x * scaleFactor,
                    obj.position.y * scaleFactor,
                    obj.position.z
                );
            
        }

        Debug.Log("All object positions have been permanently adjusted for new PPU!");
    }
}

#endif