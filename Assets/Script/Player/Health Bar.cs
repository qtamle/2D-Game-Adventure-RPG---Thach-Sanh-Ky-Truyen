using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider lostHealthSlider;
    public float maxHealth = 100f;
    public float health;
    public float lerpSpeed = 5f;
    public Image fillImage;
    public Image lostFillImage;

    private float currentHealth;
    private float delayedHealth;
    private Coroutine damageCoroutine;
    
    public GameOverScene gameOverScene;
    private bool gameOverTriggered = false;
    void Start()
    {
        health = maxHealth;
        currentHealth = health;
        delayedHealth = health;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        lostHealthSlider.maxValue = maxHealth;
        lostHealthSlider.value = health;

        fillImage = healthSlider.fillRect.GetComponent<Image>();
        lostFillImage = lostHealthSlider.fillRect.GetComponent<Image>();
    }

    void Update()
    {
        currentHealth = Mathf.Lerp(currentHealth, health, Time.deltaTime * lerpSpeed);
        healthSlider.value = currentHealth;

        UpdateHealthColor();

        if (health <= 0 && !gameOverTriggered)
        {
            health = 0f;
            TriggerGameOver();
        }
        else if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
        damageCoroutine = StartCoroutine(UpdateLostHealth());
    }

    private IEnumerator UpdateLostHealth()
    {
        yield return new WaitForSeconds(1.5f); 

        while (delayedHealth > health)
        {
            delayedHealth = Mathf.Lerp(delayedHealth, health, Time.deltaTime * lerpSpeed);
            lostHealthSlider.value = delayedHealth;
            yield return null;
        }
    }

    private void UpdateHealthColor()
    {
        if (health >= 80f)
        {
            fillImage.color = Color.green;
        }
        else if (health >= 60f && health < 80f)
        {
            fillImage.color = new Color(0.5f, 1f, 0.5f);
        }
        else if (health >= 40f && health < 60f)
        {
            fillImage.color = Color.yellow;
        }
        else if (health >= 20f && health < 40f)
        {
            fillImage.color = new Color(1f, 0.64f, 0f);
        }
        else
        {
            fillImage.color = Color.red;
        }
    }

    // Item
    public void Heal(float healAmount)
    {
        health += healAmount; 

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
        damageCoroutine = StartCoroutine(UpdateLostHealth());
    }

    private void TriggerGameOver()
    {
        if (gameOverScene != null)
        {
            gameOverScene.TriggerGameOver();
            gameOverTriggered = true; 
        }
    }
}
