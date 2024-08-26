using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

    public ParticleSystem bloodEffect;

    public SnakePhase2 snakePhase2;
    public Transform treePosition;

    private bool isPhase2Activated = false;

    public BossSkill bossSkill;

    public ObjectManager objectManager;

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

        if (snakePhase2 != null)
        {
            snakePhase2.enabled = false;
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Damage received: {damage}");
        targetHealth -= damage;
        if (targetHealth < 0) targetHealth = 0;

        health = targetHealth;
        StartCoroutine(UpdateHealthBar());
        HealthBarShake();
        ShowBloodEffect();

        if (!isPhase2Activated && health <= maxHealth * 0.5f)
        {
            ActivatePhase2();
        }
    }

    private void ActivatePhase2()
    {
        if (snakePhase2 != null)
        {
            snakePhase2.enabled = true;
            snakePhase2.treePosition = treePosition;
            Debug.Log("Phase 2 của rắn đã được kích hoạt!");
        }

        if (bossSkill != null)
        {
            bossSkill.ActivatePhase2();
            bossSkill.DestroyAllSpikeEffects();
            Debug.Log("BossSkill đã bị vô hiệu hóa.");
        }

        isPhase2Activated = true;

        if (objectManager != null)
        {
            Debug.Log("Gọi RemoveAllObjects từ ActivatePhase2.");
            objectManager.RemoveAllObjects();
        }
        else
        {
            Debug.Log("Không có object nào");
        }
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
