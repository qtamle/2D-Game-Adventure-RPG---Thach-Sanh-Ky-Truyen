using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleBounce : MonoBehaviour
{
    Vector2 lastVelocity;
    public float shakeAmount = 0.1f;
    public float rotationSpeed = 100f;
    public GameObject bubblePop;

    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
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
            animator.SetTrigger("Check");
            rb.AddForce(new Vector2(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount)), ForceMode2D.Impulse);

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
                GameObject bubble = Instantiate(bubblePop, collision.transform.position, Quaternion.Euler(0f,0f,0f));

                player.TakeDamage(18f, 0f, 0f, 0f);
                playerStatus.ApplyStun();

                Destroy(bubble,2f);
            }
            Destroy(gameObject);
        }
    }
}
