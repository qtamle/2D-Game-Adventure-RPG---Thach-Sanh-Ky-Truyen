using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBoss : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    public float maxHealth = 500;
    public float currentHealth;
    public float smoothTime = 0.2f; // Thời gian để làm mượt
    private float healthVelocity = 0.0f;
    private float targetHealth;

    public Animator anim;
    private void Start()
    {
        currentHealth = maxHealth;
        targetHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        UpdateHealthBarColor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HealthBarShake();
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        targetHealth = Mathf.Max(0, targetHealth - damage);
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
                UpdateHealthBarColor();
            }
            yield return null;
        }

        if (slider != null)
        {
            slider.value = targetHealth;
        }

    }

    private void UpdateHealthBarColor()
    {
        float healthPercentage = (float)currentHealth / maxHealth * 100f;

        if (healthPercentage >= 80f)
        {
            fillImage.color = Color.green;
        }
        else if (healthPercentage >= 60f && healthPercentage < 80f)
        {
            fillImage.color = new Color(0.5f, 1f, 0.5f);
        }
        else if (healthPercentage >= 40f && healthPercentage < 60f)
        {
            fillImage.color = Color.yellow;
        }
        else if (healthPercentage >= 20f && healthPercentage < 40f)
        {
            fillImage.color = new Color(1f, 0.64f, 0f);
        }
        else
        {
            fillImage.color = Color.red;
        }
    }

    public void HealthBarShake()
    {
        anim.SetTrigger("HealthbarShake");
    }
}
