using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public int itemID;
    public Text quantityTxt;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }
    public void UpdateQuantity()
    {
        int quantity = inventoryManager.GetItemQuantity(itemID);
        Debug.Log("ItemID: " + itemID + ", Quantity: " + quantity);
        if (quantityTxt != null)
        {
            quantityTxt.text = quantity.ToString();
        }
        else
        {
            Debug.LogWarning("quantityTxt is not assigned in ItemSlot."); 
        }
    }

}
