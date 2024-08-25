using System.Collections;
using UnityEngine;

public class Boar : MonoBehaviour
{
    public float detectionRadius = 5f;
    public float chargeSpeed = 5f;
    public float chargePreparationTime = 1.5f;
    public float chargeDuration = 1.5f;
    public float chargeCooldown = 3f;
    public float stationaryTime = 1f;
    public float disableTime = 1.5f; 
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    private Vector2 chargeDirection;
    private bool isCharging = false;
    private bool canCharge = true;
    private EnemyMove enemyMove;
    private bool isFacingRight = true;

    private PlayerMovement playerMovement;
    private bool isScriptEnabled = true; 

    private void Start()
    {
        enemyMove = GetComponent<EnemyMove>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (canCharge && !isCharging)
        {
            // Sử dụng OverlapCircle để phát hiện Player trong phạm vi radius
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerMask);

            if (playerCollider != null && playerCollider.CompareTag("Player"))
            {
                Vector2 playerPosition = playerCollider.transform.position;
                chargeDirection = new Vector2(playerPosition.x - transform.position.x, 0).normalized;

                // Kiểm tra nếu Player nằm sau lưng Boar
                if (Vector2.Dot(chargeDirection, Vector2.right * (isFacingRight ? 1 : -1)) < 0)
                {
                    // Quay mặt về phía Player
                    isFacingRight = !isFacingRight;
                    transform.localScale = new Vector3(isFacingRight ? -1 : 1, 1, 1);
                }

                if (enemyMove != null)
                {
                    enemyMove.enabled = false;
                }

                StartCoroutine(ChargeTowardsTarget(playerCollider.GetComponent<PlayerMovement>()));
            }
        }
    }

    private IEnumerator ChargeTowardsTarget(PlayerMovement playerMovement)
    {
        isCharging = true;
        canCharge = false;

        // Thời gian vận sức trước khi bắt đầu tông
        yield return new WaitForSeconds(chargePreparationTime);

        float chargeEndTime = Time.time + chargeDuration;

        while (Time.time < chargeEndTime)
        {
            Vector2 newPosition = (Vector2)transform.position + chargeDirection * chargeSpeed * Time.deltaTime;
            transform.position = newPosition;

            Collider2D hitCollider = Physics2D.OverlapCircle(newPosition, 0.1f, playerMask | obstacleMask);

            if (hitCollider != null)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    // Tông trúng Player
                    if (playerMovement != null)
                    {
                        playerMovement.TakeDamage(10, 1.5f, 0.65f, 0.1f);
                    }

                    isCharging = false;
                    transform.position = new Vector2(hitCollider.transform.position.x, transform.position.y);
                    yield return new WaitForSeconds(stationaryTime);

                    if (enemyMove != null)
                    {
                        enemyMove.enabled = true;
                    }

                    yield return new WaitForSeconds(chargeCooldown);
                    canCharge = true;
                    yield break;
                }
                else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Turn") || hitCollider.CompareTag("Turn"))
                {
                    // Tông trúng đối tượng cản trở
                    isCharging = false;
                    transform.position = new Vector2(hitCollider.transform.position.x, transform.position.y);

                    StartCoroutine(DisableBoarScript());

                    yield return new WaitForSeconds(stationaryTime);

                    if (enemyMove != null)
                    {
                        enemyMove.enabled = true;
                    }

                    yield return new WaitForSeconds(chargeCooldown);
                    canCharge = true;
                    yield break;
                }
            }

            yield return null;
        }

        isCharging = false;

        yield return new WaitForSeconds(stationaryTime);

        if (enemyMove != null)
        {
            enemyMove.enabled = true;
        }

        yield return new WaitForSeconds(chargeCooldown);
        canCharge = true;
    }

    private IEnumerator DisableBoarScript()
    {
        isScriptEnabled = false;
        this.enabled = false; 
        yield return new WaitForSeconds(disableTime);
        this.enabled = true; 
        isScriptEnabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
