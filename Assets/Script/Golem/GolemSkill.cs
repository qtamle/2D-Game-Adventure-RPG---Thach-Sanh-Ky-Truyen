using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using FirstGearGames.SmoothCameraShaker;
public class GolemSkill : MonoBehaviour
{
    [Header("Dash")]
    public float dashSpeed;
    public float dashDuration;
    private bool isDashing = false;
    private bool facingRight = true;
    private float dashTimeCounter;
    public float dashDurationCombo;
    public Transform radiusAttackTransform;
    public float detectionRadius;
    private bool isDamaged = false;
    private bool dashNormal;

    [Header("Throw Stone")]
    public GameObject rockPrefab;
    public float throwForce;
    public float throwDelay;

    [Header("Spikes")]
    public GameObject spikePrefab;
    public float spikeDelay;
    public float spikeDistance;
    public int maxSpikes = 5;
    public Transform spikeStartPoint;

    [Header("Jump Attack")]
    public float jumpForce;
    public float stompDelay;
    public float stomDuration;
    public float jumpUpDuration;
    public float radiusJumpAttack;
    public Transform JumpAttack;
    private bool isDamagedJumped = false;

    [Header("Falling Stone")]
    public Transform[] fallingStone;
    public GameObject fallStone;

    [Header("Other")]
    public LayerMask golemLayer;
    public LayerMask playerLayer;
    public GameObject rollingStone;
    public float throwForceStone;
    public Transform throwStoneRoll;
    public Transform radiusCheckforPlayer;
    public float playerRadius;
    public Transform radiusCheckBehindTransform; 
    public float radiusCheckBehind;

    private Rigidbody2D rb;
    private Transform player;
    private Vector2 lastPlayerPosition;
    private bool isPerformingSkill = false;
    private bool isSkillActived = false;
    private bool isStandingStill = false;
    private PlayerMovement playerMovement;

    [Header("Check Dash for Player")]
    public Transform radiusCheckTransform;
    public float radiusCheck;
    public LayerMask playerMask;
    public GameObject buttonPrefab; 
    private GameObject activeButton;
    public Transform buttonSpawnTransform;
    private bool isButtonActive = false;
    private bool hasSlid = false;
    private bool hasPressedZ = false;
    public GameObject afterImagePrefab;

    private CameraShake cam;

