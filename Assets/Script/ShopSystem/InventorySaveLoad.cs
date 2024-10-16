using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InventorySaveLoad : MonoBehaviour
{
    private static string folderPath = Application.dataPath + "/saveload";
    private static string saveFilePath = folderPath + "/inventory.json";

    // save
    public static void SaveInventory(InventoryManager inventoryManager)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Created folder: " + folderPath);
        }

        string json = JsonUtility.ToJson(inventoryManager, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Inventory saved to: " + saveFilePath);
    }

    // load
    public static void LoadInventory(InventoryManager inventoryManager)
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            Debug.Log("Loaded JSON: " + json);

            InventoryData data = JsonUtility.FromJson<InventoryData>(json);
            inventoryManager.inventoryList = data.inventoryList;

            foreach (var item in inventoryManager.inventoryList)
            {
                Debug.Log($"ItemID: {item.itemID}, Quantity: {item.quantity}");
            }

            Debug.Log("Inventory loaded from: " + saveFilePath);
        }
        else
        {
            Debug.LogWarning("Save file not found at: " + saveFilePath);
        }
    }


    [System.Serializable]
    public class InventoryData
    {
        public List<InventoryItem> inventoryList;
    }


}
