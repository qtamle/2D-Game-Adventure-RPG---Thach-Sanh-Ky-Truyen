using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseGame : MonoBehaviour
{
    public GameObject pauseGamePanel;

    public bool isPauseGame = false;

    private void Start()
    {
        isPauseGame = false;
        Time.timeScale = 1;
        pauseGamePanel.SetActive(false);
    }
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

    public void MainMenu()
    {
        isPauseGame = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
