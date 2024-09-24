using System.Collections;
using UnityEngine;

public class Sapling : MonoBehaviour
{
    [Header("Move")]
    public float speed = 2f;
    public float radius = 10f;
    public LayerMask playerMask;
    public bool moveRight = true;
    private bool isChasing = false;
    private bool isPrepare = false;
    private bool isExploded = false;
    private bool isInExplosionRange = false;
    private bool isOnGround = false;
    private Transform playerTransform;
    public Collider2D playerCollider;

    [Header("Explode")]
    public float radiusExplode = 10f;
    public float explodeDelay = 2f;
    public float damage = 5f;

    private void Start()
    {
        FindPlayer();

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int saplingLayer = gameObject.layer;
        int otherLayer = LayerMask.NameToLayer("Other");

        Physics2D.IgnoreLayerCollision(saplingLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(saplingLayer, otherLayer, true);

        if (playerCollider != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider);
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerCollider = player.GetComponent<Collider2D>();
        }
        else
        {
            Debug.LogWarning("Player with tag 'Player' not found.");
        }
    }
    private void Update()
    {
        if (isOnGround)
        {
            isChasing = Physics2D.OverlapCircle(transform.position, radius, playerMask);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isOnGround = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radiusExplode);
    }
}
