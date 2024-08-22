using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarBoss : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public float health;
    public float maxHealth = 100f;
    public float smoothTime = 0.2f;

    private Animator anim;
    private float targetHealth;
    private float currentHealth;
    private float healthVelocity = 0f;
    private Image fillImage;

    private void Start()
    {
        anim = GetComponent<Animator>(); // Lấy Animator từ GameObject hiện tại

        health = maxHealth;
        targetHealth = health;
        currentHealth = health;

        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = health;
            fillImage = slider.fillRect.GetComponent<Image>();
        }
    }

    private void Update()
    {
        UpdateHealthBarColor();
    }

    public void TakeDamage(float damage)
    {
        targetHealth -= damage;
        if (targetHealth < 0) targetHealth = 0;

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
        if (fillImage == null) return; // Kiểm tra nếu fillImage không được thiết lập

        float healthPercentage = targetHealth / maxHealth * 100f;

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
        if (anim != null)
        {
            anim.SetTrigger("HealthbarShake"); 
        }
    }
}
