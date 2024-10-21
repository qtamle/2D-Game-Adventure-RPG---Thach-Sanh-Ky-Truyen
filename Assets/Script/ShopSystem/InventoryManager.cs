using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class InventoryItem
{
    public int itemID;
    public string itemName;
    public int quantity;
}

public class InventoryManager : MonoBehaviour
{
    public List<InventoryItem> inventoryList = new List<InventoryItem>();

    public int healingMedicine = 1;
    public int energyMedicine = 2;
    public int antibodyMedicine = 3;
    public int addPointsMedicine = 4;

    private int selectedPotionID;
    private void Start()
    {
        LoadInventory();
        StartCoroutine(UpdateItemSlotsCoroutine());

        selectedPotionID = healingMedicine;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SaveInventory();
            Debug.Log("Inventory saved. File path: " + Application.persistentDataPath + "/inventory.json");
        }
    }

    public void AddItem(int itemID)
    {
        InventoryItem item = inventoryList.Find(i => i.itemID == itemID);

        if (item != null)
        {
            // Nếu item đã tồn tại, tăng số lượng
            item.quantity++;
        }
        else
        {
            // Nếu item chưa có, thêm vào danh sách
            InventoryItem newItem = new InventoryItem
            {
                itemID = itemID,
                quantity = 1,
                itemName = GetItemNameByID(itemID)
            };
            inventoryList.Add(newItem);
        }
    }

    private string GetItemNameByID(int itemID)
    {
        switch (itemID)
        {
            case 1: return "Healing Medicine";
            case 2: return "Energy Medicine";
            case 3: return "Antibody Medicine";
            case 4: return "Add Points Medicine";
            default: return "Unknown Item";
        }
    }
    public int GetItemQuantity(int itemID)
    {
        InventoryItem item = inventoryList.Find(i => i.itemID == itemID);
        int quantity = item != null ? item.quantity : 0;
        Debug.Log($"GetItemQuantity: itemID={itemID}, quantity={quantity}"); 
        return quantity;
    }

    public void SaveInventory()
    {
        InventorySaveLoad.SaveInventory(this);
    }

    public void LoadInventory()
    {
        InventorySaveLoad.LoadInventory(this);
    }

    private IEnumerator UpdateItemSlotsCoroutine()
    {
        yield return null; 
        UpdateAllItemSlots();
    }

    private void UpdateAllItemSlots()
    {
        ItemSlot[] itemSlots = FindObjectsOfType<ItemSlot>();
        Debug.Log("Found " + itemSlots.Length + " item slots.");
        foreach (ItemSlot slot in itemSlots)
        {
            slot.UpdateQuantity();
        }
    }

    public void SelectPotion(int itemID)
    {
        selectedPotionID = itemID;
        Debug.Log($"Đã chọn thuốc với ID: {itemID}");
    }
}
