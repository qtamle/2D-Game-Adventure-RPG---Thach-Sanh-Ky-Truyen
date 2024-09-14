using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    public float damage = 15f;
    public LayerMask bossLayer;
    public LayerMask healthbarEnemyLayer;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & bossLayer) != 0)
        {
            HealthBarBoss bossHealth = collision.gameObject.GetComponent<HealthBarBoss>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
                Destroy(gameObject);
            }

            EagleHealthbar eagleBoss = collision.gameObject.GetComponent<EagleHealthbar>();
            if (eagleBoss != null)
            {
                eagleBoss.TakeDamage(20,damage);
                Destroy(gameObject);
            }

        }
        if (((1 << collision.gameObject.layer) & healthbarEnemyLayer) != 0)
        {
            HealthbarEnemy enemyHealth = collision.gameObject.GetComponent<HealthbarEnemy>();
            if (enemyHealth != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                enemyHealth.TakeDamage(damage, knockbackDirection);
                Destroy(gameObject);
            }
        }
    }
}
