using System.Collections;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public float damagePerSecond = 5f;
    public float damageInterval = 1f;

    private bool isPlayerInRange = false;
    private bool hasDamaged = false;

    private PlayerMovement playerMovement;
    private StatusEffects playerStatus;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement = other.GetComponent<PlayerMovement>();
            playerStatus = other.gameObject.GetComponentInChildren<StatusEffects>();

            if (playerMovement != null && !hasDamaged)
            {
                isPlayerInRange = true;
                StartCoroutine(ApplyDamage());
                hasDamaged = true;
            }

            if (playerStatus != null)
            {
                playerStatus.ApplyBleed(); 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            StopCoroutine(ApplyDamage());

            if (playerStatus != null)
            {
                playerStatus.ApplyBleed(); 
            }
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
