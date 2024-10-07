using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tạo một class để đại diện cho item và số lượng của nó
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
            InventoryItem newItem = new InventoryItem();
            newItem.itemID = itemID;
            newItem.quantity = 1;
            newItem.itemName = GetItemNameByID(itemID); 
            inventoryList.Add(newItem);
        }
    }

    // Hàm để tìm tên vật phẩm dựa trên ID
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

    // Hàm để kiểm tra số lượng của một vật phẩm
    public int GetItemQuantity(int itemID)
    {
        InventoryItem item = inventoryList.Find(i => i.itemID == itemID);
        return item != null ? item.quantity : 0;
    }

}
