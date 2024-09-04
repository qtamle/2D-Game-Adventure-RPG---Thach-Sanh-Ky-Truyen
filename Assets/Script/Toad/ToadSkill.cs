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

    private GameObject tongue;
    private Vector3 tongueOriginalScale;
    private Vector3 tongueDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalJumpForce = jumpForce;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumped)
        {
            Jump(originalJumpForce);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ShootTongue();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ShootBubble();
        }
    }

    void Jump(float currentJumpForce)
    {
        isJumped = true;

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
        }
        else if (collision.contacts[0].normal.y > 0.5f)
        {
            isJumped = false;
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

        while (currentLength < maxTongueLength)
        {
            currentLength += tongueSpeed * Time.deltaTime;
            float scaleRatio = Mathf.Clamp(currentLength / maxTongueLength, 0f, 1f);
            tongue.transform.localScale = new Vector3(tongueOriginalScale.x * scaleRatio, tongueOriginalScale.y, tongueOriginalScale.z);

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        StartCoroutine(RetractTongue());
    }

    IEnumerator RetractTongue()
    {
        float currentLength = tongue.transform.localScale.x / tongueOriginalScale.x * maxTongueLength;

        while (currentLength > 0f)
        {
            currentLength -= tongueSpeed * Time.deltaTime;
            float scaleRatio = Mathf.Clamp(currentLength / maxTongueLength, 0f, 1f);
            tongue.transform.localScale = new Vector3(tongueOriginalScale.x * scaleRatio, tongueOriginalScale.y, tongueOriginalScale.z);

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
}
