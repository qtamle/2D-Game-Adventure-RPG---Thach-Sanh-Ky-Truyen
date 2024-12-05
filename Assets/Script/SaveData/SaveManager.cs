using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Hàm tạo đường dẫn tới các thư mục khác nhau cho từng slot
    private string GetSaveFilePath(int saveSlot)
    {
        string folderPath = Application.dataPath + "/savedata/saveslot" + saveSlot;
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Đã tạo thư mục lưu dữ liệu tại: " + folderPath);
        }

        return folderPath + "/savefile" + saveSlot + ".json";
    }

    // Lưu game vào file theo slot
    public void SaveGame(Vector3 playerPosition, string sceneName, int saveSlot)
    {
        SaveData data = new SaveData();
        data.playerPosition = playerPosition;
        data.currentScene = sceneName;

        string saveFilePath = GetSaveFilePath(saveSlot);
        string json = JsonUtility.ToJson(data);

        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game đã được lưu vào slot " + saveSlot + " tại: " + saveFilePath);
    }

    // Tải game từ file theo slot
    public SaveData LoadGame(int saveSlot)
    {
        string saveFilePath = GetSaveFilePath(saveSlot);

        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Đã tải dữ liệu game từ slot " + saveSlot);
            return data;
        }
        else
        {
            Debug.Log("Không tìm thấy file lưu cho slot " + saveSlot);
            return null;
        }
    }

    // Kiểm tra nếu có dữ liệu lưu trong slot
    public bool HasSaveData(int saveSlot)
    {
        string saveFilePath = GetSaveFilePath(saveSlot);
        return File.Exists(saveFilePath);
    }

    // Xóa dữ liệu trong slot lưu
    public void DeleteSaveData(int saveSlot)
    {
        string saveFilePath = GetSaveFilePath(saveSlot);

        // Delete the game save file
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath); // Delete the save file
            Debug.Log("Đã xóa dữ liệu trong slot " + saveSlot);

            // Check if the folder is empty and delete it
            string folderPath = Path.Combine(Application.dataPath, "savedata", "saveslot" + saveSlot);
            if (Directory.GetFiles(folderPath).Length == 0)
            {
                Directory.Delete(folderPath);
                Debug.Log("Đã xóa thư mục lưu dữ liệu tại: " + folderPath);
            }
        }
        else
        {
            Debug.Log("Không có dữ liệu để xóa trong slot " + saveSlot);
        }

        // Delete specific files in saveload (excluding 'settings')
        string saveLoadPath = Path.Combine(Application.dataPath, "saveload");
        string[] filesToDeleteInSaveLoad = { "coins.json", "inventory.json" };

        foreach (string fileName in filesToDeleteInSaveLoad)
        {
            string filePath = Path.Combine(saveLoadPath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("Đã xóa file: " + fileName);
            }
            else
            {
                Debug.Log("Không tìm thấy file: " + fileName + " để xóa.");
            }
        }

        // Delete the timelineData file in saveTimeline
        string saveTimelinePath = Path.Combine(Application.dataPath, "saveTimeline");
        string timelineDataFile = Path.Combine(saveTimelinePath, "timelineData.json");

        if (File.Exists(timelineDataFile))
        {
            File.Delete(timelineDataFile);
            Debug.Log("Đã xóa file timelineData trong thư mục saveTimeline.");
        }
        else
        {
            Debug.Log("Không tìm thấy file timelineData trong thư mục saveTimeline để xóa.");
        }

        string saveBossPath = Path.Combine(Application.dataPath, "SaveBoss");
        string BossDataFile = Path.Combine(saveBossPath, "boss_save.json");

        if (File.Exists(BossDataFile))
        {
            File.Delete(BossDataFile);
            Debug.Log("Đã xóa file saveboss trong thư mục saveBoss.");
        }
        else
        {
            Debug.Log("Không tìm thấy file saveBoss trong thư mục saveBoss để xóa.");
        }
    }
}
