using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class HealthBarBoss : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Slider lostHealthSlider;
    public float health;
    public float maxHealth = 1000f;
    public float smoothTime = 0.2f;
    public float lostHealthLerpSpeed = 5f; // Tốc độ giảm của fill máu đã mất

    public Animator anim;
    public string bossName;

    public float targetHealth;
    private float currentHealth;
    private float healthVelocity = 0f;
    private float delayedHealth;
    private Image fillImage;
    private Image lostFillImage;
    private Rigidbody2D rb;

    public ParticleSystem bloodEffect;
    public BossSkill bossSkill;
    public ObjectManager objectManager;

    [SerializeField] private DamageFlash dameflash;
    private SaveBoss saveBoss;
    private void Start()
    {
        saveBoss = FindObjectOfType<SaveBoss>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

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
        dameflash.CallDamageFlash();
        targetHealth -= damage;
        if (targetHealth < 0) 
        { 
            targetHealth = 0;
            saveBoss.MarkBossAsDefeated(bossName);
        }

        health = targetHealth;
        StartCoroutine(UpdateHealthBar());
        StartCoroutine(UpdateLostHealthBar());
        ShowBloodEffect();
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


    private void ShowBloodEffect()
    {
        if (bloodEffect != null)
        {
            Vector3 offset = new Vector3(0f, -2f, 0f);
            Vector3 effectPosition = transform.position + offset;
            ParticleSystem blood = Instantiate(bloodEffect, effectPosition, Quaternion.identity);
            Destroy(blood.gameObject, 0.5f);
        }
    }
}
