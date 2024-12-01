using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ShieldSkill : MonoBehaviour
{
    private StatusEffects statusEffects;
    private HealthBar healthBar;  // Thêm tham chiếu tới HealthBar
    public float shieldDuration = 5f;
    public float cooldown = 30f;
    public float damageReduction = 0.5f;

    private float lastUsedTime;
    private bool isShieldActive = false;
    private bool hasBlocked = false;

    private void Start()
    {
        statusEffects = GetComponent<StatusEffects>();
        healthBar = GetComponent<HealthBar>();  // Lấy tham chiếu đến HealthBar
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2) && CanUseSkill())
        {
            ActivateSkill();
        }
    }

    private bool CanUseSkill()
    {
        return Time.time >= lastUsedTime + cooldown && !isShieldActive;
    }

    private void ActivateSkill()
    {
        if (!CanUseSkill())
        {
            Debug.Log("Shield Skill đang hồi...");
            return;
        }

        lastUsedTime = Time.time;

        if (statusEffects != null)
        {
            isShieldActive = true;
            hasBlocked = false;
            statusEffects.ApplyDamageReduction(damageReduction, shieldDuration);
            Debug.Log("Shield activated! Damage reduction of " + (damageReduction * 100) + "% for " + shieldDuration + " seconds.");
        }
        else
        {
            Debug.LogError("StatusEffects component not found!");
        }
    }

    public void OnEnemyAttack(float incomingDamage)
    {
        if (isShieldActive && !hasBlocked)
        {
            hasBlocked = true;
            Debug.Log("Shield blocked the first attack!");

            // Chặn đòn đầu tiên và giảm sát thương
            if (healthBar != null)
            {
                float reducedDamage = incomingDamage * (1 - damageReduction);
                healthBar.TakeDamage(reducedDamage);
            }
            return;
        }
        else if (isShieldActive && hasBlocked)
        {
            return;  // Sau khi chặn đòn đầu tiên, không làm gì nữa
        }

        // Nếu không có shield, nhận sát thương bình thường
        if (healthBar != null)
        {
            healthBar.TakeDamage(incomingDamage);
        }
    }

    private void EndShieldEffect()
    {
        isShieldActive = false;
        hasBlocked = false;
        Debug.Log("Shield effect ended.");
    }
}
