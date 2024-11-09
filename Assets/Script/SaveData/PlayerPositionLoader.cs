using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionLoader : MonoBehaviour
{
    public SaveManager saveManager;
    public GameObject player;

    private void Start()
    {
        int saveSlot = MainMenuSL.selectedSaveSlot; // Lấy saveSlot từ Menu
        SaveData data = saveManager.LoadGame(saveSlot);

        if (data != null)
        {
            player.transform.position = data.playerPosition;
        }
    }
}
