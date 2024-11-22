using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public Slider staminaSlider;
    public float maxStamina = 100f;
    public float staminaDecreaseAmount = 10f;
    public float staminaRegenRate = 5f; 
    public float smoothTime = 0.2f; 

    private float currentStamina;
    private float targetStamina;
    private float staminaVelocity = 0f; 

    private StatusEffects statusEffects;
    private Bow bow;
    private void Start()
    {
        statusEffects = GetComponent<StatusEffects>();
        bow = GetComponent<Bow>();

        currentStamina = maxStamina;
        targetStamina = currentStamina;
        staminaSlider.value = currentStamina;
    }

    private void Update()
    {
        if (!PauseGame.isGamePaused) 
        {
            if (bow.isAiming)
            {
                DecreaseStamina(staminaDecreaseAmount * Time.unscaledDeltaTime);
            }
            else
            {
                RegenerateStamina();
            }
        }
        currentStamina = Mathf.SmoothDamp(currentStamina, targetStamina, ref staminaVelocity, smoothTime);
        UpdateStaminaUI();
    }


    public void DecreaseStamina(float amount)
    {
        targetStamina -= amount;
        targetStamina = Mathf.Clamp(targetStamina, 0, maxStamina);
        if (targetStamina < 0)
        {
            targetStamina = 0;
        }
        else if (targetStamina > maxStamina)
        {
            targetStamina = maxStamina;
        }
    }

    private void RegenerateStamina()
    {
        if (targetStamina < maxStamina)
        {
            targetStamina += staminaRegenRate * Time.unscaledDeltaTime;
            targetStamina = Mathf.Clamp(targetStamina, 0, maxStamina);
            if (targetStamina > maxStamina)
            {
                targetStamina = maxStamina;
            }
        }
    }

    public void UpdateStaminaUI()
    {
        staminaSlider.value = currentStamina;

        Image fillImage = staminaSlider.fillRect.GetComponent<Image>();
        if (fillImage != null)
        {
            fillImage.color = new Color(1f, 1f, 0f, 1f);
        }
    }

    public void RestoreStamina(float amount)
    {
        targetStamina += amount;
        if (targetStamina > maxStamina)
        {
            targetStamina = maxStamina;
        }
    }

    public float CurrentStamina
    {
        get { return currentStamina; }
    }

    public void ResetStaminaValues()
    {
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        targetStamina = currentStamina;
        UpdateStaminaUI();
    }

}
