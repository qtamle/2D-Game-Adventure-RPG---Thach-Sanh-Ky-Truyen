using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    public GameObject interactionUI;
    public SaveManager saveManager;
    public Transform player;

    private bool isPlayerNearby = false;

    private void Start()
    {
        interactionUI.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.Q))
        {
            int saveSlot = MainMenuSL.selectedSaveSlot;

            // Lưu vào save slot được chỉ định
            saveManager.SaveGame(player.position, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, saveSlot);
            Debug.Log($"Game đã được lưu vào slot {saveSlot}!");
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
}
