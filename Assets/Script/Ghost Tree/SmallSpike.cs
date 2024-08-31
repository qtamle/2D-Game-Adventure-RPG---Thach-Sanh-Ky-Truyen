using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallSpike : MonoBehaviour
{
    public float damage = 5f;


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
    }
}
