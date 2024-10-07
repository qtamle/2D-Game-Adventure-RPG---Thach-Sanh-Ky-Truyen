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

        // Quantity
        shopItems[3, 1] = 0;
        shopItems[3, 2] = 0;
        shopItems[3, 3] = 0;
        shopItems[3, 4] = 0;
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


}
