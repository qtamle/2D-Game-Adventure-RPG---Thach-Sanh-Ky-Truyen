using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool isUsingSkill = false;

    [Header("Tongue Attack")]
    public GameObject tonguePrefab;
    public float tongueSpeed = 10f;
    public float maxTongueLength = 5f;
    public Transform tongueStartPosition;

    [Header("Bubble")]
    public GameObject bubblePrefab;
    public float bubbleSpeed;
    public float bubbleDestroy = 10f;
    public float bubbleUpwardAngle = 45f;

    [Header("Catch Bug")]
    public float bugDetectionRadius = 10f;
    public LayerMask bugLayerMask;
    public Transform bugTongueStartPosition;

    [Header("Skill Management")]
    public float minSkillDelay = 5f;
    public float maxSkillDelay = 8f;
    public float skillCooldown = 2f;

    private GameObject tongue;
    private Vector3 tongueOriginalScale;
    private Vector3 tongueDirection;
    private ToadHealth healthToad;
    private PlayerMovement playerMovement;
    private CameraShake cameraShake;

    void Start()
    {
        healthToad = GetComponent<ToadHealth>();
        playerMovement = GetComponent<PlayerMovement>();
        cameraShake = GameObject.FindGameObjectWithTag("Shake").GetComponent<CameraShake>();

        rb = GetComponent<Rigidbody2D>();
        originalJumpForce = jumpForce;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Bug"), true);

    }

    void Update()
    {
        if (isGrounded && !isJumped && !isUsingSkill)
        {
            cameraShake.ToadJumpShake();
            isJumped = true;
        }

        if (isGrounded && !isJumped && !hasCamShaken)
        {
            cameraShake.ToadJumpShake();
            hasCamShaken = true;
        }
    }

    void Jump(float currentJumpForce)
    {
        isJumped = true;
        isUsingSkill = false;
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

    void ShootTongue()
    {
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
        GameObject bubble1 = Instantiate(bubblePrefab, transform.position, Quaternion.identity);
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
        GameObject bubble2 = Instantiate(bubblePrefab, transform.position, Quaternion.identity);
        Rigidbody2D bubbleRb2 = bubble2.GetComponent<Rigidbody2D>();
        Vector2 direction2 = isFacingRight ? Vector2.right : Vector2.left;
        bubbleRb2.velocity = direction2 * bubbleSpeed;

        StartCoroutine(DestroyBubbleAfterTime(bubble2, bubbleDestroy));
    }


    IEnumerator DestroyBubbleAfterTime(GameObject bubble, float time)
    {
        yield return new WaitForSeconds(time);
        if (bubble != null)
        {   
            Destroy(bubble);
        }
    }

    void CatchBugs()
    {
        Collider2D[] bugsInRange = Physics2D.OverlapCircleAll(transform.position, bugDetectionRadius, bugLayerMask);

        if (bugsInRange.Length > 0)
        {
            Collider2D firstBug = bugsInRange[0];

            if (firstBug != null)
            {
                StartCoroutine(ShootTongueAtBug(firstBug.transform));
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
            currentLength += tongueSpeed * Time.deltaTime;
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
            currentLength -= tongueSpeed * Time.deltaTime;
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
        healthToad.UpHealth(50);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, bugDetectionRadius);

        if (jumpAreaTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(jumpAreaTransform.position, jumpRadius);
        }
    }

}
