using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class testScript : MonoBehaviour
{
    [SerializeField] private int scale;
    [SerializeField] private float PPU;

    [SerializeField] private bool apply;

    // Update is called once per frame
    void Update()
    {
        if (apply)
        {
            apply = false;
            float size = ((Screen.currentResolution.height) / (scale * PPU)) * 0.5f;
            Camera.main.orthographicSize = size;
        }
    }
}
