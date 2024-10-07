[System.Serializable]
public class Dialogue
{
    public string mainSpeakerName; // Tên nhân vật chính nói
    public string mainDialogueText; // Đoạn hội thoại của nhân vật chính
    public int mainCharacterEmotionIndex; // Chỉ số cảm xúc nhân vật chính

    public string secondSpeakerName; // Tên nhân vật phụ nói (nếu có)
    public string secondDialogueText; // Đoạn hội thoại của nhân vật phụ
    public int secondCharacterEmotionIndex; // Chỉ số cảm xúc nhân vật phụ
}