    [Header("Shake Camera")]
    public ShakeData dashShake;
    public ShakeData jumpShake;
    public ShakeData spikeShake;
    private void Start()
    {
        buttonPrefab.SetActive(false);

        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
        }

        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);
        //cam = GameObject.FindGameObjectWithTag("Shake").GetComponent<CameraShake>();

        StartCoroutine(RandomSkillRoutine());
    }
    IEnumerator RandomSkillRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            if (!isSkillActived && !isStandingStill)
            {
                int randomSkill = Random.Range(0, 100); 

                if (randomSkill < 35) // 35% cho DashCoroutine
                {
                    Debug.Log("tong bth");
                    yield return StartCoroutine(DashCoroutine());
                }
                else if (randomSkill < 60) // 25% cho GolemCombo (35% + 25%)
                {
                    Debug.Log("combo");
                    yield return StartCoroutine(GolemCombo());
                }
                else if (randomSkill < 80) // 20% cho SpawnSpikes (60% + 20%)
                {
                    Debug.Log("spike");
                    yield return StartCoroutine(SpawnSpikes());
                }
                else if (randomSkill < 90) // 10% cho JumpAndStomp (80% + 10%)
                {
                    Debug.Log("nhay dam");
                    yield return StartCoroutine(JumpAndStomp());
                }
                else // 10% cho ThrowRollingStone (90% + 10%)
                {
                    Debug.Log("nem da lan");
                    yield return StartCoroutine(ThrowRollingStone());
                }

                yield return new WaitForSeconds(3f);

            }
            yield return null;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // tông về phía trước
    void StartDash()
    {
        StartCoroutine(DashCoroutine());
    }

    IEnumerator DashCoroutine()
    {
        dashNormal = true;
        isSkillActived = true;

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }

        isDashing = true;
        dashTimeCounter = dashDuration;
        isPerformingSkill = true;

        yield return new WaitForSeconds(1.5f);

        while (dashTimeCounter > 0)
        {
            rb.velocity = new Vector2((facingRight ? 1 : -1) * dashSpeed, rb.velocity.y);
            dashTimeCounter -= Time.deltaTime;

            Collider2D playerCollider = Physics2D.OverlapCircle(radiusAttackTransform.position, detectionRadius, LayerMask.GetMask("Player"));
            if (playerCollider != null)
            {
                PlayerMovement playerMovement = playerCollider.GetComponent<PlayerMovement>();
                if (playerMovement != null && !isDamaged && !hasSlid) 
                {
                    playerMovement.TakeDamage(10f, 1f, 1.25f, 0.3f);
                    isDamaged = true;
                }
            }
            CheckInRadius();
            yield return null;
        }

        StopDash();
        isSkillActived = false; 
        isDamaged = false;
        hasSlid = false;
        hasPressedZ = false;
    }

    void StopDash()
    {
        isDashing = false;
        dashNormal = false;
        rb.velocity = Vector2.zero;
        isPerformingSkill = false;
        isSkillActived = true;
    }

    /*
    IEnumerator ThrowStone()
    {
        int numThrows = Random.Range(3, 6);

        for (int i = 0; i < numThrows; i++)
        {
            lastPlayerPosition = player.position;

            yield return new WaitForSeconds(2f);

            GameObject rock = Instantiate(rockPrefab, transform.position, Quaternion.identity);

            Vector2 throwDirection = (lastPlayerPosition - (Vector2)transform.position).normalized;

            if (!facingRight)
            {
                throwDirection.x = -Mathf.Abs(throwDirection.x);
            }

            Rigidbody2D rockRb = rock.GetComponent<Rigidbody2D>();

            rockRb.velocity = new Vector2(throwDirection.x * throwForce, 0);

            yield return new WaitForSeconds(throwDelay);

            Destroy(rock, 5f);
        }

    }*/

    // gai đá
    IEnumerator SpawnSpikes()
    {
        isSkillActived = true;

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }

        CameraShakerHandler.Shake(spikeShake);
        yield return new WaitForSeconds(2f);

        if (spikeStartPoint == null)
        {
            Debug.LogError("SpikeStartPoint transform not assigned!");
            yield break;
        }

        float startingPosition;
        if (facingRight)
        {
            startingPosition = spikeStartPoint.position.x - spikeDistance * (maxSpikes - 1) / 2;
        }
        else
        {
            startingPosition = spikeStartPoint.position.x + spikeDistance * (maxSpikes - 1) / 2;
        }

        for (int i = 0; i < maxSpikes; i++)
        {
            Vector2 spawnPosition;
            if (facingRight)
            {
                spawnPosition = new Vector2(startingPosition + i * spikeDistance, spikeStartPoint.position.y);
            }
            else
            {
                spawnPosition = new Vector2(startingPosition - i * spikeDistance, spikeStartPoint.position.y);
            }

            Instantiate(spikePrefab, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(spikeDelay);
        }

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }
        isSkillActived = false;
    }

    //combo
    IEnumerator DashForwad()
    {
        dashNormal = true;
        isSkillActived = true;
        isDashing = true;
        dashTimeCounter = dashDurationCombo;

        while (dashTimeCounter > 0)
        {
            rb.velocity = new Vector2((facingRight ? 1 : -1) * dashSpeed, rb.velocity.y);
            dashTimeCounter -= Time.deltaTime;

            Collider2D playerCollider = Physics2D.OverlapCircle(radiusAttackTransform.position, detectionRadius, LayerMask.GetMask("Player"));
            if (playerCollider != null)
            {
                PlayerMovement playerMovement = playerCollider.GetComponent<PlayerMovement>();
                if (playerMovement != null && !isDamaged && !hasSlid)
                {
                    playerMovement.TakeDamage(10f, 1f, 1.25f, 0.3f);
                    isDamaged = true;
                }
            }
            CheckInRadius();
            yield return null;
        }

        StopDash();
        isSkillActived = false;
        isDamaged = false;
        hasSlid = false;
        hasPressedZ = false;
    }
    IEnumerator JumpAndStomp()
    {
        isSkillActived = true;

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }

        yield return new WaitForSeconds(stompDelay);

        lastPlayerPosition = player.position;

        Vector2 directionToPlayer = (lastPlayerPosition - (Vector2)transform.position).normalized;
        rb.velocity = new Vector2(directionToPlayer.x * Mathf.Abs(jumpForce), jumpForce);

        yield return new WaitForSeconds(jumpUpDuration);

        rb.velocity = new Vector2(0, -Mathf.Abs(jumpForce * 5f));

        yield return new WaitUntil(() => Mathf.Abs(rb.velocity.y) < 0.1f);

        // Kiểm tra xem có trúng player không
        Collider2D playerCollider = Physics2D.OverlapCircle(JumpAttack.position, radiusJumpAttack, LayerMask.GetMask("Player"));
        if (playerCollider != null)
        {
            Debug.Log("Đã trúng player");
            PlayerMovement playerMovement = playerCollider.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(10f, 1f, 1.25f, 0.3f);
            }
        }

        CameraShakerHandler.Shake(jumpShake);
        rb.velocity = Vector2.zero;
        isSkillActived = false;
    }


    IEnumerator ThrowRollingStone()
    {
        isSkillActived = true;
        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }
        yield return new WaitForSeconds(1f);

        GameObject stone = Instantiate(rollingStone, throwStoneRoll.position, Quaternion.identity);

        Vector2 throwDirection = (player.position - transform.position).normalized;

        if (!facingRight)
        {
            throwDirection.x = -Mathf.Abs(throwDirection.x);
        }

        Rigidbody2D stoneRb = stone.GetComponent<Rigidbody2D>();
        stoneRb.gravityScale = 1;
        stoneRb.AddForce(new Vector2(throwDirection.x * throwForceStone, 0), ForceMode2D.Impulse);

        Collider2D stoneCollider = stone.GetComponent<Collider2D>();
        if (stoneCollider != null)
        {
            PhysicsMaterial2D rollingMaterial = new PhysicsMaterial2D
            {
                friction = 0.4f,
                bounciness = 0f
            };
            stoneCollider.sharedMaterial = rollingMaterial;
        }

        Destroy(stone, 3f);
        isSkillActived = false;
    }

    IEnumerator GolemCombo()
    {
        isSkillActived = true;
        yield return DashForwad();

        yield return new WaitForSeconds(0.5f);

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }

        yield return JumpAndStomp();

        yield return new WaitForSeconds(0.5f);

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }

        yield return ThrowRollingStone();

        isSkillActived = false;

    }

    // rơi đá
    private void SpawnFallingStones()
    {
        for (int i = 0; i < fallingStone.Length; i++)
        {
            Vector2 stoneSpawnPosition = fallingStone[i].position;

            GameObject rock = Instantiate(fallStone, stoneSpawnPosition, Quaternion.identity);

            Rigidbody2D rockRb = rock.GetComponent<Rigidbody2D>();
            rockRb.gravityScale = 1;

            Destroy(rock, 5f);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("TurnOn") && isPerformingSkill && dashNormal)
        {
            CameraShakerHandler.Shake(dashShake);
            StopDash();
            SpawnFallingStones();
            rb.velocity = Vector2.zero;
            StartCoroutine(StandStillAndResumeSkill());
        }
        else if (collision.gameObject.CompareTag("TurnOn"))
        {
            StopDash();
            Flip();
        }
    }

    IEnumerator StandStillAndResumeSkill()
    {
        isStandingStill = true;
        yield return new WaitForSeconds(8f);
        isStandingStill = false;

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }
    }
    public bool CheckInRadius()
    {
        // Kiểm tra nếu đã nhấn nút Z, nếu có thì không xử lý va chạm
        if (hasPressedZ)
        {
            Debug.Log("Người chơi đã nhấn Z, không kiểm tra va chạm.");
            return false; 
        }

        // Kiểm tra va chạm trong bán kính phía trước bằng hình hộp
        Vector2 boxSize = new Vector2(radiusCheck * 2, radiusCheck * 2);
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(radiusCheckTransform.position, boxSize, 0f, playerMask);
        Debug.Log("Số collider phát hiện: " + hitColliders.Length);

        Vector2 playerBoxSize = new Vector2(playerRadius * 2, playerRadius * 5);
        Collider2D playerCollider = Physics2D.OverlapBox(radiusCheckforPlayer.position, playerBoxSize, 0f, LayerMask.GetMask("Player"));

        // Nếu có collider phát hiện và playerCollider không null
        if (hitColliders.Length > 0 && playerCollider != null)
        {
            // Kiểm tra va chạm giữa box hình hộp và box player
            foreach (Collider2D hitCollider in hitColliders)
            {
                float distance = Vector2.Distance(hitCollider.transform.position, playerCollider.transform.position);
                if (distance <= (radiusCheck + playerRadius))
                {
                    Debug.Log("Có va chạm giữa box hình hộp và box player.");
                    Debug.Log("Đã phát hiện: " + hitCollider.name);

                    // Kiểm tra các điều kiện để hiện nút
                    if (!isDamaged && hitCollider.CompareTag("Player") && dashNormal)
                    {
                        Debug.Log("Player đã được phát hiện và điều kiện hiển thị nút được thỏa mãn.");

                        if (!isButtonActive)
                        {
                            activeButton = Instantiate(buttonPrefab, buttonSpawnTransform.position, Quaternion.identity);
                            activeButton.SetActive(true);
                            isButtonActive = true;
                            Debug.Log("Nút đã được hiển thị tại vị trí: " + buttonSpawnTransform.position);

                            StartCoroutine(SlowMotionAndMoveButton(hitCollider.transform));
                        }

                        return true;
                    }
                    else
                    {
                        Debug.Log("Điều kiện hiển thị nút không thỏa mãn: isDamaged=" + isDamaged + ", dashNormal=" + dashNormal);
                    }
                }
            }

            return false;
        }

        return false;
    }
    private IEnumerator SlowMotionAndMoveButton(Transform playerTransform)
    {
        // Thiết lập slow motion
        float slowMotionScale = 0.01f;
        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime = 0.02f * slowMotionScale;
        float slowMotionDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < slowMotionDuration)
        {
            if (isDamaged)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                Debug.Log("Tốc độ game đã trở về bình thường do isDamaged.");
                break;
            }

            activeButton.transform.position = buttonSpawnTransform.position;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                // Đánh dấu là đã nhấn Z
                hasPressedZ = true;
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                Debug.Log("Nút Z đã được nhấn, trở về tốc độ game bình thường.");

                // Đổi màu nút thành xanh
                ChangeButtonColor(activeButton, Color.green);
                CreateAfterImage(playerTransform.position);

                // Gọi hàm lướt người chơi
                Vector2 direction = playerMovement.GetFacingDirection();
                StartCoroutine(SlidePlayer(playerTransform, direction));

                hasSlid = true;
                // Đợi 1.5 giây trước khi ẩn nút
                yield return new WaitForSeconds(1.5f);

                // Ẩn nút
                if (isButtonActive && activeButton != null)
                {
                    Destroy(activeButton);
                    isButtonActive = false;
                    Debug.Log("Nút đã bị ẩn.");
                }

                yield break;
            }

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Hủy nút sau khi slow motion
        if (isButtonActive && activeButton != null)
        {
            Destroy(activeButton);
            isButtonActive = false;
            Debug.Log("Nút đã bị ẩn sau " + slowMotionDuration + " giây.");
        }

        // Đặt tốc độ game về bình thường
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    private IEnumerator SlidePlayer(Transform playerTransform, Vector2 direction, bool preventDamage = false)
    {
        float slideSpeed = 8f;
        float slideDuration = 1f;
        float elapsedTime = 0f;

        Vector2 startPosition = playerTransform.position;
        Vector2 slideDistance = direction.normalized * slideSpeed;
        Vector2 slideDirection = direction.normalized;

        BoxCollider2D playerCollider = playerTransform.GetComponent<BoxCollider2D>();

        while (elapsedTime < slideDuration)
        {
            float t = elapsedTime / slideDuration;
            float smoothStep = t * t * (3f - 2f * t);

            Vector2 newPosition = Vector2.Lerp(startPosition, startPosition + slideDistance, smoothStep);

            // Kiểm tra va chạm với BoxCollider
            RaycastHit2D hit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, slideDirection, slideSpeed * Time.deltaTime, LayerMask.GetMask("TurnOn"));

            if (hit.collider != null)
            {
                Debug.Log("Va chạm với vật cản, dừng trượt.");
                break; 
            }

            // Cập nhật vị trí người chơi
            playerTransform.position = Vector2.Lerp(startPosition, startPosition + slideDistance, smoothStep);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (elapsedTime >= slideDuration)
        {
            playerTransform.position = startPosition + slideDistance;
        }

        if (preventDamage)
        {
            Debug.Log("Không nhận sát thương trong chế độ slow motion.");
        }
    }
    private void CreateAfterImage(Vector2 position)
    {
        GameObject afterImage = Instantiate(afterImagePrefab, position, Quaternion.identity);

        Vector2 facingDirection = playerMovement.GetFacingDirection();
        float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;

        if (angle > 0)
        {
            angle = 360f;
            afterImage.transform.localScale = new Vector3(Mathf.Abs(afterImage.transform.localScale.x), afterImage.transform.localScale.y, afterImage.transform.localScale.z);
        }
        else
        {
            afterImage.transform.localScale = new Vector3(afterImage.transform.localScale.x, afterImage.transform.localScale.y, afterImage.transform.localScale.z);
        }

        afterImage.transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(afterImage, 2f);
    }
    private void ChangeButtonColor(GameObject button, Color color)
    {
        SpriteRenderer buttonSprite = button.GetComponent<SpriteRenderer>();
        if (buttonSprite != null)
        {
            buttonSprite.color = color;
        }
    }

    public bool GetStatus()
    {
        return isStandingStill;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(radiusAttackTransform.position, detectionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(JumpAttack.position, radiusJumpAttack);

        if (radiusCheckTransform != null)
        {
            Gizmos.color = Color.yellow;
            Vector2 boxSize = new Vector2(radiusCheck * 2, radiusCheck * 2);
            Gizmos.DrawWireCube(radiusCheckTransform.position, boxSize);
        }

        if (radiusCheckforPlayer != null)
        {
            Gizmos.color = Color.blue; 
            Vector2 playerBoxSize = new Vector2(playerRadius * 2, playerRadius * 5);
            Gizmos.DrawWireCube(radiusCheckforPlayer.position, playerBoxSize);
        }
    }

}