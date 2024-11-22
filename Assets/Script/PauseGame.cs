using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseGamePanel;
    public GameObject settingsMenuUI;
    public static bool isGamePaused = false;

    public static bool isGameOver = false;
    private void Start()
    {
        isGamePaused = false;
        isGameOver = false;
        pauseGamePanel.SetActive(false);
        settingsMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            if (!isGamePaused && !InventoryManagerWar.isSelecting)
            {
                Pause();
            }
            else
            {
                Continue();
            }
        }
    }

    public void Pause()
    {
        if (isGameOver) return;

        pauseGamePanel.SetActive(true);
        Time.timeScale = 0;
        isGamePaused = true;
    }

    public void Continue()
    {
        pauseGamePanel.SetActive(false);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;

        Stamina stamina = FindObjectOfType<Stamina>();
        if (stamina != null)
        {
            stamina.ResetStaminaValues();
        }
    }

    public void OpenSettings()
    {
        pauseGamePanel.SetActive(false); 
        settingsMenuUI.SetActive(true); 
    }

    public void CloseSettings()
    {
        settingsMenuUI.SetActive(false);
        pauseGamePanel.SetActive(true);
    }

    public void MainMenu()
    {
        isGamePaused = false;
        isGameOver = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
