using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sapling : MonoBehaviour
{
    [Header("Move")]
    public float speed = 2f;
    public float radius = 10f;
    public Transform playerTransform;
    public LayerMask playerMask;
    public bool moveRight = true;
    private bool isChasing = false;
    public Collider2D playerCollider;

    [Header("Explode")]
    public float radiusExplode = 10f;
    public float explodeDelay = 2f;
    public float damage = 5f;
    public bool isPrepare = false;
    public bool isExploded = false;
    private bool isInExplosionRange = false;

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
        isChasing = Physics2D.OverlapCircle(transform.position, radius, playerMask);

        // Kiểm tra xem Player có nằm trong bán kính kích hoạt vụ nổ không
        isInExplosionRange = Physics2D.OverlapCircle(transform.position, radiusExplode, playerMask);

        if (isChasing && !isInExplosionRange && playerTransform != null && !isPrepare)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            Vector2 moveDirection = new Vector2(direction.x, 0).normalized;
            transform.Translate(moveDirection * speed * Time.deltaTime);

            if (direction.x > 0)
            {
                transform.localScale = new Vector2(-1, 1);
            }
            else
            {
                transform.localScale = new Vector2(1, 1);
            }
        }
        else if (isInExplosionRange && !isExploded)
        {
            isPrepare = true;
            StartCoroutine(Explode());
        }
        else if (!isPrepare && !isInExplosionRange)
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

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(explodeDelay);

        Collider2D player = Physics2D.OverlapCircle(transform.position, radiusExplode, playerMask);
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(damage, 0.5f, 0.65f, 0.1f);
            }
        }

        isExploded = true;
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radiusExplode);
    }
}
