using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindCutDamage : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            StatusEffects playerStatus = collision.gameObject.GetComponentInChildren<StatusEffects>();
            if (player != null)
            {
                player.TakeDamage(damage, 0.5f, 0.65f, 0.1f);
                playerStatus.ApplyBleed();
                Destroy(gameObject);
            }
        }
    }
}
