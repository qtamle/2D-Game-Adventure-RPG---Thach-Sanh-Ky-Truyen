using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class BossData
{
    public string bossName; // Tên của boss
    public bool isDefeated; // Trạng thái đánh bại
}

[System.Serializable]
public class SaveDataBoss
{
    public List<BossData> bosses = new List<BossData>();

}

public class SaveBoss : MonoBehaviour
{
    private string savePath; 
    private SaveDataBoss saveData;
    public List<string> defaultBossList = new List<string>(); // Danh sách trùm mặc định

    private void Awake()
    {
        // Đặt đường dẫn lưu file vào thư mục SaveBoss trong Assets
        string folderPath = Path.Combine(Application.dataPath, "SaveBoss");

        // Tạo thư mục nếu chưa tồn tại
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Created SaveBoss folder at: " + folderPath);
        }

        savePath = Path.Combine(folderPath, "boss_save.json");

        LoadData(); // Tải dữ liệu khi game khởi động
    }
    // Thêm tất cả trùm mặc định nếu chưa tồn tại
    private void InitializeDefaultBosses()
    {
        foreach (string bossName in defaultBossList)
        {
            if (!saveData.bosses.Exists(b => b.bossName == bossName))
            {
                saveData.bosses.Add(new BossData { bossName = bossName, isDefeated = false });
            }
        }

        SaveDataToFile(); // Lưu lại danh sách cập nhật
    }
    public void PrintBossStates()
    {
        Debug.Log("=== Trạng thái các trùm ===");
        foreach (var boss in saveData.bosses)
        {
            Debug.Log($"Trùm: {boss.bossName}, Đã bị đánh bại: {boss.isDefeated}");
        }
        Debug.Log("==========================");
    }
    // Kiểm tra xem tên trùm có tồn tại trong danh sách lưu trữ hay không
    public bool IsBossExist(string bossName)
    {
        return saveData.bosses.Exists(b => b.bossName == bossName);
    }

    // Hàm dùng để đánh dấu boss bị đánh bại
    public void MarkBossAsDefeated(string bossName)
    {
        BossData boss = saveData.bosses.Find(b => b.bossName == bossName);
        if (boss == null)
        {
            // Nếu chưa có trong danh sách, thêm mới
            boss = new BossData { bossName = bossName, isDefeated = true };
            saveData.bosses.Add(boss);
        }
        else
        {
            // Cập nhật trạng thái
            boss.isDefeated = true;
        }

        SaveDataToFile();
    }

    // Kiểm tra boss đã bị đánh bại chưa
    public bool IsBossDefeated(string bossName)
    {
        BossData boss = saveData.bosses.Find(b => b.bossName == bossName);
        return boss != null && boss.isDefeated;
    }

    // Lưu dữ liệu ra file JSON
    private void SaveDataToFile()
    {
        string json = JsonUtility.ToJson(saveData, true); 
        File.WriteAllText(savePath, json);
        Debug.Log("Data saved to: " + savePath);
    }

    // Tải dữ liệu từ file JSON
    private void LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<SaveDataBoss>(json); 
        }
        else
        {
            saveData = new SaveDataBoss(); 
        }
    }
}
