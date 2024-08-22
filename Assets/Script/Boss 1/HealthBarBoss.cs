using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarBoss : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public float health;
    public float maxHealth = 1000f;
    public float smoothTime = 0.2f;

    private Animator anim;
    private float targetHealth;
    private float currentHealth;
    private float healthVelocity = 0f;
    private Image fillImage;

    private void Start()
    {
        anim = GetComponent<Animator>();

        health = maxHealth;
        targetHealth = health;
        currentHealth = health;

        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = health;
            fillImage = slider.fillRect.GetComponent<Image>();
            UpdateHealthBarColor(); 
        }
    }

    public void TakeDamage(float damage)
    {
        targetHealth -= damage;
        if (targetHealth < 0) targetHealth = 0;

        // Cập nhật giá trị health hiện tại để phản ánh đúng trong UI
        health = targetHealth;
        StartCoroutine(UpdateHealthBar());
        HealthBarShake();
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
            UpdateHealthBarColor(); 
        }

        if (targetHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateHealthBarColor()
    {
        if (fillImage == null) return; 

        float healthPercentage = currentHealth / maxHealth;

        if (healthPercentage >= 0.8f)
        {
            fillImage.color = Color.green;
        }
        else if (healthPercentage >= 0.6f)
        {
            fillImage.color = new Color(0.5f, 1f, 0.5f);
        }
        else if (healthPercentage >= 0.4f)
        {
            fillImage.color = Color.yellow;
        }
        else if (healthPercentage >= 0.2f)
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
        if (anim != null)
        {
            anim.SetTrigger("HealthbarShake");
        }
    }
}
