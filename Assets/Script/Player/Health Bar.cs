using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public float maxHealth = 100f;
    public float health;
    public float lerpSpeed = 5f;
    public Image fillImage;

    private float currentHealth;

    void Start()
    {
        health = maxHealth;
        currentHealth = health;
        healthSlider.maxValue = maxHealth; 
        healthSlider.value = health;

        fillImage = healthSlider.fillRect.GetComponent<Image>();
    }

    void Update()
    {
        currentHealth = Mathf.Lerp(currentHealth, health, Time.deltaTime * lerpSpeed);
        healthSlider.value = currentHealth;

        UpdateHealthColor();

        if (health < 0)
        {
            health = 0f;
        }
        else if (health > 100)
        {
            health = 100f;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    private void UpdateHealthColor()
    {
        if (health >= 80f)
        {
            fillImage.color = Color.green;
        }else if (health >= 60f && health < 80f)
        {
            fillImage.color = new Color(0.5f, 1f, 0.5f);
        }else if (health >= 40f && health < 60f)
        {
            fillImage.color = Color.yellow;
        }else if (health >= 20f && health < 40f)
        {
            fillImage.color = new Color(1f, 0.64f, 0f);
        }else
        {
            fillImage.color = Color.red;
        }
    }
}
