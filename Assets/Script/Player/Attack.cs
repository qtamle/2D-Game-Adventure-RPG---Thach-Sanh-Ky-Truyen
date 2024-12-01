using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Attack : MonoBehaviour
{
    
    [SerializeField] public AnyStateAnimator animator;
    public AnimationManager animationManager;
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

    [Header("Combo Timing")]
    public float timeBetweenCombos = 1f; 
    private float lastComboTime = 0f;

    private bool isCooldown = false;
    private float lastAttackTime = 0f;
    private bool isAttack;
    private bool canAttackAgain = true;

    public float reducedSpeed = 2f;
    private float originalSpeed;

    private LadderMovement ladder;
    private PlayerMovement playerMovement;
    private Stamina stamina;

    public float staminaCostPerAttack = 5f;

    private void Start()
    {
        if (animationManager == null)
        {
            animationManager = GetComponent<AnimationManager>();
        }
        ladder = GetComponent<LadderMovement>();
        playerMovement = GetComponent<PlayerMovement>();
        stamina = GetComponent<Stamina>();
        originalSpeed = playerMovement.speed;
        animator.SetInteger("attackCombo", comboCount);

    }
    private void Update()
    {
        if (PauseGame.isGamePaused) return;

        if (Input.GetMouseButtonDown(0) && !ladder.isClimbing && !playerMovement.isSwinging && playerMovement.CanAttack() && !isAttack && canAttackAgain)
        {
            if (Time.time - lastComboTime >= timeBetweenCombos)
            {
                if (!isCooldown && stamina.CurrentStamina > staminaCostPerAttack)
                {
                    stamina.DecreaseStamina(staminaCostPerAttack);
                    StartCoroutine(AttackRoutine());

                    comboCount++;
                    lastAttackTime = Time.time;

                    PlayAttackAnimation();

                    lastComboTime = Time.time; 

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
            else
            {
                Debug.Log("Too soon to perform another combo!");
            }
        }

        if (Time.time - lastAttackTime > comboResetTime && comboCount > 0)
        {
            comboCount = 0;
            animator.SetInteger("attackCombo", comboCount);
            animator.ResetTrigger("Attack");
            Debug.Log("Combo attack reset");
        }
    }

    private void PlayAttackAnimation()
    {

        bool isRunning = playerMovement.horizontal != 0;
        animator.SetInteger("attackCombo", comboCount);
        string bodyAnimation = $"Body_Attack{comboCount % 3}";
        string legsAnimation = isRunning ? "Legs_Run" : $"Legs_Attack{comboCount % 3}";

        animator.TryPlayAnimation(bodyAnimation);
        animator.TryPlayAnimation(legsAnimation);

        animator.SetTrigger("Attack");
        // Sau khi hoàn thành combo 3 đòn, kiểm tra và đặt về trạng thái chạy hoặc đứng yên
        if (comboCount == 3)
        {
            StartCoroutine(EndComboRoutine());
        }
    }

    // Coroutine kết thúc combo và chuyển trạng thái về Idle hoặc Run nếu cần
    private IEnumerator EndComboRoutine()
    {
        bool isRunning = playerMovement.horizontal != 0;
        animator.ResetAnimations(isRunning);  // Gọi ResetAnimations với trạng thái di chuyển
        isAttack = false;

        canAttackAgain = false;
        yield return new WaitForSeconds(0.1f); 
        canAttackAgain = true;
    }



    // giảm speed khi vừa tấn công vừa di chuyển
    private IEnumerator AttackRoutine()
    {
        isAttack = true;
        playerMovement.isAttacking = true; // Đặt trạng thái đang tấn công
        // Giảm tốc độ khi tấn công
        playerMovement.speed = reducedSpeed;
        
        // Thời gian giảm tốc độ khi tấn công (có thể thay đổi dựa trên combo)
        float reducedSpeedDuration = 0.1f;

        PlayerAttack();

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
        playerMovement.isAttacking = false; // Kết thúc trạng thái tấn công
        isAttack = false;
    }

    private void PlayerAttack()
    {
        float damageRandom1 = Random.Range(10f, 14f);
        float damageRandom2 = Random.Range(15f, 18f);
        float damageRandom3 = Random.Range(19f, 22f);

        int damageShield = Random.Range(25,30);

        Vector3 attackDirection = transform.localScale.x < 0 ? -transform.right : transform.right;

        float halfAngle = angleAttack / 2f;
        float angleStep = angleAttack / attackSegments;

        LayerMask attackMask = LayerMask.GetMask("Enemy") | LayerMask.GetMask("Dart");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radiusAttack, attackMask);

        foreach (var enemy in enemies)
        {
            Vector2 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            float angleToEnemy = Vector2.Angle(attackDirection, directionToEnemy);

            Debug.Log($"Attacking enemy: {enemy.name}, Angle: {angleToEnemy}");

            if (angleToEnemy <= halfAngle)
            {

                DartDamage dart = enemy.GetComponent<DartDamage>();
                if (dart != null)
                {
                    dart.BounceOnHit();
                    GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

                    if (audioManagerObject != null)
                    {
                        AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

                        if (audioManager != null)
                        {
                            audioManager.PlaySFX(1);
                        }
                        else
                        {
                            Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
                        }
                    }
                    else
                    {
                        Debug.LogError("No GameObject found with the tag 'AudioManager'.");
                    }
                    Debug.Log("Dart bị tấn công và sẽ nảy lên!");
                }
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

                // ly thong phase 1
                HealthBarLT lt = enemy.GetComponent<HealthBarLT>();
                if (lt != null)
                {
                    Vector3 popupPosition = lt.transform.position;
                    if (comboCount == 0)
                    {
                        ShowDamage(Mathf.Round(damageRandom1 * 10).ToString(), popupPosition);
                        lt.TakeDamage(Mathf.Round(damageRandom1));
                        Debug.Log("Đòn 1 với sát thương: " + damageRandom1);
                    }
                    else if (comboCount == 1)
                    {
                        ShowDamage(Mathf.Round(damageRandom2 * 10).ToString(), popupPosition);
                        lt.TakeDamage(Mathf.Round(damageRandom2));
                        Debug.Log("Đòn 2 với sát thương: " + damageRandom2);
                    }
                    else if (comboCount == 2)
                    {
                        ShowDamage(Mathf.Round(damageRandom3 * 10).ToString(), popupPosition);
                        lt.TakeDamage(Mathf.Round(damageRandom3));
                        Debug.Log("Đòn 3 với sát thương: " + damageRandom3);
                    }
                }

                // ly thong phase 2
                Phase2Health ltPhase2 = enemy.GetComponent<Phase2Health>();
                if (ltPhase2 != null)
                {
                    Vector3 popupPosition = ltPhase2.transform.position;
                    if (comboCount == 0)
                    {
                        ShowDamage(Mathf.Round(damageRandom1 * 10).ToString(), popupPosition);
                        ltPhase2.TakeDamage(Mathf.Round(damageRandom1));
                        Debug.Log("Đòn 1 với sát thương: " + damageRandom1);
                    }
                    else if (comboCount == 1)
                    {
                        ShowDamage(Mathf.Round(damageRandom2 * 10).ToString(), popupPosition);
                        ltPhase2.TakeDamage(Mathf.Round(damageRandom2));
                        Debug.Log("Đòn 2 với sát thương: " + damageRandom2);
                    }
                    else if (comboCount == 2)
                    {
                        ShowDamage(Mathf.Round(damageRandom3 * 10).ToString(), popupPosition);
                        ltPhase2.TakeDamage(Mathf.Round(damageRandom3));
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
        animator.SetInteger("attackCombo", comboCount);
        animator.ResetTrigger("Attack");
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
