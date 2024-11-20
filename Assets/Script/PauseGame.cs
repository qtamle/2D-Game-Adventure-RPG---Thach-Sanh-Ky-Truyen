using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseGamePanel;
    public static bool isGamePaused = false;

    public static bool isGameOver = false;
    private void Start()
    {
        isGamePaused = false;
        isGameOver = false;
        pauseGamePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
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
        if (isGameOver) return;

        isGamePaused = true;
        pauseGamePanel.SetActive(true);
        Time.timeScale = 0;
        Debug.Log("Game paused");
    }

    public void Continue()
    {
        isGamePaused = false;
        pauseGamePanel.SetActive(false);
        Debug.Log("Game continued");
    }

    public void MainMenu()
    {
        isGamePaused = false;
        isGameOver = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
