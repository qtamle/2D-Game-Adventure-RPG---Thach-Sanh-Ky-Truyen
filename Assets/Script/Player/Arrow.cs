using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    public float damage = 15f;
    public LayerMask bossLayer;
    public LayerMask healthbarEnemyLayer;
    public GameObject PopupDamage;
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
        int damageShieldEagle = Random.Range(15, 20);
        int damageShield = Random.Range(25, 30);
        int damageBoss = Random.Range(25, 30);

        Vector3 popupPosition = collision.transform.position;

        if (((1 << collision.gameObject.layer) & bossLayer) != 0)
        {
            EagleHealthbar eagleBoss = collision.gameObject.GetComponent<EagleHealthbar>();
            if (eagleBoss != null)
            {
                eagleBoss.TakeDamage(damageShieldEagle, damageBoss);
                Destroy(gameObject);
            }

            GolemHealthbar golemHealth = collision.gameObject.GetComponent<GolemHealthbar>();
            if (golemHealth != null)
            {
                golemHealth.TakeDamage(damageShield, damageBoss);
                Destroy(gameObject);
            }

            HealthBarLT lt = collision.gameObject.GetComponent<HealthBarLT>();
            if (lt != null)
            {
                lt.TakeDamage(damageBoss);
                Destroy(gameObject);
            }

            Phase2Health ltPhase2 = collision.gameObject.GetComponent<Phase2Health>();
            if (ltPhase2 != null)
            {
                ltPhase2.TakeDamage(damageBoss);
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
                ShowDamage((damage * 10).ToString(), popupPosition);
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.CompareTag("Block"))
        {
            Destroy(gameObject);
        }
    }

    private void ShowDamage(string text, Vector3 position)
    {
        if (PopupDamage != null)
        {
            GameObject popup = Instantiate(PopupDamage, position, Quaternion.identity);
            TMP_Text damageText = popup.GetComponentInChildren<TMP_Text>();

            Color randomColor = Random.value > 0.5f
                ? new Color(1f, 0f, 0f, 132f / 255f)
                : new Color(1f, 1f, 1f, 132f / 255f);

            damageText.color = randomColor;
            damageText.text = text;
        }
    }
}
