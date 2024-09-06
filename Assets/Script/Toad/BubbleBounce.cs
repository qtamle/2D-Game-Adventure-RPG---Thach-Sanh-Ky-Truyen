using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleBounce : MonoBehaviour
{
    private Rigidbody2D rb;
    Vector2 lastVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        int bubbleLayer = LayerMask.NameToLayer("Bubble");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int bugLayer = LayerMask.NameToLayer("Bug");

        Physics2D.IgnoreLayerCollision(bubbleLayer, bubbleLayer, true);
        Physics2D.IgnoreLayerCollision(bubbleLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(bugLayer, bugLayer, true);
    }

    private void Update()
    {
        lastVelocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bubble"))
        {
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Bug"))
        {
            return;
        }

        if (collision.gameObject.CompareTag("TurnOn"))
        {
            var speed = lastVelocity.magnitude;
            var direction = Vector2.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
            rb.velocity = direction * Mathf.Max(speed, 0);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            StatusEffects playerStatus = collision.gameObject.GetComponentInChildren<StatusEffects>();
            if (player != null)
            {
                player.TakeDamage(10, 0f, 0f, 0f);
                playerStatus.ApplyStun();
            }
            Destroy(gameObject);
        }
    }
}
