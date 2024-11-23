using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PauseGame : MonoBehaviour
{
    public GameObject pauseGamePanel;
    public GameObject settingsMenuUI;
    public Image blackBackground;
    public static bool isGamePaused = false;
    public static bool isGameOver = false;

    private void Start()
    {
        isGamePaused = false;
        isGameOver = false;
        pauseGamePanel.SetActive(false);
        settingsMenuUI.SetActive(false);

        // Ẩn blackBackground lúc đầu
        blackBackground.gameObject.SetActive(false);
        blackBackground.color = new Color(0, 0, 0, 0);
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

        // Hiện hiệu ứng fade in và pause game
        StartCoroutine(FadeInBackground(() =>
        {
            pauseGamePanel.SetActive(true);
            Time.timeScale = 0;
            isGamePaused = true;
        }));
    }

    public void Continue()
    {
        // Hiện hiệu ứng fade out và tiếp tục game
        StartCoroutine(FadeOutBackground(() =>
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
        }));
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

    private IEnumerator FadeInBackground(System.Action onFadeComplete)
    {
        blackBackground.gameObject.SetActive(true);
        float duration = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0, 0.5f, elapsedTime / duration); 
            blackBackground.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.unscaledDeltaTime; // Sử dụng unscaledDeltaTime để không bị ảnh hưởng bởi Time.timeScale = 0
            yield return null;
        }

        blackBackground.color = new Color(0, 0, 0, 0.5f);
        onFadeComplete?.Invoke();
    }

    private IEnumerator FadeOutBackground(System.Action onFadeComplete)
    {
        float duration = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0.7f, 0, elapsedTime / duration);
            blackBackground.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        blackBackground.color = new Color(0, 0, 0, 0);
        blackBackground.gameObject.SetActive(false);
        onFadeComplete?.Invoke();
    }
}
