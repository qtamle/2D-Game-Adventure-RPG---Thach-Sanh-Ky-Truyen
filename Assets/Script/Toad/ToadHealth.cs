using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToadHealth : MonoBehaviour
{
    [SerializeField] private DamageFlash dameflash;
    [SerializeField] private Slider slider;
    [SerializeField] private Slider lostHealthSlider;
    public float health;
    public float maxHealth = 1000f;
    public float smoothTime = 0.2f;
    public float lostHealthLerpSpeed = 5f; // Tốc độ giảm của fill máu đã mất

    public Animator anim;
    private float targetHealth;
    private float currentHealth;
    private float healthVelocity = 0f;
    private float delayedHealth;
    private Image fillImage;
    private Image lostFillImage;
    public Active active;

    private void Start()
    {
        anim = GetComponent<Animator>();

        health = maxHealth;
        targetHealth = health;
        currentHealth = health;
        delayedHealth = health;

        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = health;
            fillImage = slider.fillRect.GetComponent<Image>();
            UpdateHealthBarColor();
        }

        if (lostHealthSlider != null)
        {
            lostHealthSlider.maxValue = maxHealth;
            lostHealthSlider.value = health;
            lostFillImage = lostHealthSlider.fillRect.GetComponent<Image>();
        }

    }

    public void TakeDamage(float damage)
    {
        targetHealth -= damage;
        dameflash.CallDamageFlash();
        if (targetHealth < 0) targetHealth = 0;

        health = targetHealth;
        StartCoroutine(UpdateHealthBar());
        StartCoroutine(UpdateLostHealthBar());
    }

    public void UpHealth(float Uphealth)
    {
        targetHealth += Uphealth;

        if (targetHealth > maxHealth)
        {
            targetHealth = maxHealth;
        }

        health = targetHealth;

        StartCoroutine(UpdateHealthBar());
        StartCoroutine(UpdateLostHealthBar());
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
            active.SetAndPlayTimeline(1);
          
            Destroy(gameObject, 2f);
        }
    }

    private IEnumerator UpdateLostHealthBar()
    {
        yield return new WaitForSeconds(1.5f);

        while (Mathf.Abs(delayedHealth - health) > 0.01f)
        {
            delayedHealth = Mathf.Lerp(delayedHealth, health, Time.deltaTime * lostHealthLerpSpeed);
            if (lostHealthSlider != null)
            {
                lostHealthSlider.value = delayedHealth;
            }
            yield return null;
        }

        if (lostHealthSlider != null)
        {
            lostHealthSlider.value = health;
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
}
