using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthbarEnemy : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRadius = 5f;
    public float health;
    public float maxHealth = 100f;
    public float smoothTime = 0.2f; 

    private float targetHealth;
    private float currentHealth;
    private float healthVelocity = 0f;
    private Image fillImage;

    private KnockbackEnemy knockbackEnemy; 

    private void Start()
    {
        health = maxHealth;
        targetHealth = health;
        currentHealth = health;

        knockbackEnemy = GetComponent<KnockbackEnemy>(); 

        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = health;
            fillImage = slider.fillRect.GetComponent<Image>();
        }
    }
        
    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        targetHealth -= damage;
        if (targetHealth < 0) targetHealth = 0;

        StartCoroutine(UpdateHealthBar());

        if (knockbackEnemy != null)
        {
            knockbackEnemy.ApplyKnockback(knockbackDirection);
        }
    }

    private IEnumerator UpdateHealthBar()
    {
        while (Mathf.Abs(currentHealth - targetHealth) > 0.01f)
        {
            currentHealth = Mathf.SmoothDamp(currentHealth, targetHealth, ref healthVelocity, smoothTime);
            if (slider != null)
            {
                slider.value = currentHealth;
                UpdateHealthBarColor();
            }
            yield return null;
        }

        if (slider != null)
        {
            slider.value = targetHealth;
        }

        if (targetHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void UpdateHealthBarColor()
    {
        if (targetHealth >= 80f)
        {
            fillImage.color = Color.green;
        }
        else if (targetHealth >= 60f && targetHealth < 80f)
        {
            fillImage.color = new Color(0.5f, 1f, 0.5f); 
        }
        else if (targetHealth >= 40f && targetHealth < 60f)
        {
            fillImage.color = Color.yellow;
        }
        else if (targetHealth >= 20f && targetHealth < 40f)
        {
            fillImage.color = new Color(1f, 0.64f, 0f); 
        }
        else
        {
            fillImage.color = Color.red;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
