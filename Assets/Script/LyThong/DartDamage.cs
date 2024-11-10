using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartDamage : MonoBehaviour
{
    [Header("Damage and Rotation")]
    public float damage;
    public float rotationSpeed;

    [Header("Knockback")]
    public float bounceBackForce = 3f;
    public float bounceForce = 5f; 
    public float bounceUpDuration = 0.5f; 
    private bool isBounced = false;
    public float fallSpeedBoost = 2f;
    public float backwardForce = 3f;

    [Header("Other")]
    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private Collider2D dartCollider;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dartCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerMovement = collision.GetComponent<PlayerMovement>();
            playerMovement.TakeDamage(damage, 0f, 0f, 0f);
            Destroy(gameObject);
        }
    }

    public void BounceOnHit()
    {
        if (!isBounced)
        {
            isBounced = true;

            rb.velocity = new Vector2(0f, 0f);  
            dartCollider.enabled = false;  

            StartCoroutine(HandleBounceUp());
        }
    }

    private IEnumerator HandleBounceUp()
    {
        yield return new WaitForSeconds(0.1f); 

        // Nảy lên
        rb.velocity = new Vector2(0f, bounceForce); 
        rb.gravityScale = 0;  

        yield return new WaitForSeconds(bounceUpDuration);

        rb.gravityScale = 1;
        rb.isKinematic = false;

        float direction = transform.localScale.x < 0 ? 1f : -1f;  
        rb.velocity = new Vector2(direction * backwardForce, -fallSpeedBoost);

        Destroy(gameObject, 3f);
    }

    /*DartDamage dart = enemy.GetComponent<DartDamage>();
            if (dart != null)
            {
                dart.BounceOnHit();
                Debug.Log("Dart bị tấn công và sẽ nảy lên!");
            }*/
}
