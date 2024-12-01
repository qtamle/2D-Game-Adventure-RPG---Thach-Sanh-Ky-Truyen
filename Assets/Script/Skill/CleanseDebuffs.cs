using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanseDebuffs : MonoBehaviour
{
    private StatusEffects statusEffects; // Truy cập component StatusEffects
    public float cooldown = 30f; // Thời gian hồi chiêu

    private float lastUsedTime;

    private void Start()
    {
        // Lấy component StatusEffects từ GameObject
        statusEffects = GetComponent<StatusEffects>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4) && CanUseSkill())
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

        // Dừng tất cả các hiệu ứng bất lợi
        if (statusEffects != null)
        {
            statusEffects.StopAllEffects();
            Debug.Log("Cleanse activated! All negative effects removed.");
        }
    }
}


