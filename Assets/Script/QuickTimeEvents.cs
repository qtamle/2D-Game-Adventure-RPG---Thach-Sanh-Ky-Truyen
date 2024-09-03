using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickTimeEvents : MonoBehaviour
{
    public static QuickTimeEvents Instance { get; private set; } // Singleton Instance

    public Button[] keybutton;
    public Text[] txt;
    private string[] keys = { "W", "A", "S", "D", "Q", "E", "R" };
    private int currentKeyIndex = 0;
    private List<string> selectedKeys = new List<string>();

    public BoxCollider2D keyAreaCollider;
    private bool isActive = false;

    public delegate void AllKeysPressedHandler();
    public event AllKeysPressedHandler OnAllKeysPressed;
    public bool IsActive => isActive;

    private HealthBar playerHealth;
    public float wrongKeyDamage = 10f; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GenerateRandomKeys();
        HideAllKeys();

        playerHealth = FindObjectOfType<HealthBar>();
    }

    private void Update()
    {
        if (isActive && currentKeyIndex < selectedKeys.Count)
        {
            string currentKey = txt[currentKeyIndex].text;

            if (Input.GetKeyDown(currentKey.ToLower()))
            {
                Debug.Log("ấn thành công phím: " + currentKey);

                keybutton[currentKeyIndex].gameObject.SetActive(false);
                currentKeyIndex++;
                ShowNextKey();

                // Kiểm tra xem đã ấn đủ 4 phím chưa
                if (currentKeyIndex >= selectedKeys.Count)
                {
                    StopQuickTimeEvents();
                    OnAllKeysPressed?.Invoke(); // Gọi sự kiện khi đã ấn đủ 4 phím
                }
            }
            else if (Input.anyKeyDown && !IsMouseInput())
            {
                string pressedKey = Input.inputString.ToLower();
                if (pressedKey != currentKey.ToLower())
                {
                    OnWrongKeyPressed();
                }
            }
        }
    }

    private bool IsMouseInput()
    {
        // Check if the input is coming from a mouse button
        return Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2);
    }

    public void StopQuickTimeEvents()
    {
        isActive = false;
        HideAllKeys();
        Debug.Log("Quá trình ấn phím bị dừng lại");
    }

    public void GenerateRandomKeys()
    {
        Vector2 areaPosition = keyAreaCollider.bounds.min;
        Vector2 areaSize = keyAreaCollider.bounds.size;

        selectedKeys.Clear(); // Xóa danh sách phím cũ

        for (int i = 0; i < 4; i++)
        {
            string randomKey;
            do
            {
                randomKey = keys[Random.Range(0, keys.Length)];
            }
            while (selectedKeys.Contains(randomKey));

            selectedKeys.Add(randomKey);
            txt[i].text = randomKey;

            float randomX = Random.Range(areaPosition.x, areaPosition.x + areaSize.x);
            float randomY = Random.Range(areaPosition.y, areaPosition.y + areaSize.y);
            keybutton[i].transform.position = new Vector2(randomX, randomY);
        }
    }

    public void HideAllKeys()
    {
        foreach (Button btn in keybutton)
        {
            btn.gameObject.SetActive(false);
        }
    }

    public void ShowKeyPrompts()
    {
        HideAllKeys();
        currentKeyIndex = 0;
        ShowNextKey();
        isActive = true;
    }

    public void ShowNextKey()
    {
        if (currentKeyIndex < selectedKeys.Count)
        {
            keybutton[currentKeyIndex].gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("hoàn thành");
            StopQuickTimeEvents();
            OnAllKeysPressed?.Invoke();
        }
    }

    public void HideKeyPrompts()
    {
        HideAllKeys();
        isActive = false;
    }

    public void OnWrongKeyPressed()
    {
        Debug.Log("Nhấn sai phím");

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(wrongKeyDamage);
        }

        keybutton[currentKeyIndex].gameObject.SetActive(false);
        currentKeyIndex++;
        ShowNextKey();
    }
}
