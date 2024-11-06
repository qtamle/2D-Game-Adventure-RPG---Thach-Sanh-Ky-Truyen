using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseGamePanel;
    public static bool isGamePaused = false;

    private void Start()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        pauseGamePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isGamePaused && !InventoryManagerWar.isSelecting)
            {
                Pause();
            }
            else if (isGamePaused)
            {
                Continue();
            }
        }
    }

    public void Pause()
    {
        isGamePaused = true;
        pauseGamePanel.SetActive(true);
        Time.timeScale = 0;
        Debug.Log("Game paused");
    }

    public void Continue()
    {
        isGamePaused = false;
        pauseGamePanel.SetActive(false);
        Time.timeScale = 1;
        Debug.Log("Game continued");
    }

    public void MainMenu()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
