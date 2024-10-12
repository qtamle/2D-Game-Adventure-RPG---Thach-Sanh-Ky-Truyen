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
    private void Start()
    {
        statusEffects = GetComponent<StatusEffects>();

        currentStamina = maxStamina;
        targetStamina = currentStamina;
        staminaSlider.value = currentStamina;
    }

    private void Update()
    {
        // Tự động hồi phục stamina
        RegenerateStamina();

        currentStamina = Mathf.SmoothDamp(currentStamina, targetStamina, ref staminaVelocity, smoothTime);
        UpdateStaminaUI();

    }

    public void DecreaseStamina(float amount)
    {
        targetStamina -= amount;
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
            targetStamina += staminaRegenRate * Time.deltaTime;
            if (targetStamina > maxStamina)
            {
                targetStamina = maxStamina;
            }
        }
    }

    private void UpdateStaminaUI()
    {
        staminaSlider.value = currentStamina;
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

}
