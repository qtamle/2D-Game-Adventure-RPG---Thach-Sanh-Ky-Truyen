using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneRoll : MonoBehaviour
{
    public float damage = 10f;
    private PlayerMovement playerMovement;
    private StatusEffects statusEffects;
    private void Start()
    {
        int stoneLayer = LayerMask.NameToLayer("StoneRoll");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int turnOnLayer = LayerMask.NameToLayer("TurnOn");

        Physics2D.IgnoreLayerCollision(stoneLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(stoneLayer, turnOnLayer, true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            statusEffects = collision.gameObject.GetComponentInChildren<StatusEffects>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(damage, 1f, 1.25f, 0.3f);
                statusEffects.ApplyStun();
                Destroy(gameObject);
            }
        }
    }
}
