using System.Collections;
using UnityEngine;

public class GhostSpike : MonoBehaviour
{
    [Header("Spike Settings")]
    public GameObject smallSpikePrefab;
    public Transform[] smallSpikeSpawnPoints;
    public float explosionDelay = 2f;
    public float explosionRadius = 5f; 
    public float explosionDamage = 10f;
    public LayerMask playerMask;
    private bool isStuck = false;

    private Rigidbody2D rb2d;
    private void Start()
    {
        Collider2D[] playerColliders = GameObject.FindGameObjectWithTag("Player").GetComponents<Collider2D>();
        foreach (Collider2D playerCollider in playerColliders)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerCollider);
        }

        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rb2d.isKinematic = true;
            if (!isStuck)
            {
                isStuck = true;
                rb2d.isKinematic = false;
                rb2d.velocity = Vector2.zero;
                StartCoroutine(HandleExplosion());
            }
        }
    }
    private IEnumerator HandleExplosion()
    {
        float elapsedTime = 0f; 
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 2f;

        while (elapsedTime < explosionDelay) 
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / explosionDelay); 
            elapsedTime += Time.deltaTime; 
            yield return null;
        }

        transform.localScale = targetScale;

        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(5);
            }
            else
            {
                Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'AudioManager'.");
        }

        if (smallSpikeSpawnPoints.Length > 0)
        {
            // Spike bắn sang phải
            GameObject smallSpikeRight = Instantiate(smallSpikePrefab, smallSpikeSpawnPoints[0].position, Quaternion.identity);
            Rigidbody2D rbRight = smallSpikeRight.GetComponent<Rigidbody2D>();
            if (rbRight != null)
            {
                Vector2 directionRight = Vector2.right;
                rbRight.velocity = directionRight * 25f;
            }
        }

        if (smallSpikeSpawnPoints.Length > 1)
        {
            // Spike bắn sang trái
            GameObject smallSpikeLeft = Instantiate(smallSpikePrefab, smallSpikeSpawnPoints[1].position, Quaternion.identity);
            Rigidbody2D rbLeft = smallSpikeLeft.GetComponent<Rigidbody2D>();
            if (rbLeft != null)
            {
                Vector2 directionLeft = Vector2.left;
                rbLeft.velocity = directionLeft * 25f;
            }
        }

        if (smallSpikeSpawnPoints.Length > 2)
        {
            // Spike bắn chéo lên trên bên phải
            GameObject smallSpikeDiagonalRight = Instantiate(smallSpikePrefab, smallSpikeSpawnPoints[2].position, Quaternion.identity);
            Rigidbody2D rbDiagonalRight = smallSpikeDiagonalRight.GetComponent<Rigidbody2D>();
            if (rbDiagonalRight != null)
            {
                Vector2 directionDiagonalRight = new Vector2(1, 1).normalized; // Hướng chéo lên bên phải
                rbDiagonalRight.velocity = directionDiagonalRight * 25f;
            }
        }

        if (smallSpikeSpawnPoints.Length > 3)
        {
            // Spike bắn chéo lên trên bên trái
            GameObject smallSpikeDiagonalLeft = Instantiate(smallSpikePrefab, smallSpikeSpawnPoints[3].position, Quaternion.identity);
            Rigidbody2D rbDiagonalLeft = smallSpikeDiagonalLeft.GetComponent<Rigidbody2D>();
            if (rbDiagonalLeft != null)
            {
                Vector2 directionDiagonalLeft = new Vector2(-1, 1).normalized; // Hướng chéo lên bên trái
                rbDiagonalLeft.velocity = directionDiagonalLeft * 25f;
            }
        }

        Collider2D player = Physics2D.OverlapCircle(transform.position, explosionRadius, playerMask);
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(explosionDamage, 0.5f, 0.65f, 0.1f);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}