using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float speed = 2f; 
    public float detectionRadius = 5f; 
    public Transform player;
    public LayerMask playerLayer; 

    public bool moveRight = true; 

    private bool isChasing = false;

    public Collider2D playerCollider;
    private void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);

        if (playerCollider != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider);
        }
    }


    private void Update()
    {
        // Kiểm tra xem Player có nằm trong bán kính phát hiện không
        isChasing = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (isChasing && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 moveDirection = new Vector2(direction.x, 0).normalized;
            transform.Translate(moveDirection * speed * Time.deltaTime);

            if (direction.x > 0)
            {
                transform.localScale = new Vector2(-1, 1); // Hướng về bên phải
            }
            else
            {
                transform.localScale = new Vector2(1, 1); // Hướng về bên trái
            }
        }
        else
        {
            if (moveRight)
            {
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                transform.localScale = new Vector2(-1, 1);
            }
            else
            {
                transform.Translate(Vector2.left * speed * Time.deltaTime);
                transform.localScale = new Vector2(1, 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Turn"))
        {
            moveRight = !moveRight; 
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
