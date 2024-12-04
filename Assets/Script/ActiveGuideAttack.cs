using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveGuideAttack : MonoBehaviour
{
    public GameObject canvasGuide;

    private void Start()
    {
        canvasGuide.SetActive(false);
    }

    public void ActiveGuide()
    {
        canvasGuide.SetActive(true);
    }

    public void ActiveButtonQuit()
    {
        canvasGuide.SetActive(false);
        Time.timeScale = 0f;
    }
}
