using UnityEngine;

public class Check : MonoBehaviour
{
    private Transform player;
    private SpriteRenderer spriteRenderer;
    public DialogueManager dialogueManager; // Biến tham chiếu tới DialogueManager
    private bool isActivated = false; // Biến để kiểm tra kích hoạt khu vực
    private bool isDialogueActive = false; // Biến kiểm tra hội thoại đang chạy

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false; // Bắt đầu với sprite không hiển thị
    }

    private void Update()
    {
        if (isActivated && Input.GetKeyDown(KeyCode.E)) // Kiểm tra kích hoạt và nhấn phím E
        {
            if (dialogueManager != null)
            {
                if (!isDialogueActive)
                {
                    // Bắt đầu hội thoại
                    dialogueManager.ShowDialogue();
                    isDialogueActive = true; // Đánh dấu hội thoại đang chạy
                }
                else
                {
                    // Chuyển sang dòng hội thoại tiếp theo
                    dialogueManager.NextLine();
                }
            }
            else
            {
                Debug.LogError("DialogueManager is not assigned in Check.");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Kích hoạt sprite khi người chơi ở gần
            spriteRenderer.enabled = true;
            player = collision.transform; // Lấy Transform của người chơi
            isActivated = true; // Đặt trạng thái kích hoạt thành true
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Tắt sprite khi người chơi ra khỏi khu vực
            spriteRenderer.enabled = false;
            isActivated = false; // Đặt trạng thái kích hoạt thành false

            if (dialogueManager != null)
            {
                dialogueManager.EndDialogue(); // Kết thúc hội thoại
                isDialogueActive = false; // Reset trạng thái hội thoại
            }
            else
            {
                Debug.LogError("DialogueManager is not assigned in Check.");
            }
        }
    }
}
