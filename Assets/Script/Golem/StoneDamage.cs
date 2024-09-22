using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneDamage : MonoBehaviour
{
    public float damage = 10f;
    private PlayerMovement playerMovement;
    private StatusEffects statusEffects;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMovement = collision.GetComponent<PlayerMovement>();
            statusEffects = collision.GetComponentInChildren<StatusEffects>();

            if (playerMovement != null )
            {
                playerMovement.TakeDamage(damage, 1f, 1.25f, 0.3f);
                statusEffects.ApplyStun();
                Destroy(gameObject);
            }
        }
    }
}
