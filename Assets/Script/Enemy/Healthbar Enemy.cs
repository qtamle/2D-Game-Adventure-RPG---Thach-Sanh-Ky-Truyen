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
    public float smoothTime = 0.2f; // Thời gian để chuyển đổi mượt mà

    private float targetHealth;
    private float currentHealth;
    private float healthVelocity = 0f;

    private void Start()
    {
        health = maxHealth;
        targetHealth = health;
        currentHealth = health;

        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = health;
            slider.gameObject.SetActive(false); // Ẩn thanh máu ban đầu
        }
    }

    private void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= detectionRadius)
            {
                if (slider != null)
                {
                    slider.gameObject.SetActive(true); // Hiện thanh máu khi Player gần
                }
            }
            else
            {
                if (slider != null)
                {
                    slider.gameObject.SetActive(false); // Ẩn thanh máu khi Player xa
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        targetHealth -= damage;
        if (targetHealth < 0) targetHealth = 0;

        StartCoroutine(UpdateHealthBar());
    }

    private IEnumerator UpdateHealthBar()
    {
        while (Mathf.Abs(currentHealth - targetHealth) > 0.01f)
        {
            currentHealth = Mathf.SmoothDamp(currentHealth, targetHealth, ref healthVelocity, smoothTime);
            if (slider != null)
            {
                slider.value = currentHealth;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
