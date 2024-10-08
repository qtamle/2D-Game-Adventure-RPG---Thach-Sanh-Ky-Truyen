using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public DialogueCharacter mainCharacter;  // Nhân vật chính
    public DialogueCharacter secondCharacter; // Nhân vật phụ

    [System.Serializable]
    public class DialogueLine
    {
        public DialogueCharacter speaker;   // Nhân vật nói câu này
        public string dialogueText;         // Nội dung hội thoại
        public int emotionIndex;            // Biểu cảm của nhân vật khi nói
    }

    public DialogueLine[] dialogueLines;    // Mảng các đoạn hội thoại
}
