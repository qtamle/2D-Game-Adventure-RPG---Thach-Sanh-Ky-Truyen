using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public Image staminaFillImage; 
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

        UpdateStaminaUI(); 
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
    }

    private void RegenerateStamina()
    {
        if (targetStamina < maxStamina)
        {
            targetStamina += staminaRegenRate * Time.unscaledDeltaTime;
            targetStamina = Mathf.Clamp(targetStamina, 0, maxStamina);
        }
    }

    public void UpdateStaminaUI()
    {
        if (staminaFillImage != null)
        {
            // Tính toán tỷ lệ stamina còn lại
            float fillAmount = currentStamina / maxStamina;
            staminaFillImage.fillAmount = fillAmount;

            staminaFillImage.color = Color.Lerp(Color.yellow, Color.yellow, fillAmount);
        }
    }

    public void RestoreStamina(float amount)
    {
        targetStamina += amount;
        targetStamina = Mathf.Clamp(targetStamina, 0, maxStamina);
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
