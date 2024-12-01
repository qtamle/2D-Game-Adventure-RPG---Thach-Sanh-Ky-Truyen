using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    [Header("Bleed")]
    public float damage = 1.5f;
    public float bleedDuration = 5f;
    public float bleedInterval = 1f;
    private float bleedTimer;
    private bool isBleeding = false;

    [Header("Stun")]
    public float stunDuration = 3f;
    private bool isStunned = false;

    [Header("Slow")]
    public float slowDuration = 2f;
    public float slowFactor = 0.5f;
    private bool isSlowed = false;

    [Header("Stamina Reduction")]
    public Stamina playerStamina;

    private PlayerMovement playerMovement;
    private float originalSpeed;
    private bool isImmune = false;

    // Thêm biến lưu trữ tỷ lệ giảm sát thương
    private float currentDamageReduction = 0f;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        originalSpeed = playerMovement.speed;
        playerStamina = GetComponent<Stamina>();
    }

    public void ApplyImmunity(float immunityDuration)
    {
        if (!isImmune)
        {
            isImmune = true;
            StopAllEffects();
            StartCoroutine(ImmunityDuration(immunityDuration));
        }
    }

    private IEnumerator ImmunityDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        isImmune = false;
    }

    public void StopAllEffects()
    {
        if (isImmune) return;

        isBleeding = false;

        if (isStunned)
        {
            isStunned = false;
            playerMovement.SetStunned(false);
        }

        if (isSlowed)
        {
            isSlowed = false;
            playerMovement.speed = originalSpeed;
        }

        // Reset giảm sát thương khi kết thúc tất cả các hiệu ứng
        currentDamageReduction = 0f;
    }

    // Bleed
    public void ApplyBleed()
    {
        if (isImmune) return;

        isBleeding = true;
        bleedTimer = bleedDuration;
        StartCoroutine(Bleed());
        Debug.Log("Dang chay mau");
    }

    private IEnumerator Bleed()
    {
        while (isBleeding)
        {
            yield return new WaitForSeconds(bleedInterval);
            playerMovement.TakeDamage(damage, 0f, 0f, 0f);

            bleedTimer -= bleedInterval;

            if (bleedTimer <= 0f)
            {
                isBleeding = false;
            }
        }
    }

    // Stun
    public void ApplyStun()
    {
        if (isImmune || isStunned) return;

        isStunned = true;
        playerMovement.SetStunned(true);
        StartCoroutine(Stun());
    }
    private IEnumerator Stun()
    {
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        playerMovement.SetStunned(false);
    }

    // Slow
    public void ApplySlow()
    {
        if (isImmune || isSlowed) return;

        isSlowed = true;
        playerMovement.speed *= slowFactor;
        StartCoroutine(Slow());
    }

    private IEnumerator Slow()
    {
        yield return new WaitForSeconds(slowDuration);
        playerMovement.speed = originalSpeed;
        isSlowed = false;
    }

    // Stamina Reduction
    public void ApplyStaminaReduction()
    {
        if (playerStamina != null)
        {
            float staminaReduction = playerStamina.maxStamina * 0.4f;
            playerStamina.DecreaseStamina(staminaReduction);
        }
    }

    // Phương thức áp dụng giảm sát thương
    public void ApplyDamageReduction(float reduction, float duration)
    {
        // Cập nhật tỷ lệ giảm sát thương
        currentDamageReduction = reduction;

        // Bạn có thể áp dụng một cách thức giảm sát thương vào các hành động tấn công hoặc nhận sát thương của nhân vật
        Debug.Log("Applying damage reduction: " + (reduction * 100) + "% for " + duration + " seconds.");

        // Reset lại giảm sát thương sau khi hết thời gian
        StartCoroutine(RemoveDamageReductionAfterTime(duration));
    }

    // Coroutine để kết thúc hiệu ứng giảm sát thương sau một khoảng thời gian
    private IEnumerator RemoveDamageReductionAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        currentDamageReduction = 0f;  // Reset giảm sát thương
        Debug.Log("Damage reduction effect ended.");
    }

    // Hàm để tính sát thương cuối cùng có thể áp dụng cho nhân vật
    public float CalculateDamage(float incomingDamage)
    {
        // Trả về sát thương đã giảm, bạn có thể áp dụng thêm logic khác tùy vào game
        return incomingDamage * (1 - currentDamageReduction);
    }
}
