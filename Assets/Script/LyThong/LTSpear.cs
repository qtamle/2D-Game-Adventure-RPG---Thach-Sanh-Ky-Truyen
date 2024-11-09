using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LTSpear : MonoBehaviour
{
    [Header("Dash Attack")]
    public Transform player;
    public float dashSpeed = 10f;
    public float stopDistance = 1f;
    private Vector3 lastKnownPosition;
    private bool isDashing = false;

    [Header("Jump Attack")]
    public float jumpHeight;
    public float activationRadius;
    private bool isJumping;

    [Header("Gas Bomb Attack")]
    public GameObject gasBombPrefab;
    public float throwSpeed = 5f;
    public Transform throwPoint;

    [Header("Check Ground")]
    public LayerMask groundLayer;
    public Transform groundCheck;

    private void Start()
    {
        int myLayer = gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");

        Physics2D.IgnoreLayerCollision(myLayer, playerLayer, true);
    }
    private void Update()
    {
        if (!isDashing)
        {
            lastKnownPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        }

        if (CheckGround())
        {
            if (Input.GetKeyDown(KeyCode.P) && !isDashing)
            {
                FlipSprite();  
                StartDash();
            }

            if (Input.GetKeyDown(KeyCode.O) && !isJumping && Vector3.Distance(transform.position, player.position) <= activationRadius)
            {
                FlipSprite();  
                StartCoroutine(StartJumpAttack());
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                FlipSprite();
                StartCoroutine(ThrowGasBomb());
            }
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
        if (player.position.x < transform.position.x && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (player.position.x > transform.position.x && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    public void StartDash()
    {
        lastKnownPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        isDashing = true;
        StartCoroutine(DashToLastKnownPosition());
    }

    private IEnumerator DashToLastKnownPosition()
    {
        yield return new WaitForSeconds(1f);

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
    }

    public IEnumerator StartJumpAttack()
    {
        isJumping = true;
        lastKnownPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
        yield return new WaitForSeconds(1f);
        StartCoroutine(JumpAndDashDownward());
    }

    private IEnumerator JumpAndDashDownward()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = lastKnownPosition;

        float jumpDuration = 1.2f;
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            float t = elapsedTime / jumpDuration;

            float xPos = Mathf.Lerp(startPosition.x, targetPosition.x, t);
            float yPos = Mathf.Lerp(startPosition.y, targetPosition.y, t) + jumpHeight * Mathf.Sin(Mathf.PI * t);

            transform.position = new Vector3(xPos, yPos, transform.position.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        isJumping = false;
    }

    private IEnumerator ThrowGasBomb()
    {
        yield return new WaitForSeconds(1f);
        if (throwPoint != null)
        {
            GameObject gasBomb = Instantiate(gasBombPrefab, throwPoint.position, Quaternion.identity);

            Vector2 direction = new Vector2(Mathf.Sign(player.position.x - throwPoint.position.x), 0).normalized;
            gasBomb.GetComponent<Rigidbody2D>().velocity = direction * throwSpeed;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}
