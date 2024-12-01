using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallSmall : MonoBehaviour
{
    public float damage = 10f;
    public GameObject explosionPrefab;
    public LayerMask groundLayer;
    public float yOffset = 3f;
    private CircleCollider2D circleCollider;  

    private void Start()
    {
        circleCollider = gameObject.AddComponent<CircleCollider2D>();

        circleCollider.radius = 1f;
        circleCollider.isTrigger = true;

        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(damage, 0.5f, 0.65f, 0.1f);
            }
            Destroy(gameObject);
        }

        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
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
            Vector3 hitPosition = collision.ClosestPoint(transform.position);
            hitPosition.y += yOffset;

            GameObject explosion = Instantiate(explosionPrefab, hitPosition, Quaternion.identity);

            Destroy(explosion, 1f);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (circleCollider != null)
        {
            Gizmos.color = Color.cyan;

            Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
        }
    }
}
