using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineTie : MonoBehaviour
{
    public float holdDuration = 5f;
    public float missDuration = 0.1f;
    public float damagePerSecond = 5f;

    public bool isHitPlayer = false;
    private PlayerMovement playerMovement;
    private HealthBar playerHealth;
    private QuickTimeEvents quickTimeEvents;

    public static bool isVineActive = false;
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        quickTimeEvents = QuickTimeEvents.Instance;
        isVineActive = true;

        StartCoroutine(Drop());

    }

    private void OnDestroy()
    {
        isVineActive = false; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isHitPlayer = true;

            playerMovement = collision.GetComponent<PlayerMovement>();
            playerHealth = collision.GetComponent<HealthBar>();

            if (playerMovement != null)
            {
                Rigidbody2D player = collision.GetComponent<Rigidbody2D>();

                if (player != null)
                {
                    player.velocity = Vector2.zero;
                }

                playerMovement.enabled = false;

                StartCoroutine(ShowKeyPromptsWithDelay(1.5f));

                if (quickTimeEvents != null)
                {
                    quickTimeEvents.ShowKeyPrompts();
                    quickTimeEvents.OnAllKeysPressed += OnAllKeysPressed;
                }
                StopCoroutine(Drop()); 
                StartCoroutine(ReleasePlayer());
                StartCoroutine(DealDamageOverTime());
            }
        }
    }
    private IEnumerator ShowKeyPromptsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); 
        if (quickTimeEvents != null)
        {
            quickTimeEvents.ShowKeyPrompts();
            quickTimeEvents.OnAllKeysPressed += OnAllKeysPressed;
        }
    }

    private void OnAllKeysPressed()
    {
        Debug.Log("Tất cả phím đã được ấn, thả người chơi."); 
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            if (quickTimeEvents != null)
            {
                quickTimeEvents.HideKeyPrompts();
                quickTimeEvents.OnAllKeysPressed -= OnAllKeysPressed; 
            }
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    private IEnumerator ReleasePlayer()
    {
        while (quickTimeEvents.IsActive)
        {
            yield return null;
        }

        Debug.Log("Hết thời gian, thả người chơi."); 

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            if (quickTimeEvents != null)
            {
                quickTimeEvents.HideKeyPrompts();
                quickTimeEvents.OnAllKeysPressed -= OnAllKeysPressed; 
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator Drop()
    {
        yield return new WaitForSeconds(missDuration);

        if (!isHitPlayer)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DealDamageOverTime()
    {
        while (isHitPlayer)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);
            }
            yield return null;
        }
    }
}
