using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseGamePanel;

    public bool isPauseGame = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPauseGame)
        {
            Debug.Log("Escape key pressed");
            Pause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPauseGame)
        {
            Continue();
        }
    }

    public void Pause()
    {
        isPauseGame = true;
        pauseGamePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Continue()
    {
        isPauseGame = false;
        pauseGamePanel.SetActive(false);
        Time.timeScale = 1;
    }
}
