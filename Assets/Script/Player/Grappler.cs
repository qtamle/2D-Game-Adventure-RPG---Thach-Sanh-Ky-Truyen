using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : MonoBehaviour
{
    public Camera mainCamera;
    public LineRenderer lineRenderer;
    public DistanceJoint2D distanceJoint;
    public string pointTag = "Point";  // Tag cho các điểm grappler
    public float maxGrappleDistance = 10f; // Khoảng cách tối đa để grappler
    private bool isGrappling = false;

    private void Start()
    {
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 closestPoint = FindClosestPoint();
            if (closestPoint != Vector2.zero)
            {
                lineRenderer.SetPosition(0, closestPoint);
                lineRenderer.SetPosition(1, transform.position);
                distanceJoint.connectedAnchor = closestPoint;
                distanceJoint.enabled = true;
                lineRenderer.enabled = true;
                isGrappling = true;
            }
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            distanceJoint.enabled = false;
            lineRenderer.enabled = false;
            isGrappling = false;
        }

        if (distanceJoint.enabled)
        {
            lineRenderer.SetPosition(1, transform.position);
        }
    }

    private Vector2 FindClosestPoint()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag(pointTag);
        float closestDistance = Mathf.Infinity;
        Vector2 closestPoint = Vector2.zero;

        foreach (GameObject point in points)
        {
            float distance = Vector2.Distance(transform.position, point.transform.position);
            if (distance < closestDistance && distance <= maxGrappleDistance)
            {
                closestDistance = distance;
                closestPoint = point.transform.position;
            }
        }

        // Trả về Vector2.zero nếu không có điểm nào trong phạm vi
        return closestDistance <= maxGrappleDistance ? closestPoint : Vector2.zero;
    }

    public bool IsGrappling()
    {
        return isGrappling;
    }
}
