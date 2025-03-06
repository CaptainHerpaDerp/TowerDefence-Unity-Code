using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerTest : MonoBehaviour
{

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask waterLayer;

    private void Update()
    {
        if (IsOverWater())
        {
            Debug.Log("Over water");
        }
    }

    /// <summary>
    /// Returns true if the projectile is over water
    /// </summary>
    /// <returns></returns>
    protected bool IsOverWater()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, 0, groundLayer))
        {
            return false;
        }

        return Physics2D.Raycast(transform.position, Vector2.down, 0, waterLayer);
    }
}
