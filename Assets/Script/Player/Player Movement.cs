using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move and Jump")]
    public float horizontal;
    public float speed = 5f;
    public float jumpPower = 10f;
    public bool isFacingRight = true;

    [Header("Dash")]
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 20f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    [Header("Wall Slide and Wall Jump")]
    private bool isWallSliding;
    private float wallSlidingSpeed = 1f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [Header("Rope Swing")]
    public bool isSwinging = false;
    private HingeJoint2D ropeHingeJoint;
    private GameObject rope;
    public float swingForce = 200f;

    [Header("Check")]
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private void Update()
    {
        if (isDashing || isSwinging)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isSwinging)
            {
                JumpOffRope();
            }
            if (isSwinging && Input.GetKeyDown(KeyCode.Q))
            {
                Flip();
            }
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.W) && IsGrounded())
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpPower);
        }

        if (Input.GetKeyUp(KeyCode.W) && rb2d.velocity.y > 0f)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.L) && canDash && !isWallSliding && IsGrounded())
        {
            StartCoroutine(Dash());
        }

        WallSlide();
        WallJump();

        if (!isWallJumping && !isSwinging)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (!isWallJumping && !isDashing && !isSwinging)
        {
            rb2d.velocity = new Vector2(horizontal * speed, rb2d.velocity.y);
        }
        else if (isSwinging)
        {
            float swingInput = Input.GetAxis("Horizontal");
            float swingForceAdjusted = swingForce * Time.fixedDeltaTime;
            rb2d.AddForce(new Vector2(swingInput * swingForceAdjusted, 0), ForceMode2D.Force);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isSwinging)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
        else if (horizontal != 0)
        {
            // Flip hướng khi di chuyển
            if (horizontal > 0 && !isFacingRight || horizontal < 0 && isFacingRight)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1;
                transform.localScale = localScale;
            }
        }
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (isDashing || isSwinging) return;

        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Clamp(rb2d.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }

        if (IsGrounded())
        {
            rb2d.velocity = new Vector2(horizontal * speed, rb2d.velocity.y);
            Flip();
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = true;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.W) && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb2d.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1;
                transform.localScale = localScale;
            }
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb2d.gravityScale;
        rb2d.gravityScale = 0f;
        rb2d.velocity = new Vector2((isFacingRight ? 1 : -1) * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb2d.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope") && !isSwinging)
        {
            AttachToRope(collision.gameObject);
        }
    }

    private void AttachToRope(GameObject ropeObject)
    {
        isSwinging = true;
        rb2d.velocity = Vector2.zero;
        rb2d.gravityScale = 0;
        rb2d.angularDrag = 0.1f;
        rb2d.inertia = 1f;

        ropeHingeJoint = gameObject.AddComponent<HingeJoint2D>();
        ropeHingeJoint.connectedBody = ropeObject.GetComponent<Rigidbody2D>();
        ropeHingeJoint.autoConfigureConnectedAnchor = false;
        ropeHingeJoint.anchor = Vector2.zero;
        ropeHingeJoint.connectedAnchor = new Vector2(0f, -0.5f);

        rope = ropeObject;
    }

    private void JumpOffRope()
    {
        DetachFromRope();

        // Tăng giá trị để tăng lực đẩy theo chiều ngang
        float horizontalJumpForce = 50f;
        float verticalJumpForce = 20f;

        // Tính toán lực đẩy theo hướng nhảy
        Vector2 jumpDirection = new Vector2(transform.localScale.x, 1).normalized;
        rb2d.velocity = new Vector2(jumpDirection.x * horizontalJumpForce, verticalJumpForce);

        rb2d.velocity += new Vector2(transform.localScale.x * 15, 0);
    }

    private void DetachFromRope()
    {
        isSwinging = false;
        rb2d.gravityScale = 1;

        if (rope != null)
        {
            RopeController ropeController = rope.GetComponent<RopeController>();
            if (ropeController != null)
            {
                ropeController.DisableColliderTemporarily(2f);

                StartCoroutine(ResetRopeAfterDelay(3f, ropeController));
            }
        }

        Destroy(ropeHingeJoint);
        ropeHingeJoint = null;
        rope = null;
    }

    private IEnumerator ResetRopeAfterDelay(float delay, RopeController ropeController)
    {
        yield return new WaitForSeconds(delay);
        ropeController.ResetRope();
    }
}
