using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LTPhase2 : MonoBehaviour
{
    [Header("Summon Lightning")]
    public GameObject explosionPrefab; 
    public GameObject lightningPrefab;
    public Transform player;

    [Header("Fireball")]
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;
    public float flyUpHeight;
    public float fireballSpeed;

    [Header("Fireball and Flames")]
    public GameObject bigFireball;
    public GameObject flamePrefab;
    public GameObject explosionFirePrefab;
    public Transform bigFireballSpawnPoint;
    public float bigFireballSpeed;
    public int flameCount = 3;
    public float flameInterval = 0.5f;

    [Header("Jump Attack and Lightning")]
    public float jumpsCount;
    public float jumpHeight;
    public float jumpDuration = 1f;
    public float jumpDistance = 5f;
    public Transform damageAreaTransform;
    public GameObject jumpImpact;
    public float damageRadius;
    private bool skillCompleted = false;
    private bool isSkillInProgress = false;

    [Header("Lightning Shoot")]
    public GameObject lightningShootPrefab;
    public GameObject explositionLightning;
    public Transform spawnPoint;     
    public Transform playerPosition;
    private bool isLightningActive = false;
    private int lightningStrikeCount = 0;

    [Header("Spear")]
    [SerializeField] private GameObject spearPrefab;
    [SerializeField] private float spearSpawnHeight = 15.4f; 
    [SerializeField] private float spearSpacing = 4f;
    [SerializeField] private float spearSpeed = 10f;
    [SerializeField] private Transform playerPositionShoot;

    [Header("Teleport")]
    public Collider2D teleportArea;
    private bool isTeleporting = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundCheckRadius = 0.5f;
    private bool isOnGround;

    private Rigidbody2D rb;
    private bool isFacingRight = true;

    [Header("Random Skill")]
    private bool hasDamaged = false;
    private bool isSkillActive = false;
    private List<int> skillList = new List<int> { 0, 1, 2, 3, 4, 5 };
    private int lastSkillIndex = -1;
    private bool isDowned = false;
    private bool isSkyfallActive = false;
    private bool isRandomSkillActive = false;
    private void Start()
    {
        int myLayer = gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");

        Physics2D.IgnoreLayerCollision(myLayer, playerLayer, true);

        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(SkillRoutine());
    }

    private void Update()
    {
        isOnGround = CheckGround();

    }

    private IEnumerator SkillRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            if (!IsAnySkillRunning())
            {
                isRandomSkillActive = true;
                int skillIndex = GetRandomSkill();

                if (isTeleporting && (skillIndex == 3 || skillIndex == 4))
                {
                    Debug.Log("Skipping skill due to ongoing teleportation.");
                    isRandomSkillActive = false; 
                    yield return new WaitForSeconds(0.5f);
                    continue; 
                }

                yield return StartCoroutine(ActivateSkill(skillIndex));
                isRandomSkillActive = false;

                yield return new WaitForSeconds(Random.Range(0.5f, 1f)); 
            }
            else
            {
                yield return null;
            }
        }   
    }

    private int GetRandomSkill()
    {
        int skillIndex;
        do
        {
            skillIndex = skillList[Random.Range(0, skillList.Count)];
        } while (skillIndex == lastSkillIndex); 

        lastSkillIndex = skillIndex;
        return skillIndex;
    }

    private IEnumerator ActivateSkill(int skillIndex)
    {
        if (isSkillActive) yield break;

        isSkillActive = true;
        Vector3 originalPosition = transform.position;

        if (skillIndex == 0 || skillIndex == 1 || skillIndex == 2 || skillIndex == 5)
        {
            // Bắt đầu dịch chuyển
            yield return StartCoroutine(TeleportSkill());
            // Sử dụng skill ngay trong trạng thái dịch chuyển
            yield return StartCoroutine(ExecuteSkill(skillIndex));
            // Kết thúc dịch chuyển, quay lại vị trí cũ
            yield return StartCoroutine(ReturnToOriginalPosition(originalPosition));
        }
        else
        {
            FlipBasedOnPlayerPosition();
            yield return StartCoroutine(ExecuteSkill(skillIndex));
        }

        isSkillActive = false;
    }

    private IEnumerator ExecuteSkill(int skillIndex)
    {
        switch (skillIndex)
        {
            case 0:
                Debug.Log("Skill Summon Lightning");
                yield return SummonLightning();
                break;
            case 1:
                Debug.Log("Skill Fireball");
                yield return FireballSkill();
                break;
            case 2:
                Debug.Log("Skill Fire and Flames");
                yield return FireballAndFlames();
                break;
            case 3:
                Debug.Log("Skill Jump and Lightning");
                yield return JumpAndSummonLightning();
                break;
            case 4:
                Debug.Log("Skill Spawn Lightning");
                if (CheckGround())
                {
                    yield return SpawnLightning();
                }
                break;
            case 5:
                Debug.Log("Skill Spear");
                yield return SpearAttackDuringTeleport();
                break;
            default:
                Debug.Log("Skill ra khỏi tầm random");
                break;
        }
    }

    private void FlipBasedOnPlayerPosition()
    {
        if (player.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && isFacingRight)
        {
            Flip(); 
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 localScale = transform.localScale;
        localScale.x = -localScale.x;
        transform.localScale = localScale;
    }

    private IEnumerator TeleportSkill()
    {
        isTeleporting = true;

        Vector3 targetPosition = GetRandomPositionInCollider(teleportArea);

        yield return new WaitForSeconds(1f); 

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        transform.position = targetPosition;
        FlipBasedOnPlayerPosition();

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        yield return new WaitForSeconds(0.5f);

        isTeleporting = false;
    }

    private Vector3 GetRandomPositionInCollider(Collider2D collider)
    {
        Bounds bounds = collider.bounds;

        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector3(randomX, randomY, transform.position.z);
    }
    private IEnumerator ReturnToOriginalPosition(Vector3 originalPosition)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // Tắt hiển thị
        }

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Quay về vị trí ban đầu
        transform.position = originalPosition;
        FlipBasedOnPlayerPosition();

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true; // Hiển thị lại
        }

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        yield return null;
    }


    // summon lightning
    private IEnumerator SummonLightning()
    {
        isSkillActive = true;

        FlipBasedOnPlayerPosition();

        int summonCount = Random.Range(5, 7);

        for (int i = 0; i < summonCount; i++)
        {
            Vector3 playerPosition = player.position;

            Vector3 explosionPosition = new Vector3(playerPosition.x, 18.73f, playerPosition.z);
            GameObject explosion = Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);

            Destroy(explosion, 1f);

            yield return new WaitForSeconds(0.5f);

            Vector3 lightningPosition = new Vector3(playerPosition.x, 23f, playerPosition.z);
            GameObject lightning = Instantiate(lightningPrefab, lightningPosition, Quaternion.Euler(90f,0f,0f));

            Destroy(lightning, 1f);

            yield return new WaitForSeconds(1f);
        }

        StartCoroutine(SummonRandomLightning());

        isSkillActive = false;
    }

    // Fireball Skill
    private IEnumerator FireballSkill()
    {
        isSkillActive = true;

        int fireballCount = Random.Range(3, 6);

        /*Vector3 originalPosition = transform.position;
        Vector3 flyUpPosition = originalPosition + new Vector3(0, flyUpHeight, 0);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true; 
            rb.velocity = Vector2.zero;
        }

        float duration = 0.7f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(originalPosition, flyUpPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = flyUpPosition;

        yield return new WaitForSeconds(1f);*/

        for (int i = 0; i < fireballCount; i++)
        {
            FlipBasedOnPlayerPosition();

            GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.Euler(-90f, 0f, 0f));

            Vector3 directionToPlayer = player.position - fireball.transform.position;

            float angleX = Mathf.Atan2(directionToPlayer.y, directionToPlayer.z) * Mathf.Rad2Deg;

            fireball.transform.rotation = Quaternion.Euler(angleX, 0, 0);

            // Chờ trước khi bắn
            yield return new WaitForSeconds(1.5f);

            Vector3 targetPosition = player.position;
            Vector3 direction = (targetPosition - fireballSpawnPoint.position).normalized;

            Rigidbody2D fireballRb = fireball.GetComponent<Rigidbody2D>();
            if (fireballRb != null)
            {
                fireballRb.velocity = direction * fireballSpeed;
            }

            Destroy(fireball, 5f);

            yield return new WaitForSeconds(1f);
        }
        isSkillActive = false;

        /*elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(flyUpPosition, originalPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;

        if (rb != null)
        {
            rb.isKinematic = false; 
        }*/
    }

    // Fireball and Flame
    private IEnumerator FireballAndFlames()
    {
        isSkillActive = true;

        FlipBasedOnPlayerPosition();

        // Bay lên
        /*Vector3 originalPosition = transform.position;
        Vector3 flyUpPosition = originalPosition + new Vector3(0, 8f, 0); 
        float duration = 0.7f; 
        float elapsedTime = 0f;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true; 
            rb.velocity = Vector2.zero;
        }

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(originalPosition, flyUpPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = flyUpPosition;

        yield return new WaitForSeconds(0.5f);*/

        GameObject fireball = Instantiate(bigFireball, bigFireballSpawnPoint.position, Quaternion.identity);

        yield return new WaitForSeconds(3f);

        Vector3 throwDirection = isFacingRight ? new Vector3(1, -1, 0).normalized : new Vector3(-1, -1, 0).normalized;

        Rigidbody2D fireballRb = fireball.GetComponent<Rigidbody2D>();
        if (fireballRb != null)
        {
            fireballRb.velocity = throwDirection * bigFireballSpeed;
        }

        bool hasHitGround = false;
        while (!hasHitGround)
        {
            RaycastHit2D hit = Physics2D.Raycast(fireball.transform.position, Vector2.down, 0.1f, groundMask);
            if (hit.collider != null)
            {
                hasHitGround = true;
            }
            yield return null;
        }

        Destroy(fireball);

        Vector3 explosionPosition = fireball.transform.position; 
        GameObject explosion = Instantiate(explosionFirePrefab, explosionPosition, Quaternion.identity);
        Destroy(explosion, 1f);

        Vector3 summonPosition = explosionPosition + (isFacingRight ? new Vector3(4f, 0f, 0f) : new Vector3(-4f, 0f, 0f));

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < flameCount; i++)
        {
            GameObject flame = Instantiate(flamePrefab, summonPosition, Quaternion.Euler(-90f,0f,0f));

            Destroy(flame, 5f);

            summonPosition += isFacingRight ? new Vector3(5f, 0f, 0f) : new Vector3(-5f, 0f, 0f);

            yield return new WaitForSeconds(flameInterval); 
        }

        isSkillActive = false;

        /*yield return new WaitForSeconds(0.5f);

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(flyUpPosition, originalPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;

        if (rb != null)
        {
            rb.isKinematic = false; 
        }*/
    }

    // Jump Attack and Lightning
    private IEnumerator JumpAndSummonLightning()
    {
        if (isSkillInProgress) yield break; 

        isSkillInProgress = true;

        yield return new WaitForSeconds(1f); 

        Vector3 lastPlayerPosition = player.position;

        // Thực hiện các lần nhảy
        for (int i = 0; i < jumpsCount; i++)
        {
            FlipBasedOnPlayerPosition();

            Vector3 jumpTarget = player.position;
            Vector3 startPosition = transform.position;

            float elapsedTime = 0f;

            // Tính toán và thực hiện nhảy theo quỹ đạo parabol
            while (elapsedTime < jumpDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / jumpDuration;
                float heightOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight; // Độ cao nhảy

                transform.position = Vector3.Lerp(startPosition, jumpTarget, t) + new Vector3(0, heightOffset, 0);
                yield return null;
            }

            transform.position = jumpTarget;

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
                        playerMovement.TakeDamage(10f, 0.5f, 0.65f, 0.1f);
                    }
                }
            }

            if (CheckGround())
            {
                Vector3 groundImpactPosition = new Vector3(transform.position.x, transform.position.y - 2f, transform.position.z);
                GameObject groundImpactPrefab = Instantiate(jumpImpact, groundImpactPosition, Quaternion.Euler(-90f,0f,0f));
                Destroy(groundImpactPrefab, 2f);

                Vector3 explosionGround = new Vector3(transform.position.x, 18.73f, transform.position.z);
                GameObject explosion = Instantiate(explosionPrefab, explosionGround, Quaternion.identity);
                Destroy(explosion, 1f);

                yield return new WaitForSeconds(0.5f);

                Vector3 lightningPosition = new Vector3(transform.position.x, 23f, transform.position.z);
                GameObject lightning = Instantiate(lightningPrefab, lightningPosition, Quaternion.Euler(90f, 0f, 0f));
                Destroy(lightning, 1f);
            }

            yield return new WaitForSeconds(1f); 
        }

        StartCoroutine(SummonRandomLightning());

        // Tạo vụ nổ và tia sét cuối cùng
        Vector3 finalExplosionPosition = new Vector3(transform.position.x, 18.73f, transform.position.z);
        GameObject finalExplosion = Instantiate(explosionPrefab, finalExplosionPosition, Quaternion.identity);
        Destroy(finalExplosion, 1f);

        yield return new WaitForSeconds(0.5f);

        Vector3 finalLightningPosition = new Vector3(transform.position.x, 23f, transform.position.z);
        GameObject finalLightning = Instantiate(lightningPrefab, finalLightningPosition, Quaternion.Euler(90f, 0f, 0f));
        Destroy(finalLightning, 1f);

        isSkillInProgress = false; // Kết thúc trạng thái skill
    }

    private IEnumerator SummonRandomLightning()
    {
        Vector3[] explosionPositions = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            float randomX = Random.Range(-30.7f, 30.8f);
            explosionPositions[i] = new Vector3(randomX, 18.73f, transform.position.z);

            GameObject explosion = Instantiate(explosionPrefab, explosionPositions[i], Quaternion.identity);
            Destroy(explosion, 1f);
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 4; i++)
        {
            Vector3 lightningPosition = new Vector3(explosionPositions[i].x, 23f, explosionPositions[i].z);

            GameObject randomLightning = Instantiate(lightningPrefab, lightningPosition, Quaternion.Euler(90f, 0f, 0f));
            Destroy(randomLightning, 1f);
        }
    }

    // Spawn Lightning
    public IEnumerator SpawnLightning()
    {
        isSkillActive = true;

        if (lightningPrefab == null || spawnPoint == null || player == null || explosionPrefab == null)
        {
            Debug.LogWarning("Thiếu Lightning Prefab, Spawn Point, Player hoặc Explosion Prefab.");
            yield break;
        }

        if (isLightningActive)
        {
            Debug.LogWarning("Tia sét đã được kích hoạt, không thể tạo thêm.");
            yield break;
        }

        isLightningActive = true;

        Instantiate(explosionPrefab, spawnPoint.position, Quaternion.identity);

        yield return new WaitForSeconds(1f);

        GameObject lightning = Instantiate(lightningPrefab, spawnPoint.position, Quaternion.Euler(90f, 90f, 90f));

        if (isFacingRight)
        {
            lightning.transform.rotation = Quaternion.Euler(0f, 90f, 90f);
        }
        else
        {
            lightning.transform.rotation = Quaternion.Euler(180f, 90f, 90f); 
        }

        Debug.Log("Tia sét được tạo và bắn theo hướng nhân vật đang đối mặt.");

        yield return new WaitForSeconds(1f);

        Destroy(lightning);
        isLightningActive = false;
        isSkillActive = true;
    }

    // Spear
    private IEnumerator SpearAttackDuringTeleport()
    {
        int spearCount = Random.Range(3, 6);
        List<GameObject> spears = new List<GameObject>();

        Vector3 spawnStartPosition = new Vector3(transform.position.x - (spearCount - 1) * spearSpacing / 2, spearSpawnHeight, transform.position.z);

        // Triệu hồi giáo
        for (int i = 0; i < spearCount; i++)
        {
            Vector3 spawnPosition = spawnStartPosition + new Vector3(i * spearSpacing, 0, 0);
            GameObject spear = Instantiate(spearPrefab, spawnPosition, Quaternion.identity);
            spears.Add(spear);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1f);

        // Tấn công từng ngọn giáo
        foreach (GameObject spear in spears)
        {
            if (spear == null) continue;

            Vector3 targetPosition = playerPositionShoot.position;
            Vector3 direction = (targetPosition - spear.transform.position).normalized;

            // Tính góc xoay trục Z để mũi giáo hướng về Player
            float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg; 
            spear.transform.rotation = Quaternion.Euler(0, 0, rot + 20f); 

            // Di chuyển ngọn giáo về phía Player
            while (spear != null && Vector3.Distance(spear.transform.position, targetPosition) > 0.1f)
            {
                spear.transform.position += direction * spearSpeed * Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private bool CheckGround()
    {
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

        return hit != null;
    }

    private bool IsAnySkillRunning()
    {
        return isSkillActive || isRandomSkillActive || isTeleporting;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(damageAreaTransform.position, damageRadius);

    }
}
