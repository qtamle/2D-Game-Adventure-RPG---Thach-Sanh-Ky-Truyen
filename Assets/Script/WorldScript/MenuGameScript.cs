using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuGameScript : MonoBehaviour
{
    public GameObject loadgameCanvas;
    public GameObject settingCanvas;

    private void Start()
    {
        loadgameCanvas.SetActive(false);
        settingCanvas.SetActive(false);
    }

    public void ShowLoadGameCanvas()
    {
        loadgameCanvas.SetActive(true);
    }

    public void HideLoadGameCanvas()
    {
        loadgameCanvas.SetActive(false);
    }

    public void ShowSettingGameCanvas()
    {
        settingCanvas.SetActive(true);
    }

    public void HideSettingGameCanvas() 
    { 
        settingCanvas.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Da thoat game");
    }
}
