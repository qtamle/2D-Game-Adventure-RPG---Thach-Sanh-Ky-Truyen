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

        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath); // Xóa file lưu game
            Debug.Log("Đã xóa dữ liệu trong slot " + saveSlot);

            // Xóa thư mục nếu không còn file nào trong đó
            string folderPath = Application.dataPath + "/savedata/saveslot" + saveSlot;
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
    }
}
