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

    private void StopAllEffects()
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
    }

    // Bleed
    public void ApplyBleed()
    {
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
}
