using UnityEngine;

public class Check : MonoBehaviour
{
    private Transform player;
    private SpriteRenderer spriteRenderer;
    public DialogueManager dialogueManager; // Thêm biến này

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false; // Bắt đầu với sprite không hiển thị
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Kích hoạt sprite khi người chơi ở gần
            spriteRenderer.enabled = true;
            player = collision.transform; // Lấy Transform của người chơi
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Tắt sprite khi người chơi ra khỏi khu vực
            spriteRenderer.enabled = false;

            // Kết thúc cuộc hội thoại khi người chơi ra khỏi collider
            if (dialogueManager != null)
            {
                dialogueManager.EndDialogue(); // Gọi phương thức để kết thúc hội thoại
            }
            else
            {
                Debug.LogError("DialogueManager is not assigned in Check.");
            }
        }
    }
}
