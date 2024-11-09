using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [Header("Attack Settings")]
    public float radiusAttack = 2f;
    public float angleAttack = 90f;
    public int attackSegments = 20;

    [Header("Cooldown and Damage Settings")]
    public float damage = 10f;
    public float damageBoss = 5f;
    private int comboCount = 0;
    public float comboCooldown = 1f;
    public float comboResetTime = 2f;

    [Header("Other")]
    public GameObject PopupDamage;

    private bool isCooldown = false;
    private float lastAttackTime = 0f;

    public float reducedSpeed = 2f;
    private float originalSpeed;

    private LadderMovement ladder;
    private PlayerMovement playerMovement;
    private Stamina stamina;

    public float staminaCostPerAttack = 5f;
    private void Start()
    {
        ladder = GetComponent<LadderMovement>();
        playerMovement = GetComponent<PlayerMovement>();
        stamina = GetComponent<Stamina>();
        originalSpeed = playerMovement.speed;

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !ladder.isClimbing && !playerMovement.isSwinging && playerMovement.CanAttack())
        {
            // Check if the attack is not on cooldown and has enough stamina
            if (!isCooldown && stamina.CurrentStamina > staminaCostPerAttack)
            {
                stamina.DecreaseStamina(staminaCostPerAttack);
                StartCoroutine(AttackRoutine());
                Debug.Log($"Attack {comboCount + 1}");

                comboCount++;
                lastAttackTime = Time.time;

                if (comboCount >= 3)
                {
                    StartCoroutine(ComboCooldownRoutine());
                }
            }
            else
            {
                Debug.Log("Not enough stamina to attack!");
            }
        }

        // Reset combo count and animation if reset time has passed
        if (Time.time - lastAttackTime > comboResetTime && comboCount > 0)
        {
            comboCount = 0;
            animator.SetBool("isIdle", true);  // Return to idle state
            animator.SetInteger("attackCombo", 0);  // Reset attack combo state
            Debug.Log("Combo attack reset");
        }
    }

    // giảm speed khi vừa tấn công vừa di chuyển
    private IEnumerator AttackRoutine()
    {
        // Giảm tốc độ khi tấn công
        playerMovement.speed = reducedSpeed;

        // Gọi hàm tấn công, thực hiện logic tấn công
        PlayerAttack();

        // Thời gian giảm tốc độ khi tấn công (có thể thay đổi dựa trên combo)
        float reducedSpeedDuration = 0.1f;

        // Nếu comboCount tăng, có thể giảm thời gian di chuyển chậm hơn
        switch (comboCount)
        {
            case 0:
                reducedSpeedDuration = 0.1f;
                break;
            case 1:
                reducedSpeedDuration = 0.15f; // Combo 1, thời gian giảm tốc độ lâu hơn chút
                break;
            case 2:
                reducedSpeedDuration = 0.2f;  // Combo 2, giảm tốc độ lâu nhất
                break;
        }

        // Giữ tốc độ giảm trong khoảng thời gian nhất định
        yield return new WaitForSeconds(reducedSpeedDuration);

        // Khôi phục tốc độ ban đầu
        playerMovement.speed = originalSpeed;
    }

    private void PlayerAttack()
    {
        float damageRandom1 = Random.Range(5f, 7f);
        float damageRandom2 = Random.Range(6f, 8f);
        float damageRandom3 = Random.Range(8f, 12f);

        int damageShield = Random.Range(25,30);

        Vector3 attackDirection = transform.localScale.x < 0 ? -transform.right : transform.right;

        float halfAngle = angleAttack / 2f;
        float angleStep = angleAttack / attackSegments;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radiusAttack, LayerMask.GetMask("Enemy"));

        foreach (var enemy in enemies)
        {
            Vector2 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            float angleToEnemy = Vector2.Angle(attackDirection, directionToEnemy);

            Debug.Log($"Attacking enemy: {enemy.name}, Angle: {angleToEnemy}");

            if (angleToEnemy <= halfAngle)
            {

                // enemy normal
                HealthbarEnemy enemyHealth = enemy.GetComponent<HealthbarEnemy>();
                if (enemyHealth != null)
                {
                    Vector3 popupPosition = enemyHealth.transform.position;
                    Vector2 knockbackDirection = directionToEnemy * 1;
                    if (comboCount == 0)
                    {
                        ShowDamage(Mathf.Round(damageRandom1 * 10).ToString(), popupPosition);
                        enemyHealth.TakeDamage(Mathf.Round(damageRandom1), knockbackDirection);
                        Debug.Log("Đòn 1 với sát thương: " + damageRandom1);
                    }
                    else if (comboCount == 1)
                    {
                        ShowDamage(Mathf.Round(damageRandom2 * 10).ToString(), popupPosition);
                        enemyHealth.TakeDamage(Mathf.Round(damageRandom2), knockbackDirection);
                        Debug.Log("Đòn 2 với sát thương: " + damageRandom2);
                    }
                    else if (comboCount == 2)
                    {
                        ShowDamage(Mathf.Round(damageRandom3 * 10).ToString(), popupPosition);
                        enemyHealth.TakeDamage(Mathf.Round(damageRandom3), knockbackDirection);
                        Debug.Log("Đòn 3 với sát thương: " + damageRandom3);
                    }
                }

                // snake
                HealthBarBoss bossHealth = enemy.GetComponent<HealthBarBoss>();
                if (bossHealth != null)
                {
                    Vector3 popupPosition = bossHealth.transform.position;
                    if (comboCount == 0)
                    {
                        ShowDamage(Mathf.Round(damageRandom1 * 10).ToString(), popupPosition);
                        bossHealth.TakeDamage(Mathf.Round(damageRandom1));
                        Debug.Log("Đòn 1 với sát thương: " + damageRandom1);
                    }
                    else if (comboCount == 1)
                    {
                        ShowDamage(Mathf.Round(damageRandom2 * 10).ToString(), popupPosition);
                        bossHealth.TakeDamage(Mathf.Round(damageRandom2));
                        Debug.Log("Đòn 2 với sát thương: " + damageRandom2);
                    }
                    else if (comboCount == 2)
                    {
                        ShowDamage(Mathf.Round(damageRandom3 * 10).ToString(), popupPosition);
                        bossHealth.TakeDamage(Mathf.Round(damageRandom3));
                        Debug.Log("Đòn 3 với sát thương: " + damageRandom3);
                    }
                }

                // ghost tree
                GhostTreeHealth ghostTreeHealth = enemy.GetComponent<GhostTreeHealth>();
                if (ghostTreeHealth != null)
                {
                    Vector3 popupPosition = ghostTreeHealth.transform.position;
                    Debug.Log("Ghost Tree detected and attacked");
                    if (comboCount == 0)
                    {
                        ShowDamage(Mathf.Round(damageRandom1 * 10).ToString(), popupPosition);
                        ghostTreeHealth.TakeDamage(Mathf.Round(damageRandom1));
                        Debug.Log("Đòn 1 với sát thương: " + damageRandom1);
                    }
                    else if (comboCount == 1)
                    {
                        ShowDamage(Mathf.Round(damageRandom2 * 10).ToString(), popupPosition);
                        ghostTreeHealth.TakeDamage(Mathf.Round(damageRandom2));
                        Debug.Log("Đòn 2 với sát thương: " + damageRandom2);
                    }
                    else if (comboCount == 2)
                    {
                        ShowDamage(Mathf.Round(damageRandom3 * 10).ToString(), popupPosition);
                        ghostTreeHealth.TakeDamage(Mathf.Round(damageRandom3));
                        Debug.Log("Đòn 3 với sát thương: " + damageRandom3);
                    }
                }

                // eagle
                EagleHealthbar eagleBoss = enemy.GetComponent<EagleHealthbar>();
                if (eagleBoss != null)
                {
                    Vector3 popupPosition = eagleBoss.transform.position;
                    Debug.Log("Eagle da bi tan cong");
                    if (comboCount == 0)
                    {
                        ShowDamage(Mathf.Round(damageRandom1 * 10).ToString(), popupPosition);
                        eagleBoss.TakeDamage(damageShield, Mathf.Round(damageRandom1));
                        Debug.Log("Đòn 1 với sát thương: " + damageRandom1);
                    }
                    else if (comboCount == 1)
                    {
                        ShowDamage(Mathf.Round(damageRandom2 * 10).ToString(), popupPosition);
                        eagleBoss.TakeDamage(damageShield, Mathf.Round(damageRandom2));
                        Debug.Log("Đòn 2 với sát thương: " + damageRandom2);
                    }
                    else if (comboCount == 2)
                    {
                        ShowDamage(Mathf.Round(damageRandom3 * 10).ToString(), popupPosition);
                        eagleBoss.TakeDamage(damageShield, Mathf.Round(damageRandom3));
                        Debug.Log("Đòn 3 với sát thương: " + damageRandom3);
                    }
                }

                // toad
                ToadHealth toadhealth = enemy.GetComponent<ToadHealth>();
                if (toadhealth != null)
                {
                    Vector3 popupPosition = toadhealth.transform.position;
                    if (comboCount == 0)
                    {
                        ShowDamage(Mathf.Round(damageRandom1 * 10).ToString(), popupPosition);
                        toadhealth.TakeDamage(Mathf.Round(damageRandom1));
                        Debug.Log("Đòn 1 với sát thương: " + damageRandom1);
                    }
                    else if (comboCount == 1)
                    {
                        ShowDamage(Mathf.Round(damageRandom2 * 10).ToString(), popupPosition);
                        toadhealth.TakeDamage(Mathf.Round(damageRandom2));
                        Debug.Log("Đòn 2 với sát thương: " + damageRandom2);
                    }
                    else if (comboCount == 2)
                    {
                        ShowDamage(Mathf.Round(damageRandom3 * 10).ToString(), popupPosition);
                        toadhealth.TakeDamage(Mathf.Round(damageRandom3));
                        Debug.Log("Đòn 3 với sát thương: " + damageRandom3);
                    }
                }

                // golem
                GolemHealthbar golemHealth = enemy.GetComponent<GolemHealthbar>();
                if (golemHealth != null)
                {
                    Vector3 popupPosition = golemHealth.transform.position;
                    if (comboCount == 0)
                    {
                        ShowDamage(Mathf.Round(damageRandom1 * 10).ToString(), popupPosition);
                        golemHealth.TakeDamage(damageShield, Mathf.Round(damageRandom1));
                        Debug.Log("Đòn 1 với sát thương: " + damageRandom1);
                    }
                    else if (comboCount == 1)
                    {
                        ShowDamage(Mathf.Round(damageRandom2 * 10).ToString(), popupPosition);
                        golemHealth.TakeDamage(damageShield, Mathf.Round(damageRandom2));
                        Debug.Log("Đòn 2 với sát thương: " + damageRandom2);
                    }
                    else if (comboCount == 2)
                    {
                        ShowDamage(Mathf.Round(damageRandom3 * 10).ToString(), popupPosition);
                        golemHealth.TakeDamage(damageShield, Mathf.Round(damageRandom3));
                        Debug.Log("Đòn 3 với sát thương: " + damageRandom3);
                    }
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
    public void ShowDamage(string text, Vector3 position, int multiplier = 1)
    {
        if (PopupDamage)
        {
            GameObject popupDamage = Instantiate(PopupDamage, position, Quaternion.identity);
            TMP_Text damageText = popupDamage.GetComponentInChildren<TMP_Text>();

            Color randomColor = Random.value > 0.5f ? new Color(1f, 0f, 0f, 132f / 255f) : new Color(1f, 1f, 1f, 132f / 255f);

            damageText.color = randomColor;

            damageText.text = (int.Parse(text) * multiplier).ToString();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        float halfAngle = angleAttack / 2f;

        Vector3 forward = transform.localScale.x < 0 ? -transform.right : transform.right;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -halfAngle) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, halfAngle) * forward;

        // Vẽ phạm vi tấn công
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
