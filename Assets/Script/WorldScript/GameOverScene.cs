using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScene : MonoBehaviour
{
    public Image blackBackground;
    public GameObject[] gameOverButtons;
    public TextMeshProUGUI gameOverText;
    public Canvas[] hiddenCanvases;
    public Button[] objectsToHideOnRestart;
    private void Start()
    {
        blackBackground.gameObject.SetActive(false);
        SetButtonsAlpha(0);
        gameOverText.color = new Color(gameOverText.color.r, gameOverText.color.g, gameOverText.color.b, 0);

        foreach (Button button in objectsToHideOnRestart)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    public void TriggerGameOver()
    {
        PauseGame.isGameOver = true;

        blackBackground.gameObject.SetActive(true);
        StartCoroutine(FadeInBackground());

        foreach (Canvas canvas in hiddenCanvases)
        {
            canvas.enabled = false;
        }


        foreach (Button button in objectsToHideOnRestart)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }

        Time.timeScale = 0;
    }

    private IEnumerator FadeInBackground()
    {
        blackBackground.color = new Color(0, 0, 0, 0);
        float duration = 2f;
        float elapsedTime = 0f;

        // Fade-in background
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            blackBackground.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        blackBackground.color = new Color(0, 0, 0, 1);


        StartCoroutine(FadeInButtonsAndText());
    }

    private IEnumerator FadeInButtonsAndText()
    {
        float duration = 1.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            gameOverText.color = new Color(gameOverText.color.r, gameOverText.color.g, gameOverText.color.b, alpha);
            SetButtonsAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameOverText.color = new Color(gameOverText.color.r, gameOverText.color.g, gameOverText.color.b, 1);
        SetButtonsAlpha(1);
    }

    public void OnPlayAgainButtonPressed()
    {
        StartCoroutine(ReloadSceneAndShowCanvases());
    }

    private IEnumerator ReloadSceneAndShowCanvases()
    {
        PauseGame.isGameOver = false;

        yield return new WaitForSeconds(1f);

        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        yield return new WaitForSeconds(1f);

        foreach (Button button in objectsToHideOnRestart)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }

        foreach (Canvas canvas in hiddenCanvases)
        {
            canvas.enabled = true;
        }

        Time.timeScale = 1;
    }

    private IEnumerator FadeOutBackground()
    {
        float duration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            blackBackground.color = new Color(0, 0, 0, alpha);
            gameOverText.color = new Color(gameOverText.color.r, gameOverText.color.g, gameOverText.color.b, alpha);
            SetButtonsAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        blackBackground.color = new Color(0, 0, 0, 0);
        blackBackground.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
    }

    private void SetButtonsAlpha(float alpha)
    {
        foreach (GameObject button in gameOverButtons)
        {
            CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = button.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = alpha;
            button.SetActive(alpha > 0);
        }
    }
}

