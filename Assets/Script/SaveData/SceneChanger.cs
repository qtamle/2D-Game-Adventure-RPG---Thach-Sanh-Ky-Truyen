using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string targetScene;  // Tên của scene sẽ chuyển đến

    // Chuyển scene khi Player va chạm
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadTargetScene();
        }
    }

    // Hàm chuyển scene khi ấn button
    public void LoadSceneFromButton()
    {
        LoadTargetScene();
    }

    // Hàm dùng chung để chuyển scene
    private void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetScene))
        {
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("Tên scene chưa được thiết lập!");
        }
    }
}
