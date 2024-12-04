using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class LTSpear : MonoBehaviour
{
    [Header("Animator")]
    public Animator anim;

    [Header("Dash Attack")]
    public Transform player;
    public float dashSpeed = 10f;
    public float stopDistance = 1f;
    private Vector3 lastKnownPosition;
    private bool isDashing = false;

    [Header("Jump Attack")]
    public float jumpHeight;
    public float activationRadius;
    public Transform damageAreaTransform;
    public float damageRadius;
    public float stopDistanceJump;
    private bool isJumping;

    [Header("Gas Bomb Attack")]
    public GameObject gasBombPrefab;
    public float throwSpeed = 5f;
    public Transform throwPoint;

    [Header("Stab Spear")]
    public Transform attackTransform;
    public float attackRadius;
    public float damageAmount;

    [Header("Swipe Spear")]
    public float swipeRadius;
    public float swipeDuration = 1f;
    public Transform swipeTransform;

    [Header("Chase Player")]
    public float chaseSpeed = 5f;

    [Header("Check Ground")]
    public LayerMask groundLayer;
    public Transform groundCheck;

    [Header("Active Zone")]
    public float activationRadiusZone;
    private bool isChasing = true;
    private bool isUsingSkill = false;
    private Rigidbody2D rb;

    [Header("Camera Shake")]
    public ShakeData jumpShake;

    private HealthBarLT heathLT;

    private void Start()
    {
        heathLT = GetComponent<HealthBarLT>();
        rb = GetComponent<Rigidbody2D>();
        int myLayer = gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");

        Physics2D.IgnoreLayerCollision(myLayer, playerLayer, true);

        StartCoroutine(RandomSkillSelection());

        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlayBackgroundMusic(0);
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
    }
    private void Update()
    {
        if (!isDashing)
        {
            lastKnownPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        if (isChasing && !isUsingSkill)
        {
            FlipSprite();
            ChasePlayer();
        }
    }

    private IEnumerator RandomSkillSelection()
    {
        yield return new WaitForSeconds(2.5f);

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 3f));

            if (!CheckGround() || isUsingSkill) continue;

            FlipSprite();
            isChasing = false;
            isUsingSkill = true;

            int skill = Random.Range(0, 4);
            //int skill = 4;
            if (skill == 0)
            {
                StartDash();

                // sau khi dash thì chọn random 2 skill đâm và quạt
                int nextSkill = Random.Range(3, 4);  
                if (nextSkill == 3)
                {
                    Debug.Log("Xai skill đâm giáo");
                    if (Vector3.Distance(transform.position, player.position) <= activationRadiusZone)
                        StartCoroutine(StabSpear());
                }
                else if (nextSkill == 4)
                {
                    Debug.Log("Xai skill quạt giáo");
                    if (Vector3.Distance(transform.position, player.position) <= activationRadiusZone)
                        StartCoroutine(SwipeSpear());
                }
            }
            else if (skill == 1)
            {
                Debug.Log("nhảy lên và đập giáo");
                if (Vector3.Distance(transform.position, player.position) <= activationRadius)
                StartCoroutine(StartJumpAttack());
            }
            else if (skill == 2)
            {
                Debug.Log("ném ám khí");
                StartCoroutine(ThrowGasBomb());
            }
            else if (skill == 3)
            {
                Debug.Log("Xai skill đâm giáo");
                StartDash();
                yield return new WaitUntil(() => !isDashing);
                if (Vector3.Distance(transform.position, player.position) <= activationRadiusZone)
                    StartCoroutine(StabSpear());
            }
            else if (skill == 4)
            {
                Debug.Log("Xai skill quạt giáo");
                StartDash();
                yield return new WaitUntil(() => !isDashing);
                if (Vector3.Distance(transform.position, player.position) <= activationRadiusZone)
                    StartCoroutine(SwipeSpear());
            }
            else if (heathLT.health <= 0)
            {
                StopAllCoroutines();
            }
           
            yield return new WaitForSeconds(1f);
            isChasing = true;

        }
    }

    private bool CheckGround()
    {
        float checkRadius = 0.1f;
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        return hit != null;
    }

    private void FlipSprite()
    {
        // Nếu đối tượng đang ở bên trái player, flip sang bên phải, ngược lại flip sang bên trái
        if (player.position.x > transform.position.x && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (player.position.x < transform.position.x && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    // lướt tới
    public void StartDash()
    {
        isUsingSkill = true;
        
        lastKnownPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        isDashing = true;
        isChasing = false;
        StartCoroutine(DashToLastKnownPosition());
    }

    private IEnumerator DashToLastKnownPosition()
    {
        
        yield return new WaitForSeconds(1f);
        anim.SetTrigger("Dash");
        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(0);
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

        while (isDashing)
        {
            
            float distanceToTarget = Mathf.Abs(transform.position.x - lastKnownPosition.x);
            if (distanceToTarget <= stopDistance)
            {
                isDashing = false;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, lastKnownPosition, dashSpeed * Time.deltaTime);
            }

            yield return null;
        }
        yield return new WaitForSeconds(1.5f);

        isUsingSkill = false;
        isChasing = true;
    }

    // tấn công bằng nhảy
    public IEnumerator StartJumpAttack()
    {
        
        isUsingSkill = true;
        isJumping = true;
        lastKnownPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
       
        isChasing = false;
        
        yield return new WaitForSeconds(2f);
        anim.SetTrigger("JumpAttack");
        StartCoroutine(JumpAndDashDownward());
    }

    private IEnumerator JumpAndDashDownward()
    {
        
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = lastKnownPosition;

        float adjustedTargetX;
        if (player.position.x < transform.position.x)
        {
            adjustedTargetX = targetPosition.x + 10f;  
        }
        else
        {
            adjustedTargetX = targetPosition.x - 10f;  
        }

        float jumpDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            float t = elapsedTime / jumpDuration;

            float xPos = Mathf.Lerp(startPosition.x, adjustedTargetX, t);
            float yPos = Mathf.Lerp(startPosition.y, targetPosition.y, t) + jumpHeight * Mathf.Sin(Mathf.PI * t);

            transform.position = new Vector3(xPos, yPos, transform.position.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(adjustedTargetX, targetPosition.y, transform.position.z);

        CameraShakerHandler.Shake(jumpShake);

        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(2);
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

        Collider2D[] colliders = Physics2D.OverlapCircleAll(damageAreaTransform.position, damageRadius);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // Gây sát thương cho player
                Debug.Log("Đã va chạm với Player khi nhảy xuống và gây sát thương.");

                PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.TakeDamage(15f, 0.5f, 0.65f, 0.1f);  
                }
            }
        }

        yield return new WaitForSeconds(1.5f);

        isJumping = false;
        isUsingSkill = false;
        isChasing = true;
    }

    // ném ám khí
    private IEnumerator ThrowGasBomb()
    {
       
        isUsingSkill = true;
        isChasing = false;
        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(3);
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
        
        yield return new WaitForSeconds(1f);

        anim.SetTrigger("Dart");

        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < 1; i++)  
        {
            if (throwPoint != null)
            {
                GameObject gasBomb = Instantiate(gasBombPrefab, throwPoint.position, Quaternion.identity);

                Vector2 direction = new Vector2(Mathf.Sign(player.position.x - throwPoint.position.x), 0).normalized;
                gasBomb.GetComponent<Rigidbody2D>().velocity = direction * throwSpeed;

                if (audioManagerObject != null)
                {
                    AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

                    if (audioManager != null)
                    {
                        audioManager.PlaySFX(4);
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
            }

            // Chờ 1 giây trước khi ném quả tiếp theo
            yield return new WaitForSeconds(1f);

        }
        yield return new WaitForSeconds(0.5f);

        isUsingSkill = false;
        isChasing = true;
    }

    // đâm giáo liên tuc
    private IEnumerator StabSpear()
    {
        rb.isKinematic = true;
        isUsingSkill = true;
        isChasing = false;
        yield return new WaitForSeconds(1f);
        anim.SetTrigger("Stab");
        float attackTime = 0f;
        float stabDuration = 3f; 
        float damageInterval = 0.5f; 
        float lastDamageTime = 0f; 

        while (attackTime < stabDuration)
        {
            // Kiểm tra vùng tấn công tại attackTransform
            Collider2D[] colliders = Physics2D.OverlapCircleAll(attackTransform.position, attackRadius);

            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    // Kiểm tra xem có đủ thời gian để gây sát thương tiếp không
                    if (attackTime - lastDamageTime >= damageInterval)
                    {
                        Debug.Log("Đã va chạm với Player khi đâm giáo.");

                        PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                        if (playerMovement != null)
                        {
                            playerMovement.TakeDamage(damageAmount, 0f, 0f, 0f); 
                            lastDamageTime = attackTime; 
                        }
                    }
                }
            }
            attackTime += Time.deltaTime; // Tăng thời gian tấn công
            yield return null;
        }
        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(5);
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

        yield return new WaitForSeconds(1.5f);
        Debug.Log("Da hoan tat skill dam giao");
        isUsingSkill = false;
        isChasing = true;
        rb.isKinematic = false;
    }

    // quạt giáo
    private IEnumerator SwipeSpear()
    {
        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(6);
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
        anim.SetTrigger("Swipe");
        isUsingSkill = true;
        isChasing = false;
        yield return new WaitForSeconds(1f);
        
        // Kiểm tra nếu đối thủ trong vùng kích hoạt
        if (Vector3.Distance(transform.position, player.position) <= activationRadiusZone)
        {
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(swipeTransform.position, swipeRadius);

            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    Debug.Log("Đã va chạm với Player khi quạt giáo.");

                    PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                    if (playerMovement != null)
                    {
                        playerMovement.TakeDamage(30f, 0.5f, 0.65f, 0.1f);
                    }
                }
            }

            yield return new WaitForSeconds(swipeDuration);
        }
        yield return new WaitForSeconds(1.5f);

        Debug.Log("Da hoan tat skill quat giao");
        isUsingSkill = false;
        isChasing = true;
    }

    private void ChasePlayer()
    {
        
        float distanceToPlayerX = Mathf.Abs(transform.position.x - player.position.x);
        
        if (distanceToPlayerX > stopDistance)
        {
            anim.SetTrigger("Walk");
            // Di chuyển về phía player chỉ theo chiều X
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackTransform.position, attackRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, activationRadiusZone);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(swipeTransform.position, swipeRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(damageAreaTransform.position, damageRadius);
    }
}
