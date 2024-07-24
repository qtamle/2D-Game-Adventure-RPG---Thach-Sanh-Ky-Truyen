using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public float maxHealth = 100f;
    public float health;
    public float lerpSpeed = 5f;

    private float currentHealth;

    void Start()
    {
        health = maxHealth;
        currentHealth = health;
    }

    void Update()
    {
        CheckHealth();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }

        currentHealth = Mathf.Lerp(currentHealth, health, Time.deltaTime * lerpSpeed);
        healthSlider.value = currentHealth;
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
    }

    private void CheckHealth()
    {
        if (health > 100f)
        {
            health = 100f;
        }else if (health < 0f)
        {
            health = 0f;
        }
    }
}
