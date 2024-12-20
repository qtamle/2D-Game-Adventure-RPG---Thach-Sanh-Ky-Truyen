﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FirstGearGames.SmoothCameraShaker;
public class GhostTreeHealth : MonoBehaviour
{
    [SerializeField] private DamageFlash dameflash;
    [SerializeField] private Slider slider;
    [SerializeField] private Slider lostHealthSlider;
    [SerializeField] public Animator anim;

    public float health;
    public float maxHealth = 1000f;
    public float smoothTime = 0.2f;
    public float lostHealthLerpSpeed = 5f; // Tốc độ giảm của fill máu đã mất
    public string bossName;
    
    private float targetHealth;
    private float currentHealth;
    private float healthVelocity = 0f;
    private float delayedHealth;
    private Image fillImage;
    private Image lostFillImage;

    public ShakeData deadthShake;
    private SaveBoss saveBoss;

    private void Start()
    {

        saveBoss = FindObjectOfType<SaveBoss>();

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
        if (targetHealth < 0) 
        {          
            targetHealth = 0;
            Die();
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
            anim.SetTrigger("Death");
            Destroy(gameObject);
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

    public void Die()
    {
        saveBoss.MarkBossAsDefeated(bossName);
    }

}
