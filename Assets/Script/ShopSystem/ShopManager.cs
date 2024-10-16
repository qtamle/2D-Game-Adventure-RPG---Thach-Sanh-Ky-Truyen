using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public int[,] shopItems = new int[5,5];
    public float coins;
    public Text coinsTxt;

    public InventoryManager inventoryManager;
    private InventorySaveLoad saveLoad;

    private void Start()
    {
        coinsTxt.text = "Coins: " + coins.ToString();

        // ID
        shopItems[1, 1] = 1;
        shopItems[1, 2] = 2;
        shopItems[1, 3] = 3;
        shopItems[1, 4] = 4;

        // Price
        shopItems[2, 1] = 100;
        shopItems[2, 2] = 100;
        shopItems[2, 3] = 300;
        shopItems[2, 4] = 150;

        inventoryManager.LoadInventory();

        UpdateShopQuantitiesFromInventory();

        StartCoroutine(UpdateItemSlotsCoroutine());
    }

    public void Buy()
    {
        GameObject buttonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;

        if (buttonRef == null)
        {
            Debug.LogError("No button selected.");
            return;
        }

        ButtonInfo buttonInfo = buttonRef.GetComponent<ButtonInfo>();

        if (buttonInfo == null)
        {
            Debug.LogError("ButtonInfo component not found on the selected button.");
            return;
        }

        int itemID = buttonInfo.ItemID;
        if (coins >= shopItems[2, itemID])
        {
            coins -= shopItems[2, itemID];
            shopItems[3, itemID]++;

            coinsTxt.text = "Coins: " + coins.ToString();
            buttonInfo.quantityTxt.text = shopItems[3, itemID].ToString();

            inventoryManager.AddItem(itemID);
        }
        else
        {
            Debug.Log("Not enough coins.");
        }
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
    private void UpdateShopQuantitiesFromInventory()
    {
        for (int itemID = 1; itemID <= 4; itemID++)
        {
            int quantity = inventoryManager.GetItemQuantity(itemID);
            shopItems[3, itemID] = quantity; // Cập nhật số lượng trong shopItems

            // Cập nhật UI cho nút
            ButtonInfo buttonInfo = FindButtonInfoByID(itemID);
            if (buttonInfo != null)
            {
                buttonInfo.quantityTxt.text = quantity.ToString();
            }
        }
    }

    private ButtonInfo FindButtonInfoByID(int itemID)
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            ButtonInfo buttonInfo = button.GetComponent<ButtonInfo>();
            if (buttonInfo != null && buttonInfo.ItemID == itemID)
            {
                return buttonInfo; 
            }
        }
        return null; 
    }

}
