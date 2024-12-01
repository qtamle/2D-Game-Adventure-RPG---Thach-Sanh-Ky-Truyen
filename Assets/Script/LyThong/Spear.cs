using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(10f, 0.5f, 0.65f, 0.1f);
            }
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

            if (audioManagerObject != null)
            {
                AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

                if (audioManager != null)
                {
                    audioManager.PlaySFX(9);
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
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
            }

            StartCoroutine(WaitAndDestroy());
        }
        else if (collision.gameObject.CompareTag("TurnOn"))
        {
            GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

            if (audioManagerObject != null)
            {
                AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

                if (audioManager != null)
                {
                    audioManager.PlaySFX(9);
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
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
            }

            StartCoroutine(WaitAndDestroy());
        }
    }
}
