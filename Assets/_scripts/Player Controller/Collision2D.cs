using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision2D : MonoBehaviour
{
    // Constants
    float maxDistance = 1000f;

    void Update()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + (Vector3.down * 3);

        Color color;

        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector3.down, maxDistance);

        if (hit.collider != null)
        {
            color = Color.green;
            Debug.Log(hit.collider.name);
        }
        else
        {
            color = Color.magenta;
            Debug.Log(hit.collider.name);
        }

        Debug.DrawLine(startPos, endPos, color);
    }
}
