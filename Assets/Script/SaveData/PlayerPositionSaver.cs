using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPositionSaver : MonoBehaviour
{
    public SaveManager saveManager;
    public Transform player;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int saveSlot = MainMenuSL.selectedSaveSlot; // Lấy saveSlot từ Menu
        saveManager.SaveGame(player.position, scene.name, saveSlot); // Lưu vào slot tương ứng
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
