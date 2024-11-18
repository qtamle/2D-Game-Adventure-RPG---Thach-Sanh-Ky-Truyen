using System.Collections;
using UnityEngine;

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
    private bool skillCompleted = false;
    private bool isSkillInProgress = false;

    [Header("Teleport")]
    public Collider2D teleportArea;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundCheckRadius = 0.5f;
    private bool isOnGround;

    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private void Start()
    {
        int myLayer = gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");

        Physics2D.IgnoreLayerCollision(myLayer, playerLayer, true);

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        isOnGround = CheckGround();

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isSkillInProgress && !skillCompleted) 
            {
                StartCoroutine(JumpAndSummonLightning());
            }
            else if (skillCompleted) 
            {
                skillCompleted = false;
                StartCoroutine(JumpAndSummonLightning());
            }
        }

        if (Input.GetKeyDown(KeyCode.O) && CheckGround())
        {
            StartCoroutine(TeleportSkill());
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
        Vector3 originalPosition = transform.position;

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

        yield return new WaitForSeconds(5f);

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        transform.position = originalPosition;

        FlipBasedOnPlayerPosition();

        if (rb != null)
        {
            rb.isKinematic = false; 
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
    }
    private Vector3 GetRandomPositionInCollider(Collider2D collider)
    {
        Bounds bounds = collider.bounds;

        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector3(randomX, randomY, transform.position.z);
    }

    // summon lightning
    private IEnumerator SummonLightning()
    {
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
    }

    // Fireball Skill
    private IEnumerator FireballSkill()
    {
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

        Vector3 throwDirection = new Vector3(1, -1, 0).normalized;

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

        Vector3 summonPosition = explosionPosition + new Vector3(4f, 0f, 0f);

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < flameCount; i++)
        {
            GameObject flame = Instantiate(flamePrefab, summonPosition, Quaternion.Euler(-90f,0f,0f));

            Destroy(flame, 5f);

            summonPosition += new Vector3(5f, 0f, 0f); 

            yield return new WaitForSeconds(flameInterval); 
        }

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
        if (isSkillInProgress || skillCompleted) yield break; 

        isSkillInProgress = true;
        skillCompleted = false;

        yield return new WaitForSeconds(1.5f);

        Vector3 lastPlayerPosition = player.position;

        for (int i = 0; i < jumpsCount; i++)
        {
            FlipBasedOnPlayerPosition();

            Vector3 jumpTarget = player.position;

            float elapsedTime = 0f;
            Vector3 startPosition = transform.position;

            while (elapsedTime < jumpDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / jumpDuration;
                float heightOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;

                transform.position = Vector3.Lerp(startPosition, jumpTarget, t) + new Vector3(0, heightOffset, 0);
                yield return null;
            }

            transform.position = jumpTarget; 
            if (CheckGround())
            {
                Vector3 explosionGround = new Vector3(transform.position.x, 18.73f, transform.position.z);
                GameObject explosion1 = Instantiate(explosionPrefab, explosionGround, Quaternion.identity);
                Destroy(explosion1, 1f);

                yield return new WaitForSeconds(0.5f);

                Vector3 lightningPosition = new Vector3(transform.position.x, 23f, transform.position.z);
                GameObject lightning = Instantiate(lightningPrefab, lightningPosition, Quaternion.Euler(90f, 0f, 0f));
                Destroy(lightning, 1f);
            }

            yield return new WaitForSeconds(1f); 
        }

        StartCoroutine(SummonRandomLightning());

        Vector3 finalExplosionPosition = new Vector3(transform.position.x, 18.73f, transform.position.z);
        GameObject finalExplosion = Instantiate(explosionPrefab, finalExplosionPosition, Quaternion.identity);
        Destroy(finalExplosion, 1f);

        yield return new WaitForSeconds(0.5f);

        Vector3 finalLightningPosition = new Vector3(transform.position.x, 23f, transform.position.z);
        GameObject finalLightning = Instantiate(lightningPrefab, finalLightningPosition, Quaternion.Euler(90f, 0f, 0f));
        Destroy(finalLightning, 1f);

        skillCompleted = true;
        isSkillInProgress = false;
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


    private bool CheckGround()
    {
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

        return hit != null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
