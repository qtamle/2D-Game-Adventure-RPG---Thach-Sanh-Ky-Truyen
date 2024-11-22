using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StaminaFixed : MonoBehaviour
{
    public Image fillImage;

    private void Awake()
    {
        fillImage = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        if (fillImage != null && fillImage.color != Color.yellow)
        {
            fillImage.color = Color.yellow; 
        }
        else
        {
            Debug.Log("khong có fill");
        }
    }
}
