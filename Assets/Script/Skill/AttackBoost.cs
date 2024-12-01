using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBoost : MonoBehaviour
{
    private Attack attackScript; // Truy cập component Attack
    public float damageMultiplier = 1.1f;
    public float boostDuration = 5f;
    public float cooldown = 10f;

    private float lastUsedTime;

    private void Start()
    {
        // Lấy component Attack từ GameObject
        attackScript = GetComponent<Attack>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && CanUseSkill())
        {
            ActivateSkill();
        }
    }

    private bool CanUseSkill()
    {
        return Time.time >= lastUsedTime + cooldown;
    }

    private void ActivateSkill()
    {
        lastUsedTime = Time.time;

        // Tăng sát thương của nhân vật
        if (attackScript != null)
        {
            attackScript.damage *= damageMultiplier;
            Debug.Log("Attack Boost activated!");
        }

        // Reset lại sát thương sau thời gian boost
        StartCoroutine(ResetDamage());
    }

    private IEnumerator ResetDamage()
    {
        yield return new WaitForSeconds(boostDuration);
        if (attackScript != null)
        {
            attackScript.damage /= damageMultiplier;
            Debug.Log("Attack Boost ended!");
        }
    }
}

