using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManagerWar : MonoBehaviour
{
    public GameObject potionSlotPanel;
    public Image selectedPotionIcon;
    public Button[] potionButtons;
    public Sprite[] potionSprites;
    public Image[] hideListImage;

    public Text quantityText;
    public InventoryManager inventoryManager;

    private bool isSelecting = false;
    private float targetTimeScale = 1f;
    private float smoothSpeed = 5f;
    private int selectedPotionID = 0;

    private ItemFunction itemFunction;

    private void Start()
    {
        foreach (Image img in hideListImage)
        {
            img.gameObject.SetActive(false);
        }
        potionSlotPanel.SetActive(false);
        inventoryManager.LoadInventory();

        SelectDefaultPotion();

        for (int i = 0; i < Mathf.Min(potionButtons.Length, inventoryManager.inventoryList.Count); i++)
        {
            int index = i;
            potionButtons[i].onClick.AddListener(() => SelectPotion(index));
        }

        itemFunction = FindObjectOfType<ItemFunction>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            ShowPotionPanel();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            HidePotionPanel();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            UseSelectedPotion(); 
        }

        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, smoothSpeed * Time.unscaledDeltaTime);
    }

    private void ShowPotionPanel()
    {
        foreach (Image img in hideListImage)
        {
            img.gameObject.SetActive(true);
        }
        isSelecting = true;
        potionSlotPanel.SetActive(true);
        Time.timeScale = 0.1f;
    }

    private void HidePotionPanel()
    {
        foreach (Image img in hideListImage)
        {
            img.gameObject.SetActive(false);
        }
        isSelecting = false;
        potionSlotPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void SelectDefaultPotion()
    {
        int healingPotionIndex = 0;
        SelectPotion(healingPotionIndex);
    }


    private void SelectPotion(int index)
    {
        if (index < 0 || index >= inventoryManager.inventoryList.Count)
        {
            Debug.LogWarning("Index out of range: " + index);
            return; 
        }

        int itemID = inventoryManager.inventoryList[index].itemID;

        InventoryItem selectedItem = inventoryManager.inventoryList.Find(item => item.itemID == itemID);

        if (selectedItem != null)
        {
            quantityText.text = $"{selectedItem.quantity}";
            selectedPotionIcon.sprite = potionSprites[index];
            selectedPotionIcon.gameObject.SetActive(true);

            selectedPotionID = selectedItem.itemID;
        }
        else
        {
            quantityText.text = "0"; 
            selectedPotionIcon.sprite = potionSprites[index]; 
            selectedPotionIcon.gameObject.SetActive(true);
            selectedPotionID = itemID;
            Debug.LogWarning($"Thuốc với ID {itemID} không tồn tại, đặt số lượng về 0.");
        }
    }
    private void UseSelectedPotion()
    {
        if (itemFunction != null)
        {
            InventoryItem selectedItem = inventoryManager.inventoryList.Find(item => item.itemID == selectedPotionID);

            if (selectedItem != null && selectedItem.quantity > 0)
            {
                switch (selectedPotionID) 
                {
                    case 1:
                        itemFunction.UseHealthItem();
                        break;
                    case 2:
                        itemFunction.UseStaminaItem(itemFunction.GetComponent<Stamina>()); // Sử dụng thuốc hồi stamina
                        break;
                    case 3:
                        itemFunction.UseImmunityItem();
                        break;
                    case 4:
                        Debug.Log("Chưa có chức năng");
                        break;
                    default:
                        Debug.LogWarning("Unknown potion ID!");
                        break;
                }
                selectedItem.quantity--;

                inventoryManager.SaveInventory();

                if (selectedItem.quantity <= 0)
                {
                    Debug.Log($"{selectedItem.itemName} đã hết!");
                }
                quantityText.text = $"{selectedItem.quantity}";
            }
            else
            {
                Debug.LogWarning("Không thể sử dụng thuốc, số lượng bằng 0 hoặc không tìm thấy!");
            }
        }
    }
}
