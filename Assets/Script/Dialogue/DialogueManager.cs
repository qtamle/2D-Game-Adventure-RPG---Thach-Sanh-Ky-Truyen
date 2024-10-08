using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public DialogueData dialogueData; // ScriptableObject chứa dữ liệu hội thoại

    public GameObject MainDialogueCanvas; // Canvas cho nhân vật chính
    public GameObject SecondDialogueCanvas; // Canvas cho nhân vật phụ

    [SerializeField] private TMP_Text mainspeakerText; // Tên nhân vật chính
    [SerializeField] private TMP_Text mainDialogueText; // Hội thoại của nhân vật chính
    [SerializeField] private Image mainPortraitImage; // Chân dung nhân vật chính

    [SerializeField] private TMP_Text secondspeakerText; // Tên nhân vật phụ
    [SerializeField] private TMP_Text secondDialogueText; // Hội thoại của nhân vật phụ
    [SerializeField] private Image secondPortraitImage; // Chân dung nhân vật phụ

    private int currentLineIndex = 0;

    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Nhấn phím E để chuyển tiếp hội thoại
        {
            NextLine();
        }
    }

    public void ShowDialogue()
    {
        // Lấy dòng hội thoại hiện tại
        DialogueData.DialogueLine line = dialogueData.dialogueLines[currentLineIndex];

        // Cập nhật tên, đoạn văn bản và chân dung nhân vật
        if (line.speaker == dialogueData.mainCharacter)
        {
            mainspeakerText.text = line.speaker.characterName; // Cập nhật tên nhân vật chính
            mainDialogueText.text = line.dialogueText; // Cập nhật hội thoại của nhân vật chính
            mainPortraitImage.sprite = dialogueData.mainCharacter.portraits[line.emotionIndex]; // Chân dung nhân vật chính

            // Hiển thị UI của nhân vật chính và ẩn UI của nhân vật phụ
            MainDialogueCanvas.SetActive(true);
            SecondDialogueCanvas.SetActive(false);
        }
        else if (line.speaker == dialogueData.secondCharacter)
        {
            secondspeakerText.text = line.speaker.characterName; // Cập nhật tên nhân vật phụ
            secondDialogueText.text = line.dialogueText; // Cập nhật hội thoại của nhân vật phụ
            secondPortraitImage.sprite = dialogueData.secondCharacter.portraits[line.emotionIndex]; // Chân dung nhân vật phụ

            // Hiển thị UI của nhân vật phụ và ẩn UI của nhân vật chính
            MainDialogueCanvas.SetActive(false);
            SecondDialogueCanvas.SetActive(true);
        }
    }

    public void NextLine()
    {
        
        if (currentLineIndex < dialogueData.dialogueLines.Length)
        {
            ShowDialogue();
            currentLineIndex++;
        }
        else
        {
            EndDialogue();
        }

    }

    public void EndDialogue()
    {
        if (this != null) // Kiểm tra nếu đối tượng này không null
        {
            // Ẩn cả hai canvas khi hội thoại kết thúc
            MainDialogueCanvas.SetActive(false);
            SecondDialogueCanvas.SetActive(false);

            // Reset lại chỉ số dòng hội thoại
            currentLineIndex = 0;
        }
    }
}
