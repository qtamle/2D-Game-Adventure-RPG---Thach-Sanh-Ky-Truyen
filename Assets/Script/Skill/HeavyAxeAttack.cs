using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAxeAttack : MonoBehaviour
{
    private Attack attackScript; // Truy cập component Attack
    public float damage = 50f; // Sát thương riêng của Heavy Axe Attack
    public float cooldown = 20f;  // Thời gian hồi chiêu

    private float lastUsedTime;

    private void Start()
    {
        // Lấy component Attack từ GameObject (nếu có)
        attackScript = GetComponent<Attack>();
    }

    private void Update()
    {
        // Kiểm tra khi nhấn nút số 3 và nếu chiêu có thể được thực hiện
        if (Input.GetKeyDown(KeyCode.Alpha3) && CanUseSkill())
        {
            ActivateSkill();
        }
    }

    private bool CanUseSkill()
    {
        // Kiểm tra nếu thời gian hiện tại lớn hơn thời gian sử dụng lần cuối cộng với cooldown
        return Time.time >= lastUsedTime + cooldown;
    }

    private void ActivateSkill()
    {
        if (!CanUseSkill())
        {
            Debug.Log("Heavy Axe Attack đang hồi..."); // Thông báo khi chiêu đang hồi
            return;
        }

        lastUsedTime = Time.time;

        // Thực hiện đòn tấn công độc lập với sát thương riêng của Heavy Axe Attack
        PerformHeavyAxeAttack();

        // Gọi thông báo khi chiêu bắt đầu hồi lại
        Debug.Log("Heavy Axe Attack đang hồi...");
    }

    private void PerformHeavyAxeAttack()
    {
        // Thực hiện đòn tấn công với sát thương riêng
        if (attackScript != null)
        {
            // Thực hiện đòn đánh
            Debug.Log("Executing Heavy Axe Attack with " + damage + " damage.");
            // Bạn có thể gọi một hàm trong Attack script để thực hiện hành động tấn công tại đây.
            // Ví dụ: attackScript.ExecuteAttack(damage);
        }
        else
        {
            Debug.LogError("Attack script not found!");
        }
    }
}
