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
        healthSlider.maxValue = maxHealth; 
        healthSlider.value = health; 
    }

    void Update()
    {
        currentHealth = Mathf.Lerp(currentHealth, health, Time.deltaTime * lerpSpeed);
        healthSlider.value = currentHealth;

        if (health <= 0f)
        {
            health = 0f;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}
