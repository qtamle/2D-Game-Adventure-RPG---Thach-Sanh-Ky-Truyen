using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TougueDamage : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(20, 1.5f, 0.65f, 0.1f); 
            }
        }
    }

}
