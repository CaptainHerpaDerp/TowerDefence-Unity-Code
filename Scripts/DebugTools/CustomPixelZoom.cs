using UnityEngine;

public class CustomPixelPerfectZoom : MonoBehaviour
{
    public float targetZoom = 2.5f; // Adjust this value between 2.0 (zoomed in) and 3.0 (zoomed out)
    private float baseOrthographicSize;

    public float resolution = 1080;

    void FixedUpdate()
    {
        baseOrthographicSize = (resolution / 2) / 32f; // 1080p height divided by PPU
        Camera.main.orthographicSize = baseOrthographicSize / targetZoom;
    }
}
