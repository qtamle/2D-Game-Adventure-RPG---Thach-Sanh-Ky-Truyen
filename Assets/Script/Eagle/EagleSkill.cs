using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class EagleSkill : MonoBehaviour
{
    [Header("Wind-cut")]
    public GameObject windCut;
    public float speedWind;
    public Transform spawnPoint;
    private Transform player;

    [Header("Movement")]
    public Transform playerToMove; 
    public float flySpeed = 5f; 
    public float diveSpeed = 10f; 
    public float offScreenDistance = 5f; 
    public float overshootDistance = 2f;
    private Vector2[] corners;
    private Vector2 selectedCorner;
    private Vector2 currentDivePosition;
    private Vector2 startFallPosition;
    public Transform fallTarget;
    public float damageRadius = 1f;
    public Transform damageTransform;

    [Header("Tornado Skill")]
    public GameObject tornadoPrefab;
    public float tornadoDuration = 3f;
    public float pullForce = 5f;
    public Transform tornadoSpawn;
    private GameObject currentTornado;

    [Header("Feather Shot")]
    public GameObject feartherPrefab;
    public float featherSpeed;
    public float flySpeedforFeather;
    public int featherCount = 4;
    public float spreadAngle = 30f;
    private Vector2 lastKnownPlayerPosition;
    public float cornerOffset = 1f;

    [Header("Stomp Attack")]
    public float horizontalFlySpeed;
    public float flyExit;
    public float stompSpeed;
    public float detectionDistance;
    public int maxStomps = 3;
    public float stompCooldown = 3f;
    private bool canStomp = true;
    private bool isReturningToStart;
    private Vector2 initialPosition;
    public string turnTag = "TurnOn";
    public LayerMask playerMask;
    public LayerMask groundLayer;
    public float stompRadius = 1f;
    public Transform positionStompAttack;
    public float stompSpeedBack;

    [Header("Gust of Wind")]
    public GameObject windGustPrefab;
    public float windGustDuration = 3f;
    public float windGustForce = 10f;
    public float flySpeedToCorner = 5f;

    [Header("Other")]
    public ParticleSystem smoke;
    public Transform smokeSpawn;
    public float fallSpeed = 5f;

    public bool isSkillActive = false;
    private float skillCooldownMin = 4f;
    private float skillCooldownMax = 6f;
    private bool isStunned = false;
    private bool isMovingOffScreen = false;
    private bool isRepositioning = false;

    private CameraShake cam;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;

    [Header("Camera Shake")]
    public ShakeData stompShake;
    public ShakeData fallShake;
    private void Start()
    {
        //cam = GameObject.FindGameObjectWithTag("Shake").GetComponent<CameraShake>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        initialPosition = transform.position;

        corners = new Vector2[4];
        corners[0] = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)) + Vector3.left * offScreenDistance; // Góc trên trái
        corners[1] = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)) + Vector3.right * offScreenDistance; // Góc trên phải
        corners[2] = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)) + Vector3.left * offScreenDistance; // Góc dưới trái
        corners[3] = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)) + Vector3.right * offScreenDistance; // Góc dưới phải

        SelectRandomCorner();
        currentDivePosition = selectedCorner;
        startFallPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.2f, 0));

        StartCoroutine(RunRandomSkillRoutine());
    }
    private IEnumerator RunRandomSkillRoutine()
    {
        while (isStunned || isRepositioning)
        {
            yield return null; 
        }

        yield return new WaitForSeconds(3f);

        while (true)
        {
            if (!isSkillActive && !isStunned && !isRepositioning)
            {
                Debug.Log("Starting new skill execution.");
                yield return StartCoroutine(ExecuteRandomSkill());
                yield return new WaitForSeconds(Random.Range(skillCooldownMin, skillCooldownMax));
            }
            yield return null;
        }
    }
    private IEnumerator ExecuteRandomSkill()
    {
        if (isStunned)
        {
            Debug.Log("Eagle is stunned and cannot use skills.");
            yield break; 
        }

        isSkillActive = true;
        Debug.Log("Skill is active.");

        boxCollider.enabled = false;

        int skillIndex = Random.Range(0, 6);
        Debug.Log($"Executing skill index: {skillIndex}");

        switch (skillIndex)
        {
            case 0:
                yield return StartCoroutine(ShootWindCut());
                break;
            case 1:
                yield return StartCoroutine(ActivateTornadoSkill());
                break;
            case 2:
                yield return StartCoroutine(EagleDiveAttack(4));
                break;
            case 3:
                yield return StartCoroutine(FeatherShotSkill());
                break;
            case 4:
                yield return StartCoroutine(StompAttack());
                break;
            case 5:
                yield return StartCoroutine(ActivateWindGustSkill());
                break;
        }

        isSkillActive = false;
        Debug.Log("Skill has completed and isSkillActive set to false.");

        boxCollider.enabled = true;
    }

    // bắn vết cắt gió
    public IEnumerator ShootWindCut()
    {
        ShootSingleWindCut(new Vector2(-1, -1));
        yield return new WaitForSeconds(1f);

        ShootSingleWindCut(new Vector2(1, -1));
        yield return new WaitForSeconds(1f);

        ShootDualWindCut();
    }

    private void ShootSingleWindCut(Vector2 direction)
    {
        direction = direction.normalized;

        GameObject windcut = Instantiate(windCut, spawnPoint.position, Quaternion.identity);
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        windcut.transform.rotation = Quaternion.Euler(0,0, angle - 90);

        Rigidbody2D rb = windcut.GetComponent<Rigidbody2D>();
        rb.velocity = direction * speedWind;

        Destroy(windcut, 2f);
    }

    private void ShootDualWindCut()
    {
        ShootSingleWindCut(new Vector2(1,-1));
        ShootSingleWindCut(new Vector2(-1,-1));
    }

    // đại bàng lao tới
    private void SelectRandomCorner()
    {
        // Chọn góc ngẫu nhiên
        selectedCorner = corners[Random.Range(0, corners.Length)];

        if (selectedCorner.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    IEnumerator EagleDiveAttack(int maxDives)
    {
        int diveCount = 0;

        while (diveCount < maxDives)
        {
            while (Vector2.Distance(transform.position, currentDivePosition) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, currentDivePosition, flySpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            // Lưu vị trí cuối cùng của player
            Vector2 playerPosition = player.position;

            if (playerPosition.x > transform.position.x)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Hướng về bên phải
            }
            else if (playerPosition.x < transform.position.x)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Hướng về bên trái
            }

            // Tính toán điểm vượt qua vị trí player
            Vector2 directionToPlayer = (playerPosition - (Vector2)transform.position).normalized;
            Vector2 overshootPosition = playerPosition + directionToPlayer * overshootDistance;

            float diveTime = 0f;
            bool hasDamaged = false;
            while (diveTime < 4f && Vector2.Distance(transform.position, overshootPosition) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, overshootPosition, diveSpeed * Time.deltaTime);
                diveTime += Time.deltaTime;

                if (!hasDamaged)
                {
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(damageTransform.position, damageRadius);
                    foreach (var collider in colliders)
                    {
                        if (collider.CompareTag("Player"))
                        {
                            PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                            StatusEffects playerStatus = collider.gameObject.GetComponentInChildren<StatusEffects>();
                            if (playerMovement != null)
                            {
                                playerMovement.TakeDamage(10f, 0.5f, 0.65f, 0.1f);
                                playerStatus.ApplyBleed();
                                hasDamaged = true;
                                break;
                            }
                        }
                    }
                }

                yield return null;
            }

            transform.position = overshootPosition;
            yield return new WaitForSeconds(2f);
            currentDivePosition = overshootPosition;

            diveCount++;
        }

        float fallDuration = 3f;
        float elapsedTime = 0f;
        Vector2 startFallPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.2f, 0));

        while (elapsedTime < fallDuration)
        {
            float t = elapsedTime / fallDuration;
            transform.position = Vector2.Lerp(startFallPosition, fallTarget.position, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = fallTarget.position;
    }

    // lốc xoáy
    private IEnumerator ActivateTornadoSkill()
    {
        yield return new WaitForSeconds(2f);

        currentTornado = Instantiate(tornadoPrefab, tornadoSpawn.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));

        TornadoButtonPress buttonPressScript = player.GetComponent<TornadoButtonPress>();
        StartCoroutine(buttonPressScript.StartButtonPress());

        float elapsedTime = 0f;

        while (elapsedTime < tornadoDuration)
        {
            elapsedTime += Time.deltaTime;

            if (player != null)
            {
                Vector2 playerPosition = player.position;
                Vector2 tornadoPosition = currentTornado.transform.position;

                bool isButtonPressedCorrectly = buttonPressScript.IsButtonPressedCorrectly();

                float adjustedPullForce = isButtonPressedCorrectly ? pullForce * 0.65f : pullForce;

                player.position = new Vector2(
                    Mathf.MoveTowards(player.position.x, tornadoPosition.x, adjustedPullForce * Time.deltaTime),
                    player.position.y
                );
            }

            yield return null;
        }

        Destroy(currentTornado);
    }

    // bắn lông vũ
    private IEnumerator FeatherShotSkill()
    {
        Vector2 topLeftCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)) + new Vector3(cornerOffset, -cornerOffset, 0);
        Vector2 topRightCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)) + new Vector3(-cornerOffset, -cornerOffset, 0);

        // Chọn ngẫu nhiên góc ban đầu
        Vector2 firstCorner = Random.Range(0, 2) == 0 ? topLeftCorner : topRightCorner;
        Vector2 secondCorner = firstCorner == topLeftCorner ? topRightCorner : topLeftCorner;

        yield return MoveToCornerAndShootFeathers(firstCorner);

        yield return MoveToCornerAndShootFeathers(secondCorner);

        yield return MoveOffScreen();
    }
    private IEnumerator MoveToCornerAndShootFeathers(Vector2 corner)
    {
        // Di chuyển đến góc đã chọn
        while (Vector2.Distance(transform.position, corner) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, corner, flySpeedforFeather * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        if (corner.x > transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        yield return new WaitForSeconds(1f);

        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        lastKnownPlayerPosition = player.position;
        Vector2 directionToPlayer = (lastKnownPlayerPosition - (Vector2)transform.position).normalized;
        float startAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - spreadAngle / 2f;

        for (int i = 0; i < featherCount; i++)
        {
            float currentAngle = startAngle + (spreadAngle / (featherCount - 1)) * i;
            Vector2 direction = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
            ShootFeather(direction);
        }

        yield return new WaitForSeconds(1f);
    }

    private void ShootFeather(Vector2 direction)
    {
        GameObject feather = Instantiate(feartherPrefab, transform.position, Quaternion.identity);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        feather.transform.rotation = Quaternion.Euler(0, 0, angle); 

        Rigidbody2D rb = feather.GetComponent<Rigidbody2D>();
        rb.velocity = direction * featherSpeed; 

        Destroy(feather, 2f);
    }

    // đại bàng dậm
    private IEnumerator StompAttack()
    {
        int stompCount = 0;
        bool movingRight = true;

        while (stompCount < maxStomps)
        {
            // Xác định hướng di chuyển
            float horizontalDirection = movingRight ? 1f : -1f;
            transform.Translate(Vector2.right * horizontalDirection * horizontalFlySpeed * Time.deltaTime);

            if (horizontalDirection > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Quay mặt phải
            }
            else if (horizontalDirection < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Quay mặt trái
            }

            // Kiểm tra va chạm với tag để đổi hướng
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * horizontalDirection, 0.1f);
            if (hit.collider != null)
            {
                movingRight = !movingRight; // Đổi hướng nếu va chạm
            }

            // Kiểm tra nếu phát hiện Player ở dưới
            RaycastHit2D playerHit = Physics2D.Raycast(transform.position, Vector2.down, detectionDistance, playerMask);
            if (playerHit.collider != null && playerHit.collider.CompareTag("Player"))
            {
                // Lưu vị trí hiện tại trước khi lao xuống
                initialPosition = transform.position;
                yield return StompOnPlayer(playerHit.point);
                stompCount++;
            }
            yield return null;
        }

        // Sau khi hoàn thành các lần dậm, quay lại vị trí ban đầu
        while (Vector2.Distance(transform.position, initialPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, initialPosition, horizontalFlySpeed * Time.deltaTime);
            yield return null;
        }

        float timeElapsed = 0f;
        Vector2 moveDirection = Vector2.right;
        while (timeElapsed < 3f)
        {
            transform.Translate(moveDirection * flyExit * Time.deltaTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startFallPosition;

        float fallDuration = 3f;
        float elapsedTime = 0f;
        while (elapsedTime < fallDuration)
        {
            float t = elapsedTime / fallDuration;
            transform.position = Vector2.Lerp(startFallPosition, fallTarget.position, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = fallTarget.position;
    }

    private IEnumerator StompOnPlayer(Vector2 playerPosition)
    {
        bool hasDamaged = false;

        while (Vector2.Distance(transform.position, playerPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerPosition, stompSpeed * Time.deltaTime);

            RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, 6f, groundLayer);
            if (groundHit.collider != null)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(positionStompAttack.position, stompRadius, playerMask);
                foreach (var collider in colliders)
                {
                    if (collider.CompareTag("Player"))
                    {
                        if (!hasDamaged)
                        {
                            PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                            if (playerMovement != null)
                            {
                                playerMovement.TakeDamage(15f, 0.5f, 0.65f, 0.1f);
                            }
                            hasDamaged = true;
                        }
                    }
                }
                ParticleSystem smokeEffect = Instantiate(smoke, smokeSpawn.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
                StartCoroutine(DestroyAfterTime(smokeEffect, 5f));

                CameraShakerHandler.Shake(stompShake);
                yield return new WaitForSeconds(1f);
                break;
            }
            yield return null;
        }

        while (Vector2.Distance(transform.position, initialPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, initialPosition, stompSpeedBack * Time.deltaTime);
            yield return null;
        }
    }

    // lực gió đẩy
    private IEnumerator ActivateWindGustSkill()
    {
        Vector2 topLeftCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)) + new Vector3(cornerOffset, -cornerOffset, 0);
        Vector2 topRightCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)) + new Vector3(-cornerOffset, -cornerOffset, 0);

        Vector2 targetCorner = Random.Range(0, 2) == 0 ? topLeftCorner : topRightCorner;

        // Di chuyển đến góc đã chọn và flip theo hướng di chuyển
        while (Vector2.Distance(transform.position, targetCorner) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetCorner, flySpeedToCorner * Time.deltaTime);

            // Flip khi di chuyển sang trái hoặc phải
            if (targetCorner == topLeftCorner)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Quay mặt trái
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Quay mặt phải
            }

            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        Vector2 windGustSpawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0));

        Vector2 windDirection = transform.position.x < windGustSpawnPosition.x ? Vector2.right : Vector2.left;

        // Flip đối tượng để nhìn về hướng người chơi
        if (player != null)
        {
            if (transform.position.x < player.position.x)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Nhìn về bên phải
            }
            else
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Nhìn về bên trái
            }
        }

        if (windDirection == Vector2.right)
        {
            // Gió thổi từ trái qua phải, điều chỉnh vị trí X, Y
            windGustSpawnPosition.x += -30f;
            windGustSpawnPosition.y = -5.9f;
            CreateWindGust(windGustSpawnPosition);
        }
        else
        {
            // Gió thổi từ phải qua trái, điều chỉnh vị trí X, Y
            windGustSpawnPosition.x -= -30f;
            windGustSpawnPosition.y = -5.9f;
            CreateFlippedWindGust(windGustSpawnPosition);
        }

        if (player != null)
        {
            StartCoroutine(SmoothPushPlayer(windDirection));
        }

        yield return new WaitForSeconds(windGustDuration);

        // Sau khi Wind Gust kết thúc, thực hiện flyExit
        float timeElapsed = 0f;
        Vector2 moveDirection = Vector2.right;
        while (timeElapsed < 5f)
        {
            transform.Translate(moveDirection * flyExit * Time.deltaTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startFallPosition;

        float fallDuration = 3f;
        float elapsedTime = 0f;
        while (elapsedTime < fallDuration)
        {
            float t = elapsedTime / fallDuration;
            transform.position = Vector2.Lerp(startFallPosition, fallTarget.position, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = fallTarget.position;
    }

    private void CreateWindGust(Vector2 position)
    {
        GameObject windGust = Instantiate(windGustPrefab, position, Quaternion.identity);
        Destroy(windGust, windGustDuration); 
    }

    private void CreateFlippedWindGust(Vector2 position)
    {
        GameObject windGust = Instantiate(windGustPrefab, position, Quaternion.identity);
        windGust.transform.localScale = new Vector3(-1, 1, 1); 
        Destroy(windGust, windGustDuration); 
    }

    private IEnumerator SmoothPushPlayer(Vector2 direction)
    {
        float pushDuration = 2.5f;
        float elapsedTime = 0f;
        Vector2 initialPosition = player.position;

        while (elapsedTime < pushDuration)
        {
            player.position = Vector2.Lerp(initialPosition, initialPosition + direction * windGustForce, elapsedTime / pushDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.position = initialPosition + direction * windGustForce;
    }

    private IEnumerator MoveOffScreen()
    {
        float timeElapsed = 0f;
        Vector2 moveDirection = Vector2.right;
        while (timeElapsed < 3f)
        {
            transform.Translate(moveDirection * flyExit * Time.deltaTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startFallPosition;

        float fallDuration = 3f;
        float elapsedTime = 0f;
        while (elapsedTime < fallDuration)
        {
            float t = elapsedTime / fallDuration;
            transform.position = Vector2.Lerp(startFallPosition, fallTarget.position, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = fallTarget.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            CameraShakerHandler.Shake(fallShake);
            ParticleSystem smokeEffect = Instantiate(smoke, smokeSpawn.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
            StartCoroutine(DestroyAfterTime(smokeEffect, 5f));

            Debug.Log("Eagle has collided with the ground.");
            StopAllCoroutines();
            StartCoroutine(HandleGroundCollision());
        }
    }
    private IEnumerator HandleGroundCollision()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        Debug.Log("Eagle is stunned and waiting on the ground.");

        yield return new WaitForSeconds(5f);

        if (!isMovingOffScreen)
        {
            Debug.Log("Eagle is moving off-screen after ground collision.");
            isMovingOffScreen = true;
            StartCoroutine(MoveOffScreen2());
        }
    }
    public void OnShieldDepleted()
    {
        Debug.Log("Eagle shield is depleted, starting fall and reposition.");
        StartCoroutine(FallAndReposition());
    }
    private IEnumerator FallAndReposition()
    {
        isRepositioning = true;

        rb.isKinematic = false;
        rb.bodyType = RigidbodyType2D.Dynamic;

        Debug.Log("Eagle is falling...");

        float fallDuration = 3f;
        Vector2 fallStartPosition = transform.position;
        Vector2 fallEndPosition = startFallPosition;

        float elapsedTime = 0f;
        while (elapsedTime < fallDuration)
        {
            float t = elapsedTime / fallDuration;
            transform.position = Vector2.Lerp(fallStartPosition, fallEndPosition, t);
            rb.velocity = new Vector2(0, -fallSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = fallEndPosition;

        bool hasHitGround = false;

        float waitTime = 0f;
        while (waitTime < 5f)
        {
            if (hasHitGround)
            {
                break;
            }
            waitTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        if (!isMovingOffScreen)
        {
            Debug.Log("Eagle is moving off-screen after falling.");
            isMovingOffScreen = true;
            StartCoroutine(MoveOffScreen2());
        }
    }
    private IEnumerator MoveOffScreen2()
    {
        Debug.Log("Eagle is flying off-screen.");

        float timeElapsed = 0f;
        Vector2 moveDirection = Vector2.right;
        while (timeElapsed < 5f)
        {
            // Flip hướng đối tượng theo hướng di chuyển
            if (moveDirection == Vector2.right)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Nhìn về bên phải
            }
            else
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // Nhìn về bên trái
            }

            transform.Translate(moveDirection * flyExit * Time.deltaTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startFallPosition;

        float fallDuration = 3f;
        float elapsedTime = 0f;
        while (elapsedTime < fallDuration)
        {
            float t = elapsedTime / fallDuration;
            transform.position = Vector2.Lerp(startFallPosition, fallTarget.position, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = fallTarget.position;

        rb.isKinematic = true;

        isStunned = false;
        isMovingOffScreen = false;
        isRepositioning = false;

        Debug.Log("Eagle has recovered and can use skills again.");

        StartCoroutine(RunRandomSkillRoutine());
    }
    IEnumerator DestroyAfterTime(ParticleSystem ps, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(ps.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * detectionDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 6f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(positionStompAttack.position, stompRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(damageTransform.position, damageRadius);
    }
}

