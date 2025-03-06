using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Core;
using System.Collections;

/// <summary>
/// A script that ensures the game runs at the best resolution for WebGL
/// </summary>
public class WebGLResolutionFix : PersistentSingleton<WebGLResolutionFix>
{

    [DllImport("__Internal")]
    private static extern int GetBrowserWidth();

    [DllImport("__Internal")]
    private static extern int GetBrowserHeight();

    // We are going to track the screen size, so that if it changes we can apply the best resolution
    private int lastScreenWidth, lastScreenHeight;

    void Start()
    {
        // Ensure the game starts at the correct size

        Screen.fullScreen = false;

        // Apply UI scaling fix
        // CanvasScalerFix();
    }

    void CanvasScalerFix()
    {
        CanvasScaler scaler = FindFirstObjectByType<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); // Adjust based on your UI design
            scaler.matchWidthOrHeight = 0.5f; // Balance between width & height
        }
    }

    void Update() 
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            int browserWidth = GetBrowserWidth();
            int browserHeight = GetBrowserHeight();

            if (browserWidth != lastScreenWidth || browserHeight != lastScreenHeight)
            {
                lastScreenWidth = browserWidth;
                lastScreenHeight = browserHeight;

                Debug.Log($"Updating resolution to {browserWidth}x{browserHeight}");

                // Apply the new resolution with a forced refresh
                StartCoroutine(ForceResolutionUpdate(browserWidth, browserHeight));
            }
        }
    }

    // Coroutine to wait a frame and force update
    IEnumerator ForceResolutionUpdate(int width, int height)
    {
        yield return new WaitForEndOfFrame();
        ApplyBestResolution(width, height);
        yield return new WaitForSeconds(0.1f);
        ApplyBestResolution(width, height);
    }


    void ApplyBestResolution(int browserWidth, int browserHeight)
    {
        if (browserWidth >= 3840 && browserHeight >= 2160)
        {
            Screen.SetResolution(3840, 2160, false);
            EventBus.Instance.Publish("TargResolution", new Vector2(3840, 2160));
        }
        else if (browserWidth >= 2560 && browserHeight >= 1440)
        {
            Screen.SetResolution(2560, 1440, false);
            EventBus.Instance.Publish("TargResolution", new Vector2(2560, 1440));
        }
        else
        {
            Screen.SetResolution(1280, 720, false);
            EventBus.Instance.Publish("TargResolution", new Vector2(1280, 720));
        }
    }
}
