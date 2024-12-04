using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EagleHealthbar : MonoBehaviour
{
    [SerializeField] private DamageFlash dameflash;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider lostHealthSlider;
    [SerializeField] private Slider shieldSlider;
    [SerializeField] private Slider lostShieldSlider;
    public float health;
    public float maxHealth = 1200f;
    public float shield;
    public float maxShield = 100f;
    public float smoothTime = 0.2f;
    public float lostHealthLerpSpeed = 5f;
    public float lostShieldLerpSpeed = 5f;

    public Animator anim;
    public Active active;
    public string bossname;

    private float targetHealth;
    private float currentHealth;
    private float healthVelocity = 0f;
    private float delayedHealth;
    private float targetShield;
    private float currentShield;
    private float shieldVelocity = 0f;
    private float delayedShield;
    private Image healthFillImage;
    private Image lostHealthFillImage;
    private Image shieldFillImage;
    private Image lostShieldFillImage;

    private SaveBoss saveBoss;

    public Transform glassSpawn;
    [SerializeField] private ParticleSystem shieldDepletedEffect;
    [SerializeField] private EagleSkillRemake eagleSkillRemake;

    private void Start()
    {
        anim = GetComponent<Animator>();
        saveBoss = FindObjectOfType<SaveBoss>();


        health = maxHealth;
        shield = maxShield;
        targetHealth = health;
        currentHealth = health;
        delayedHealth = health;
        targetShield = shield;
        currentShield = shield;
        delayedShield = shield;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
            healthFillImage = healthSlider.fillRect.GetComponent<Image>();
            UpdateHealthBarColor();
        }

        if (lostHealthSlider != null)
        {
            lostHealthSlider.maxValue = maxHealth;
            lostHealthSlider.value = health;
            lostHealthFillImage = lostHealthSlider.fillRect.GetComponent<Image>();
        }

        if (shieldSlider != null)
        {
            shieldSlider.maxValue = maxShield;
            shieldSlider.value = shield;
            shieldFillImage = shieldSlider.fillRect.GetComponent<Image>();
        }

        if (lostShieldSlider != null)
        {
            lostShieldSlider.maxValue = maxShield;
            lostShieldSlider.value = shield;
            lostShieldFillImage = lostShieldSlider.fillRect.GetComponent<Image>();
        }
    }

    public void TakeDamage(float damageShield, float damageHealth)
    {
        if (damageShield <= 0 && damageHealth <= 0) return;

        if (shield > 0 && damageShield > 0)
        {
            ApplyShieldDamage(damageShield);
        }

        if (shield <= 0 && damageHealth > 0)
        {
            ApplyHealthDamage(damageHealth);
        }
    }

    private void ApplyShieldDamage(float damage)
    {
        dameflash.CallDamageFlash();
        float damageToShield = Mathf.Min(damage, shield);
        shield -= damageToShield;
        damage -= damageToShield;

        if (shield <= 0)
        {
            GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

            if (audioManagerObject != null)
            {
                AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

                if (audioManager != null)
                {
                    audioManager.PlaySFX(2);
                }
                else
                {
                    Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
                }
            }
            else
            {
                Debug.LogError("No GameObject found with the tag 'AudioManager'.");
            }
            shield = 0;

            if (shieldDepletedEffect != null)
            {
                ParticleSystem effect = Instantiate(shieldDepletedEffect, glassSpawn.position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, 3f);
            }
            StartCoroutine(RegenerateShield());
        }

        StartCoroutine(UpdateShieldBar());
        StartCoroutine(UpdateLostShieldBar());
    }


    private void ApplyHealthDamage(float damage)
    {
        targetHealth -= damage;
        dameflash.CallDamageFlash();
        if (targetHealth < 0) targetHealth = 0;

        health = targetHealth;
        StartCoroutine(UpdateHealthBar());
        StartCoroutine(UpdateLostHealthBar());
    }

    private IEnumerator UpdateHealthBar()
    {
        while (Mathf.Abs(currentHealth - targetHealth) > 0.01f)
        {
            currentHealth = Mathf.SmoothDamp(currentHealth, targetHealth, ref healthVelocity, smoothTime);
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
                UpdateHealthBarColor();
            }
            yield return null;
        }

        if (healthSlider != null)
        {
            healthSlider.value = targetHealth;
            UpdateHealthBarColor();
        }

        if (targetHealth <= 0)
        {
            GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

            if (audioManagerObject != null)
            {
                AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

                if (audioManager != null)
                {
                    audioManager.StopAllMusic();
                }
                else
                {
                    Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
                }
            }
            else
            {
                Debug.LogError("No GameObject found with the tag 'AudioManager'.");
            }
            active.SetAndPlayTimeline(1);
            Destroy(gameObject,4f);
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

    private IEnumerator UpdateShieldBar()
    {
        while (Mathf.Abs(currentShield - shield) > 0.01f)
        {
            currentShield = Mathf.SmoothDamp(currentShield, shield, ref shieldVelocity, smoothTime);
            if (shieldSlider != null)
            {
                shieldSlider.value = currentShield;
            }
            yield return null;
        }

        if (shieldSlider != null)
        {
            shieldSlider.value = shield;
        }
    }

    private IEnumerator UpdateLostShieldBar()
    {
        yield return new WaitForSeconds(1.5f);

        while (Mathf.Abs(delayedShield - shield) > 0.01f)
        {
            delayedShield = Mathf.Lerp(delayedShield, shield, Time.deltaTime * lostShieldLerpSpeed);
            if (lostShieldSlider != null)
            {
                lostShieldSlider.value = delayedShield;
            }
            yield return null;
        }

        if (lostShieldSlider != null)
        {
            lostShieldSlider.value = shield;
        }
    }

    private IEnumerator RegenerateShield()
    {
        yield return new WaitForSeconds(12.5f);

        while (shield < maxShield)
        {
            shield += Time.deltaTime * (maxShield / 2f);
            if (shield > maxShield)
            {
                shield = maxShield;
            }
            StartCoroutine(UpdateShieldBar());
            yield return null;
        }

        if (shieldSlider != null)
        {
            shieldSlider.value = shield;
        }
    }

    private void UpdateHealthBarColor()
    {
        if (healthFillImage == null) return;

        float healthPercentage = currentHealth / maxHealth;

        if (healthPercentage >= 0.8f)
        {
            healthFillImage.color = Color.green;
        }
        else if (healthPercentage >= 0.6f)
        {
            healthFillImage.color = new Color(0.5f, 1f, 0.5f);
        }
        else if (healthPercentage >= 0.4f)
        {
            healthFillImage.color = Color.yellow;
        }
        else if (healthPercentage >= 0.2f)
        {
            healthFillImage.color = new Color(1f, 0.64f, 0f);
        }
        else
        {
            healthFillImage.color = Color.red;
        }
    }
}
