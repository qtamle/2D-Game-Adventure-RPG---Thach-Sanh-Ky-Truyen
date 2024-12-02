using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class ItemDescription
{
    public int itemID;
    public string description;
}

public class PopupSystem : MonoBehaviour
{
    public GameObject popUpBox;
    public Animator anim;
    public TMP_Text popUpText;
    public GameObject overlay;

    public List<ItemDescription> itemDescriptions = new List<ItemDescription>();

    private void Start()
    {
        overlay.SetActive(false);
    }

    public void ShowPopUp(int itemID)
    {
        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(1);
            }
            else
            {
                Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'AudioManager'.");
        }
        ItemDescription item = itemDescriptions.Find(i => i.itemID == itemID);

        if (item != null)
        {
            overlay.SetActive(true);
            popUpBox.SetActive(true);
            popUpText.text = item.description; 
            anim.SetTrigger("Pop"); 
        }
        else
        {
            Debug.LogWarning("Không tìm thấy mô tả cho Item ID: " + itemID);
        }
    }

    public void HidePopUp()
    {
        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(1);
            }
            else
            {
                Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'AudioManager'.");
        }
        overlay.SetActive(false);
        popUpBox.SetActive(false);
        anim.SetTrigger("Close");
    }
}
