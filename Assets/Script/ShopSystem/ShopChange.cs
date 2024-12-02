using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopChange : MonoBehaviour
{
    public string targetScene;
    public GameObject promptSprite;
    private bool playerIsNearby = false;

    public NPCInteraction npc;
    public PlayerPositionLoader playerPositionLoader;
    private void Start()
    {
        if (promptSprite != null)
        {
            promptSprite.SetActive(false);
        }
    }

    // Khi Player vào vùng va chạm
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNearby = true;
            if (promptSprite != null)
            {
                promptSprite.SetActive(true);
            }
        }
    }

    // Khi Player rời khỏi vùng va chạm
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNearby = false;
            if (promptSprite != null)
            {
                promptSprite.SetActive(false);
            }
        }
    }

    // Hàm chuyển scene khi ấn button
    public void LoadSceneFromButton()
    {
        LoadTargetScene();
    }

    // Update kiểm tra phím E
    private void Update()
    {
        if (playerIsNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (npc != null)
            {
                npc.SaveGameExternally();
            }
            LoadTargetScene();
        }
    }

    // Hàm dùng chung để chuyển scene
    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetScene))
        {
            if (playerPositionLoader != null)
            {
                playerPositionLoader.LoadData();
            }
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("Tên scene chưa được thiết lập!");
        }
    }
}
