using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCheck : MonoBehaviour
{
    [Header("Radius Settings")]
    public float radius = 1f; 

    [Header("Gizmo Settings")]
    public Color gizmoColor = Color.green; 
    
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
