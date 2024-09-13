using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoDamage : MonoBehaviour
{
    public float damagePerSecond = 5f; 
    public float damageInterval = 1f;

    private bool isPlayerInRange = false; 
    private PlayerMovement playerMovement; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                isPlayerInRange = true;
                StartCoroutine(ApplyDamage());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            StopCoroutine(ApplyDamage());
        }
    }

    private IEnumerator ApplyDamage()
    {
        while (isPlayerInRange)
        {
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(damagePerSecond, 0f, 0f, 0f);
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
