using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PanelToggle : MonoBehaviour
{
    public GameObject panel; 

    void Start()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    void Update()
    {
        // Kiểm tra nếu nhấn phím Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (panel != null)
            {
                panel.SetActive(!panel.activeSelf);
            }
        }
    }
}
