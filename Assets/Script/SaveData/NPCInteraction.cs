using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    public GameObject interactionUI;
    public GameObject saveSuccessUI;
    public SaveManager saveManager;
    public Transform player;

    private bool isPlayerNearby = false;

    private void Start()
    {
        interactionUI.SetActive(false);
        saveSuccessUI.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.Q))
        {
            SaveGameFromInteraction();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactionUI.SetActive(true);
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactionUI.SetActive(false);
            isPlayerNearby = false;
        }
    }

    // Hàm gọi từ script khác
    public void SaveGameExternally()
    {
        SaveGameFromInteraction();
    }

    // Hàm lưu game và hiển thị thông báo
    private void SaveGameFromInteraction()
    {
        int saveSlot = MainMenuSL.selectedSaveSlot;

        // Lưu vào save slot được chỉ định
        saveManager.SaveGame(player.position, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, saveSlot);
        Debug.Log($"Game đã được lưu vào slot {saveSlot}!");

        StartCoroutine(ShowSaveSuccessMessage());
    }

    private IEnumerator ShowSaveSuccessMessage()
    {
        saveSuccessUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        saveSuccessUI.SetActive(false);
    }
}
