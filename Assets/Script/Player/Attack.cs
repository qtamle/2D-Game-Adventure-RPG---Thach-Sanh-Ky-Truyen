using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float radiusAttack = 2f;
    public float angleAttack = 90f;
    public int attackSegments = 20;
    public float damage = 10f;
    private int comboCount = 0;
    public float comboCooldown = 1f;
    private bool isCooldown = false;
    private float lastAttackTime = 0f;
    public float comboResetTime = 2f;

    public float reducedSpeed = 2f;
    private float originalSpeed;

    private LadderMovement ladder;
    private PlayerMovement playerMovement;

    private void Start()
    {
        ladder = GetComponent<LadderMovement>();
        playerMovement = GetComponent<PlayerMovement>();

        originalSpeed = playerMovement.speed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && !ladder.isClimbing && !playerMovement.isSwinging && playerMovement.CanAttack())
        {
            if (!isCooldown)
            {
                StartCoroutine(AttackRoutine());
                Debug.Log($"Attack {comboCount + 1}");
                comboCount++;
                lastAttackTime = Time.time;

                if (comboCount >= 4)
                {
                    StartCoroutine(ComboCooldownRoutine());
                }
            }
        }

        if (Time.time - lastAttackTime > comboResetTime && comboCount > 0)
        {
            comboCount = 0;
            Debug.Log("Combo attack reset");
        }
    }

    // giảm speed khi vừa tấn công vừa di chuyển
    private IEnumerator AttackRoutine()
    {
        playerMovement.speed = reducedSpeed;
        PlayerAttack();
        yield return new WaitForSeconds(0.5f);
        playerMovement.speed = originalSpeed;
    }

    private void PlayerAttack()
    {
        Vector3 attackDirection = transform.localScale.x > 0 ? transform.right : -transform.right;

        float halfAngle = angleAttack / 2f;
        float angleStep = angleAttack / attackSegments;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radiusAttack, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            Vector2 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            float angleToEnemy = Vector2.Angle(attackDirection, directionToEnemy);

            if (angleToEnemy <= halfAngle)
            {
                HealthbarEnemy enemyHealth = enemy.GetComponent<HealthbarEnemy>();
                if (enemyHealth != null)
                {
                    Vector2 knockbackDirection = directionToEnemy * 1;
                    enemyHealth.TakeDamage(damage, knockbackDirection);
                }

                HealthBarBoss bossHealth = enemy.GetComponent<HealthBarBoss>();
                if (bossHealth != null)
                {
                    bossHealth.TakeDamage(5);
                }
            }
        }
    }

    private IEnumerator ComboCooldownRoutine()
    {
        isCooldown = true;
        Debug.Log("Combo attack complete");
        yield return new WaitForSeconds(comboCooldown);
        comboCount = 0;
        isCooldown = false;
        Debug.Log("Cooldown end");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        float halfAngle = angleAttack / 2f;

        Vector3 forward = transform.localScale.x > 0 ? transform.right : -transform.right;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -halfAngle) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, halfAngle) * forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * radiusAttack);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * radiusAttack);

        float angleStep = angleAttack / attackSegments;
        for (int i = 0; i <= attackSegments; i++)
        {
            float angle = -halfAngle + i * angleStep;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * forward;
            Gizmos.DrawLine(transform.position + dir * radiusAttack, transform.position);
        }
    }
}
