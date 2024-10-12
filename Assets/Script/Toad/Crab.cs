using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab : MonoBehaviour
{
    public float speed;
    public float destroyTime;

    public Transform detectionPoint; 
    public float detectionRadius = 0.1f;
    public LayerMask groundLayer;

    private bool canMove = false;

    private void Start()
    {
        IgnoreCollisionWithLayers();

        StartCoroutine(DestroyAfterTime());

        transform.localScale = new Vector3(0.5682886f, 0.5682886f, 0.5682886f);
    }

    private void Update()
    {
        DetectGround();
        Debug.Log($"canMove: {canMove}");
        if (canMove)
        {
            CrabMove();
        }
    }
    private void CrabMove()
    {
        // Di chuyển về phía bên phải
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Flip nếu Crab đi sang trái hoặc phải
        if (transform.position.x < 0)
        {
            transform.localScale = new Vector3(-0.5682886f, 0.5682886f, 0.5682886f);
        }
        else
        {
            transform.localScale = new Vector3(0.5682886f, 0.5682886f, 0.5682886f);
        }

        Debug.Log($"Crab đang ở vị trí: {transform.position}");
    }


    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
    private void DetectGround()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, detectionRadius, groundLayer);

        if (hits.Length > 0)
        {
            canMove = true;
            foreach (Collider2D hit in hits)
            {
                Debug.Log($"Crab va chạm với: {hit.gameObject.name} (Layer: {LayerMask.LayerToName(hit.gameObject.layer)})");
            }
        }
        else
        {
            canMove = false; 
        }
    }

    private void IgnoreCollisionWithLayers()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int blockLayer = LayerMask.NameToLayer("TurnOn");
        int bubbleLayer = LayerMask.NameToLayer("Bubble");
        int crabLayer = LayerMask.NameToLayer("Crab");

        Physics2D.IgnoreLayerCollision(playerLayer, LayerMask.NameToLayer("Crab"), true);
        Physics2D.IgnoreLayerCollision(enemyLayer, LayerMask.NameToLayer("Crab"), true);
        Physics2D.IgnoreLayerCollision(blockLayer, LayerMask.NameToLayer("Crab"), true);
        Physics2D.IgnoreLayerCollision(bubbleLayer, LayerMask.NameToLayer("Crab"), true);
        Physics2D.IgnoreLayerCollision(crabLayer, LayerMask.NameToLayer("Crab"), true);
    }

    private void OnDrawGizmos()
    {
        if (detectionPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectionPoint.position, detectionRadius);
        }
    }

}
