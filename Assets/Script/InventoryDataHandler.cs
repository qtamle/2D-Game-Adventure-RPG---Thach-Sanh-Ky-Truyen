using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InventoryDataHandler : MonoBehaviour
{
    private static string folderPath = Application.persistentDataPath + "/saveload";
    private static string saveFilePath = folderPath + "/inventory.json";

    // Lưu inventory ra JSON
    public static void SaveInventory(List<PotionData> potions)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        InventoryData data = new InventoryData { potions = potions };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

    // Load inventory từ JSON
    public static List<PotionData> LoadInventory()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            InventoryData data = JsonUtility.FromJson<InventoryData>(json);
            return data.potions;
        }
        else
        {
            Debug.LogWarning("Save file not found, returning empty inventory.");
            return new List<PotionData>();
        }
    }

    [System.Serializable]
    public class InventoryData
    {
        public List<PotionData> potions;
    }

    [System.Serializable]
    public class PotionData
    {
        public int id;
        public int quantity;
    }
}
