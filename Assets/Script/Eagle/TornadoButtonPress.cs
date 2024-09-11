using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TornadoButtonPress : MonoBehaviour
{
    public Canvas canvas;  
    public Button buttonPrefab;  
    public string[] buttons = {"V", "B", "N", "M" };
    public float tornadoDuration = 5f;
    private string correctButton;
    private Button activeButton;
    private RectTransform buttonTransform;
    private bool isButtonPressedCorrectly = false;
    private float buttonPressTime = 0.1f;
    private float nextButtonPress = 0f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public IEnumerator StartButtonPress()
    {
        correctButton = buttons[Random.Range(0, buttons.Length)];

        activeButton = Instantiate(buttonPrefab, canvas.transform);
        buttonTransform = activeButton.GetComponent<RectTransform>();

        Text buttonText = activeButton.GetComponentInChildren<Text>();
        buttonText.text = correctButton.ToUpper();

        float elapsedTime = 0f;

        while (elapsedTime < tornadoDuration)
        {
            elapsedTime += Time.deltaTime;

            Vector3 playerWorldPosition = transform.position + Vector3.up * 2;  
            Vector3 playerScreenPosition = mainCamera.WorldToScreenPoint(playerWorldPosition);  

            buttonTransform.position = playerScreenPosition;

            // Kiểm tra liên tục nếu nhấn đúng nút
            if (Input.GetKey(correctButton.ToLower()))
            {
                if (Time.time > nextButtonPress)
                {
                    isButtonPressedCorrectly = true;
                    nextButtonPress = Time.time + buttonPressTime;
                }
            }
            else
            {
                isButtonPressedCorrectly = false;
            }

            yield return null;
        }

        if (activeButton != null)
        {
            activeButton.gameObject.SetActive(false);
        }
    }

    public bool IsButtonPressedCorrectly()
    {
        return isButtonPressedCorrectly;
    }
}
