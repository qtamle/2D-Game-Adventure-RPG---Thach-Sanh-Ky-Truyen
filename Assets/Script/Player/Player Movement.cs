using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move and Jump")]
    public float horizontal;
    public float speed = 6f;
    public float jumpPower = 10f;
    public bool isFacingRight = false;
    private float staminaJump = 5f;

    [Header("Dash")]
    private bool isDashing;
    public float dashingPower = 20f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    public float staminaDecreaseAmount = 10f;
    private bool canDash = true;

    [Header("Wall Slide and Wall Jump")]
    private bool isWallSliding;
    private float wallJumpDirection;
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

    [Header("Animator")]
    private AnyStateAnimator animator;
    public AnimationManager animationManager;

    [Header("Check")]
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask ledgeLayer;
    [SerializeField] private ParticleSystem dustPrefab;
    [SerializeField] private DamageFlash damageFlash;

    [Header("Health")]
    public HealthBar healthBar;

    [Header("Attack")]
    [SerializeField] private Attack attackScript;

    [HideInInspector] public bool ledgeDetected;
    [HideInInspector] public bool isStunned = false;

    private LadderMovement ladderMovement;
    private Grappler grappler;
    private LedgeClimb ledgeClimb;
    private Stamina stamina;
    private StatusEffects statusEffects;
    private GolemSkill golemSkill;

    [Header("KnockBack")]
    public Vector3 knockbackDirection = Vector3.left;
    public float knockbackY = 3f;

    [Header("Bow")]
    [SerializeField] private Bow bowScript;


    public bool isAttacking = false;
    private void Start()
    {
        if (animationManager == null)
        {
            animationManager = GetComponent<AnimationManager>();
        }

        if (bowScript == null)
        {
            bowScript = FindObjectOfType<Bow>(); 
        }

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Turn"), LayerMask.NameToLayer("Turn"), true);

        // Khởi tạo HealthBar
        if (healthBar != null)
        {
            healthBar.health = healthBar.maxHealth; 
        }

        // Get Component
        ladderMovement = GetComponent<LadderMovement>();
        grappler = GetComponent<Grappler>();
        ledgeClimb = GetComponent<LedgeClimb>();
        stamina = GetComponent<Stamina>();
        statusEffects = GetComponent<StatusEffects>();
        bowScript = GetComponent<Bow>();
        golemSkill = GetComponent<GolemSkill>();
    }
    private void Update()
    {
        if (isStunned) return;

        if (ladderMovement != null && ladderMovement.isClimbing)
        {
            return;
        }

        if (isDashing || isSwinging)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isSwinging)
            {
                JumpOffRope();
            }
            if (isSwinging && Input.GetKeyDown(KeyCode.Q))
            {
                Flip2();
            }
            return;
        }
        Move();
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && stamina.CurrentStamina > staminaJump)
        {
            dustPrefab.Play();
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpPower);
            stamina.DecreaseStamina(staminaJump);
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb2d.velocity.y > 0f && stamina.CurrentStamina > staminaJump)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isWallSliding && IsGrounded())
        {
            dustPrefab.Play();
            StartCoroutine(Dash());
        }

        WallSlide();
        WallJump();

        if (!isWallJumping && !isSwinging && !isDashing && horizontal != 0)
        {
            Flip();
        }

        if (!bowScript.isAiming && bowScript != null)
        {
            bool bowFacingRight = bowScript.GetPlayerFacingRight();
            if (isFacingRight != bowFacingRight)
            {
                Flip(); 
            }
        }

    }
    private void FixedUpdate()
    {
        if (isStunned) return;

        if (!isWallJumping && !isDashing && !isSwinging)
        {
            dustPrefab.Play();
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
    public void Flip()
    {
        if (isWallSliding || isWallJumping)
        {
            return;
        }

        if (horizontal != 0f && !isSwinging)
        {
            isFacingRight = horizontal > 0f;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(horizontal), transform.localScale.y, transform.localScale.z);
        }
    }
    private void Move()
    {
        if (isAttacking) return; // Bỏ qua hoạt động di chuyển nếu đang tấn công

        horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal != 0)
        {
            animationManager.animator.TryPlayAnimation("Body_Run");
            animationManager.animator.TryPlayAnimation("Legs_Run");
        }
        else
        {
            animationManager.animator.TryPlayAnimation("Body_Idle");
            animationManager.animator.TryPlayAnimation("Legs_Idle");
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

            wallJumpDirection = isFacingRight ? -1f : 1f;
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
            wallJumpingDirection = isFacingRight ? -1f : 1f; // Cập nhật hướng nhảy dựa vào hướng đang nhìn của nhân vật

            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f)
        {
            isWallJumping = true;

            // Kiểm tra và lật mặt nếu cần
            if (isFacingRight && wallJumpingDirection == 1f)
            {
                // Nếu đang nhìn phải và bật sang phải thì không cần flip
            }
            else if (!isFacingRight && wallJumpingDirection == -1f)
            {
                // Nếu đang nhìn trái và bật sang trái thì không cần flip
            }
            else
            {
                Flip2(); // Lật mặt khi cần thiết
            }

            // Thực hiện nhảy tường theo hướng đã xác định
            rb2d.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void Flip2()
    {
        isFacingRight = !isFacingRight; // Cập nhật lại trạng thái isFacingRight
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Lật mặt nhân vật
        transform.localScale = localScale;
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private IEnumerator Dash()
    {
        if (stamina.CurrentStamina >= staminaDecreaseAmount)
        {
            AudioManager.Instance.PlayPlayerSFX(2);

            stamina.DecreaseStamina(staminaDecreaseAmount);
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
        else
        {
            Debug.Log("Not enough stamina to dash!");
        }
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

        rb2d.velocity += new Vector2(transform.localScale.x * 10, 0);
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
    public void TakeDamage(float damage, float knockbackDistance, float knockbackSpeed, float knockbackDuration)
    {
        if (healthBar != null)
        {
            healthBar.TakeDamage(damage);
        }
        else
        {
            Debug.LogError("HealthBar chưa được gán.");
        }
        AudioManager.Instance.PlayPlayerSFX(1);

        Vector3 knockbackDirection = GetKnockbackDirection();
        StartCoroutine(ApplyKnockback(knockbackDirection, knockbackDistance, knockbackSpeed, knockbackDuration));
    }
    private Vector3 GetKnockbackDirection()
    {
        // Tìm tất cả enemy trong phạm vi
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f, LayerMask.GetMask("Enemy"));
        if (enemies.Length == 0)
        {
            // Không có enemy gần, trả về hướng đối diện với player
            return isFacingRight ? Vector3.left : Vector3.right;
        }

        // Tìm enemy gần nhất
        Collider2D closestEnemy = enemies[0];
        float closestDistance = Vector3.Distance(transform.position, closestEnemy.transform.position);

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        // Tính hướng knockback từ player đến enemy gần nhất
        Vector3 directionToEnemy = closestEnemy.transform.position - transform.position;

        // Nếu kẻ thù ở phía trước của player, knockback sẽ ngược lại với hướng mà player đang quay mặt
        if ((isFacingRight && directionToEnemy.x < 0) || (!isFacingRight && directionToEnemy.x > 0))
        {
            // Kẻ thù ở phía sau player, knockback về phía trước (theo hướng mà player đang quay mặt)
            return isFacingRight ? Vector3.right : Vector3.left;
        }
        else
        {
            return -directionToEnemy.normalized;
        }
    }
    private IEnumerator ApplyKnockback(Vector3 knockbackDirection, float knockbackDistance, float knockbackSpeed, float knockbackDuration)
    {
        float elapsedTime = 0f;
        Vector3 originalPosition = transform.position;

        // Thêm knockback theo trục Y vào phương thức
        Vector3 knockbackDirectionWithY = new Vector3(knockbackDirection.x, knockbackDirection.y + knockbackY, 0);

        while (elapsedTime < knockbackDuration)
        {
            transform.position = Vector3.Lerp(originalPosition, originalPosition + knockbackDirectionWithY * knockbackDistance, (elapsedTime / knockbackDuration));
            elapsedTime += Time.deltaTime * knockbackSpeed;
            yield return null;
        }

        transform.position = originalPosition + knockbackDirectionWithY * knockbackDistance;
    }

    public bool CanAttack()
    {
        return !isDashing && !isWallSliding && !isSwinging && !grappler.IsGrappling() && !ledgeClimb.IsClimbing() && !isStunned;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Turn"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
    public void SetStunned(bool stunned)
    {
        isStunned = stunned;
        if (isStunned)
        {
            rb2d.velocity = Vector2.zero;
        }
    }
    public Vector2 GetFacingDirection()
    {
        return isFacingRight ? Vector2.right : Vector2.left; 
    }

    public bool GetGroundCheck()
    {
        return IsGrounded();
    }
}
