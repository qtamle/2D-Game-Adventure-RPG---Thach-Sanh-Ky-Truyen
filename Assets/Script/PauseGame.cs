using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseGamePanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed");
            Pause();
        }
    }

    public void Pause()
    {
        pauseGamePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Continue()
    {
        pauseGamePanel.SetActive(false);
        Time.timeScale = 1;
    }
}
