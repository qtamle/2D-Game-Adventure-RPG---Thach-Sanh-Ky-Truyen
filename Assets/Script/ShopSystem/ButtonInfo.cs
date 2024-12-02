using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInfo : MonoBehaviour
{
    public int ItemID;
    public Text priceTxt;
    public Text quantityTxt;
    public GameObject shopManager;

    private void Update()
    {
        priceTxt.text = "Giá: " + shopManager.GetComponent<ShopManager>().shopItems[2, ItemID].ToString();
        quantityTxt.text = shopManager.GetComponent<ShopManager>().shopItems[3, ItemID].ToString();

    }
}
