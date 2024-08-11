using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeClimb : MonoBehaviour
{
    public LayerMask ledgeLayer; 
    public float raycastDistance = 0.5f; 
    public Vector2 climbOffset = new Vector2(0.5f, 1f); 
    public float climbSpeed = 2f;

    private bool isClimbing = false;
    private Vector2 climbPosition;
    private bool moveUp = false;

    void Update()
    {
        if (!isClimbing)
        {
            CheckLedgeClimb();
        }
        else
        {
            ClimbLedge();
        }
    }

    void CheckLedgeClimb()
    {
        // Raycast từ mặt nhân vật
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, raycastDistance, ledgeLayer);

        if (hit.collider != null)
        {
            // Debug khi Raycast chạm vào Layer Ledge
            Debug.Log("Chạm vào Ledge: " + hit.collider.name);

            // Tính toán vị trí cần leo lên
            climbPosition = new Vector2(hit.point.x + climbOffset.x * transform.localScale.x, hit.point.y + climbOffset.y);
            isClimbing = true;
            moveUp = true; 
        }
    }

    void ClimbLedge()
    {
        if (moveUp)
        {
            // Di chuyển nhân vật theo trục y lên trên
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, climbPosition.y), climbSpeed * Time.deltaTime);

            if (Mathf.Abs(transform.position.y - climbPosition.y) < 0.01f)
            {
                moveUp = false; 
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(climbPosition.x, transform.position.y), climbSpeed * Time.deltaTime);

            if (Mathf.Abs(transform.position.x - climbPosition.x) < 0.01f)
            {
                isClimbing = false; 
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * transform.localScale.x * raycastDistance);
    }
}
