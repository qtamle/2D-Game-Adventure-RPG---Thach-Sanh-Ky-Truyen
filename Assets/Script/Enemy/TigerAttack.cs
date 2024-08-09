using System.Collections;
using UnityEngine;

public class TigerAttack : MonoBehaviour
{
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public Transform player;
    public LayerMask playerMask;
    public EnemyMove enemyMove;

    private bool isAttack = false;
    private PlayerMovement playerMovement;
    private bool isWaiting = false;

    private void Start()
    {
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement == null)
            {
                Debug.LogError("Player không có PlayerMovement component.");
            }
        }
        else
        {
            Debug.LogError("Player chưa được gán trong Inspector.");
        }

        if (enemyMove == null)
        {
            enemyMove = GetComponent<EnemyMove>();
            if (enemyMove == null)
            {
                Debug.LogError("Không tìm thấy EnemyMove component.");
            }
        }
    }

    private void Update()
    {
        bool isChasing = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);

        if (isChasing && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && !isAttack && !isWaiting)
            {
                StartCoroutine(PrepareAndAttack());
            }
        }
    }

    private IEnumerator PrepareAndAttack()
    {
        isWaiting = true;

        // Tắt di chuyển của enemy
        if (enemyMove != null)
        {
            enemyMove.enabled = false;
        }
        Debug.Log("Đang vận sức!");

        yield return new WaitForSeconds(1f);

        // Kiểm tra lại xem player còn trong phạm vi tấn công không
        bool isPlayerInRange = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);

        if (isPlayerInRange)
        {
            StartCoroutine(Attacking());
        }
        else
        {
            Debug.Log("Player không còn trong phạm vi tấn công.");
        }
        if (enemyMove != null)
        {
            enemyMove.enabled = true;
        }

        isWaiting = false;
    }

    private IEnumerator Attacking()
    {
        isAttack = true;

        Debug.Log("Đã tấn công");

        if (playerMovement != null)
        {
            playerMovement.TakeDamage(10);
        }
        else
        {
            Debug.LogError("Không thể tấn công vì PlayerMovement không hợp lệ.");
        }

        yield return new WaitForSeconds(attackCooldown);

        isAttack = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
