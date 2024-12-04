using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveGuide : MonoBehaviour
{
    public GameObject guideCanvas;
    public GameObject guideActive;
    public bool isActive;
    private void Start()
    {
        guideCanvas.SetActive(false);
        guideActive.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            if (isActive) 
            {
                guideCanvas.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            guideActive.SetActive(true);
            isActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        guideActive.SetActive(false);
        isActive = false;
    }

    public void HideCanvasGuide()
    {
        guideCanvas.SetActive(false);
    }
}
