using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class ToadSkill : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce;
    public float jumpAngle = 45f;
    public LayerMask turnOnLayer;
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private float originalJumpForce;
    private bool isJumped = false;
    public Transform jumpAreaTransform;
    public float jumpRadius = 1f;
    private bool hasDamagedPlayer = false;
    private bool hasCamShaken = false;
    private bool isGrounded = false;

    [Header("Tongue Attack")]
    public GameObject tonguePrefab;
    public float tongueSpeed = 10f;
    public float maxTongueLength = 5f;
    public Transform tongueStartPosition;

    [Header("Bubble")]
    public GameObject bubblePrefab;
    public Transform shootBubble;
    public float bubbleSpeed;
    public float bubbleDestroy = 10f;
    public float bubbleUpwardAngle = 45f;
    public GameObject bubblePopPrefab;

    [Header("Catch Bug")]
    public float bugDetectionRadius = 10f;
    public LayerMask bugLayerMask;
    public Transform bugTongueStartPosition;
    public Transform radiusTransform;
    public float tongueShootSpeedBug;

    [Header("Skill Management")]
    public float minSkillDelay = 5f;
    public float maxSkillDelay = 8f;
    public float skillCooldown = 2f;

    [Header("Foot Impact")]
    public GameObject footImpactParticlePrefab;
    public Transform transformImpact;

    private GameObject tongue;
    private Vector3 tongueOriginalScale;
    private Vector3 tongueDirection;
    private ToadHealth healthToad;
    private PlayerMovement playerMovement;
    //private CameraShake cameraShake;

    [Header("Camera Shake")]
    public ShakeData jumpShake;
    void Start()
    {
        healthToad = GetComponent<ToadHealth>();
        playerMovement = GetComponent<PlayerMovement>();
        //cameraShake = GameObject.FindGameObjectWithTag("Shake").GetComponent<CameraShake>();

        rb = GetComponent<Rigidbody2D>();
        originalJumpForce = jumpForce;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Bug"), true);

        StartCoroutine(JumpAndUseSkillRoutine());
    }

    void Update()
    {
        if (isGrounded && !isJumped && !hasCamShaken)
        {
            //CameraShakerHandler.Shake(jumpShake);
            hasCamShaken = true;
        }
    }

    IEnumerator JumpAndUseSkillRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            int jumpTimes = Random.Range(6,11);

            for (int i = 0; i < jumpTimes; i++)
            {
                if (!isJumped)
                {
                    Jump(originalJumpForce); 
                }

                yield return new WaitForSeconds(1f); 
            }

            rb.velocity = Vector2.zero;
            isJumped = true; 
            yield return new WaitUntil(() => isGrounded);

            yield return new WaitForSeconds(2f);

            int randomSkill = Random.Range(0, 3);
            if (healthToad.health < 400f && randomSkill == 0)
            {
                CatchBugs();
            }
            else
            {
                if (randomSkill == 1)
                {
                    ShootTongue();
                }
                else if (randomSkill == 2)
                {
                    ShootBubble();
                }
            }

            yield return new WaitForSeconds(2f);

            isJumped = false;
        }
    }

    void Jump(float currentJumpForce)
    {
        isJumped = true;
        hasDamagedPlayer = false;
        hasCamShaken = false;

        float angleInRadians = jumpAngle * Mathf.Deg2Rad;

        Vector2 jumpVector = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)) * currentJumpForce;
        if (!isFacingRight)
        {
            jumpVector.x = -jumpVector.x;
        }

        rb.AddForce(jumpVector, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & turnOnLayer) != 0 && isJumped)
        {
            Flip();
            Jump(10f);
            isGrounded = false;
        }
        else if (collision.contacts[0].normal.y > 0.5f)
        {
            if (!hasCamShaken)
            {
                CameraShakerHandler.Shake(jumpShake);
                hasCamShaken = true;
            }

            if (footImpactParticlePrefab != null)
            { 
                GameObject particleEffect = Instantiate(footImpactParticlePrefab, transformImpact.position, Quaternion.Euler(0f,0f,90f));
                Destroy(particleEffect, 2f);
            }
            isGrounded = true;
            isJumped = false;

            if (!hasDamagedPlayer)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(jumpAreaTransform.position, jumpRadius, LayerMask.GetMask("Player"));
                foreach (Collider2D collider in colliders)
                {
                    if (collider.CompareTag("Player"))
                    {
                        PlayerMovement playerHealth = collider.GetComponent<PlayerMovement>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(20, 0f, 0f, 0f);
                            Debug.Log("Trúng player");
                            hasDamagedPlayer = true; 
                        }
                    }
                }
            }
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    GameObject FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        return player;
    }

    void ShootTongue()
    {
        StartCoroutine(ShootTongueRoutine());
    }

    IEnumerator ShootTongueRoutine()
    {
        GameObject player = FindPlayer();

        if (player == null)
        {
            Debug.LogWarning("Player not found for ShootTongue!");
            yield break; 
        }

        Vector3 playerPosition = player.transform.position;
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;

        if (directionToPlayer.x > 0 && !isFacingRight)
        {
            Flip(); 
        }
        else if (directionToPlayer.x < 0 && isFacingRight)
        {
            Flip(); 
        }

        yield return new WaitForSeconds(1f);

        if (tongue == null)
        {
            tongueDirection = isFacingRight ? Vector3.right : Vector3.left;

            tongue = Instantiate(tonguePrefab, tongueStartPosition.position, Quaternion.identity);
            tongueOriginalScale = tongue.transform.localScale;

            float angle = isFacingRight ? 0f : 180f;
            tongue.transform.rotation = Quaternion.Euler(0, 0, angle);

            StartCoroutine(ExtendTongue());
        }
    }

    IEnumerator ExtendTongue()
    {
        float currentLength = 0f;

        float originalYScale = tongueOriginalScale.y;
        float originalZScale = tongueOriginalScale.z;

        while (currentLength < maxTongueLength)
        {
            currentLength += tongueSpeed * Time.deltaTime;
            float scaleRatio = Mathf.Clamp(currentLength / maxTongueLength, 0f, 1f);
            tongue.transform.localScale = new Vector3(tongueOriginalScale.x * scaleRatio, originalYScale, originalZScale);

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        StartCoroutine(RetractTongue());
    }
    IEnumerator RetractTongue()
    {
        float currentLength = tongue.transform.localScale.x / tongueOriginalScale.x * maxTongueLength;

        float originalYScale = tongueOriginalScale.y;
        float originalZScale = tongueOriginalScale.z;

        while (currentLength > 0f)
        {
            currentLength -= tongueSpeed * Time.deltaTime;
            float scaleRatio = Mathf.Clamp(currentLength / maxTongueLength, 0f, 1f);

            tongue.transform.localScale = new Vector3(tongueOriginalScale.x * scaleRatio, originalYScale, originalZScale);

            yield return null;
        }

        Destroy(tongue);
    }
    void ShootBubble()
    {
        // Bắn viên bong bóng đầu tiên
        GameObject bubble1 = Instantiate(bubblePrefab, shootBubble.position, Quaternion.identity);
        Rigidbody2D bubbleRb1 = bubble1.GetComponent<Rigidbody2D>();
        Vector2 direction1 = isFacingRight ?
            new Vector2(1f, Mathf.Tan(bubbleUpwardAngle * Mathf.Deg2Rad)).normalized :
            new Vector2(-1f, Mathf.Tan(bubbleUpwardAngle * Mathf.Deg2Rad)).normalized;
        bubbleRb1.velocity = direction1 * bubbleSpeed;

        StartCoroutine(DestroyBubbleAfterTime(bubble1, bubbleDestroy));
        StartCoroutine(ShootBubbleWithDelay(1f));
    }

    IEnumerator ShootBubbleWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Bắn viên bong bóng thứ hai
        GameObject bubble2 = Instantiate(bubblePrefab, shootBubble.position, Quaternion.identity);
        Rigidbody2D bubbleRb2 = bubble2.GetComponent<Rigidbody2D>();
        Vector2 direction2 = isFacingRight ? Vector2.right : Vector2.left;
        bubbleRb2.velocity = direction2 * bubbleSpeed;

        StartCoroutine(DestroyBubbleAfterTime(bubble2, bubbleDestroy));
        StartCoroutine(ShootBubbleWithDelay3(1f));
    }

    IEnumerator ShootBubbleWithDelay3(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Bắn viên bong bóng thứ ba
        GameObject bubble3 = Instantiate(bubblePrefab, shootBubble.position, Quaternion.identity);
        Rigidbody2D bubbleRb3 = bubble3.GetComponent<Rigidbody2D>();
        Vector2 direction2 = isFacingRight ? Vector2.right : Vector2.left;
        bubbleRb3.velocity = direction2 * bubbleSpeed;

        StartCoroutine(DestroyBubbleAfterTime(bubble3, bubbleDestroy));
    }


    IEnumerator DestroyBubbleAfterTime(GameObject bubble, float time)
    {
        yield return new WaitForSeconds(time);
        if (bubble != null)
        {   
            Destroy(bubble);
            GameObject bubblePop = Instantiate(bubblePopPrefab, bubble.transform.position, Quaternion.identity);
            Destroy(bubblePop, 2f);
        }
    }
    void CatchBugs()
    {
        Vector2 boxSize = new Vector2(bugDetectionRadius * 2, bugDetectionRadius * 2); 

        Collider2D[] bugsInRange = Physics2D.OverlapBoxAll(radiusTransform.position, boxSize, 0f, bugLayerMask);

        if (bugsInRange.Length > 0)
        {
            Collider2D closestBug = bugsInRange[0];
            float closestDistance = Vector2.Distance(radiusTransform.position, closestBug.transform.position);

            for (int i = 1; i < bugsInRange.Length; i++)
            {
                float distance = Vector2.Distance(radiusTransform.position, bugsInRange[i].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBug = bugsInRange[i];
                }
            }
            if (closestBug != null)
            {
                StartCoroutine(ShootTongueAtBug(closestBug.transform));
            }
        }
    }


    IEnumerator ShootTongueAtBug(Transform bugTransform)
    {

        tongueDirection = (bugTransform.position - bugTongueStartPosition.position).normalized;

        tongue = Instantiate(tonguePrefab, bugTongueStartPosition.position, Quaternion.identity);
        tongueOriginalScale = tongue.transform.localScale;

        float angle = Mathf.Atan2(tongueDirection.y, tongueDirection.x) * Mathf.Rad2Deg;
        tongue.transform.rotation = Quaternion.Euler(0, 0, angle);

        float currentLength = 0f;
        float distanceToBug = Vector2.Distance(bugTongueStartPosition.position, bugTransform.position);

        // Bắn lưỡi đến bug
        while (currentLength < distanceToBug)
        {
            currentLength += tongueSpeed * tongueShootSpeedBug *Time.deltaTime;
            float scaleRatio = Mathf.Clamp(currentLength / maxTongueLength, 0f, 1f);
            tongue.transform.localScale = new Vector3(tongueOriginalScale.x * scaleRatio, tongueOriginalScale.y, tongueOriginalScale.z);

            yield return null;
        }

        bugTransform.SetParent(tongue.transform);
        bugTransform.position = tongue.transform.position;

        Rigidbody2D bugRb = bugTransform.GetComponent<Rigidbody2D>();
        if (bugRb != null)
        {
            bugRb.velocity = Vector2.zero;
            bugRb.isKinematic = true;
        }

        Bug bugMovement = bugTransform.GetComponent<Bug>();
        if (bugMovement != null)
        {
            bugMovement.enabled = false;
        }

        StartCoroutine(RetractTongueWithBug(bugTransform));
    }

    IEnumerator RetractTongueWithBug(Transform bugTransform)
    {
        float currentLength = tongue.transform.localScale.x / tongueOriginalScale.x * maxTongueLength;

        while (currentLength > 0f)
        {
            currentLength -= tongueSpeed * tongueShootSpeedBug *Time.deltaTime;
            float scaleRatio = Mathf.Clamp(currentLength / maxTongueLength, 0f, 1f);
            tongue.transform.localScale = new Vector3(tongueOriginalScale.x * scaleRatio, tongueOriginalScale.y, tongueOriginalScale.z);

            if (bugTransform != null)
            {
                bugTransform.position = tongue.transform.position;
            }

            yield return null;
        }

        if (bugTransform != null)
        {
            Destroy(bugTransform.gameObject);
        }
        Destroy(tongue);
        healthToad.UpHealth(100f);
    }

    private void OnDrawGizmosSelected()
    {
        if (radiusTransform != null)
        {
            Gizmos.color = Color.green;
            Vector2 boxSize = new Vector2(bugDetectionRadius * 2, bugDetectionRadius * 2);
            Gizmos.DrawWireCube(radiusTransform.position, boxSize);
        }

        if (jumpAreaTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(jumpAreaTransform.position, jumpRadius);
        }
    }

}
