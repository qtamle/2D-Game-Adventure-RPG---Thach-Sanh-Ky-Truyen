using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBoss : MonoBehaviour
{
    private SaveBoss saveboss;

    // Các tên boss
    public string bossName;
    public string bossName1;
    public string bossName2;
    public string bossName3;
    public string bossName4;

    // Các GameObject riêng biệt cho từng boss
    public GameObject[] hideObjects;
    public GameObject[] showObjects;

    public GameObject[] hideObjects1;
    public GameObject[] showObjects1;

    public GameObject[] hideObjects2;
    public GameObject[] showObjects2;

    public GameObject[] hideObjects3;
    public GameObject[] showObjects3;

    public GameObject[] hideObjects4;
    public GameObject[] showObjects4;

    // Default objects để hiển thị khi không có boss nào bị đánh bại
    public GameObject[] defaultHideObjects;
    public GameObject[] defaultShowObjects;

    private void Start()
    {
        saveboss = FindObjectOfType<SaveBoss>();

        bool atLeastOneBossHandled = false;

        // Kiểm tra trạng thái của từng boss và xử lý
        atLeastOneBossHandled |= CheckAndHandleBoss(bossName, hideObjects, showObjects);
        atLeastOneBossHandled |= CheckAndHandleBoss(bossName1, hideObjects1, showObjects1);
        atLeastOneBossHandled |= CheckAndHandleBoss(bossName2, hideObjects2, showObjects2);
        atLeastOneBossHandled |= CheckAndHandleBoss(bossName3, hideObjects3, showObjects3);
        atLeastOneBossHandled |= CheckAndHandleBoss(bossName4, hideObjects4, showObjects4);

        // Chỉ xử lý đối tượng mặc định nếu không có boss nào đã bị xử lý
        if (!atLeastOneBossHandled)
        {
            HandleDefaultObjects();
        }
    }

    // Kiểm tra trạng thái của từng boss và chỉ hiển thị/ẩn các đối tượng liên quan
    private bool CheckAndHandleBoss(string currentBossName, GameObject[] currentHideObjects, GameObject[] currentShowObjects)
    {
        if (saveboss == null || string.IsNullOrEmpty(currentBossName))
        {
            Debug.LogWarning("SaveBoss or BossName is null. Skipping boss check.");
            return false;
        }

        bool isDefeated = saveboss.IsBossDefeated(currentBossName);

        if (isDefeated)
        {
            // Boss đã bị đánh bại: Ẩn các đối tượng cần hide và hiển thị các đối tượng cần show
            foreach (GameObject go in currentHideObjects)
            {
                go.SetActive(false);
            }

            foreach (GameObject go in currentShowObjects)
            {
                go.SetActive(true);
            }

            Debug.Log($"Boss {currentBossName} defeated. Handled specific objects.");
            return true; // Xác nhận rằng boss này đã được xử lý
        }

        // Nếu boss chưa bị đánh bại, không làm gì
        return false;
    }

    // Kiểm tra nếu không có boss nào bị đánh bại
    private bool NoBossDefeated()
    {
        return !saveboss.IsBossDefeated(bossName) &&
               !saveboss.IsBossDefeated(bossName1) &&
               !saveboss.IsBossDefeated(bossName2) &&
               !saveboss.IsBossDefeated(bossName3) &&
               !saveboss.IsBossDefeated(bossName4);
    }

    // Hàm xử lý các đối tượng mặc định
    private void HandleDefaultObjects()
    {
        foreach (GameObject go in defaultHideObjects)
        {
            go.SetActive(false); // Hiển thị đối tượng mặc định
        }

        foreach (GameObject go in defaultShowObjects)
        {
            go.SetActive(true); // Ẩn các đối tượng khác
        }
    }
}