using UnityEngine;
using UnityEngine.UI;

public class MainMenuSL : MonoBehaviour
{
    public Button[] newGameButtons;
    public Button[] continueButtons;
    public Button[] deleteButtons;  
    public static int selectedSaveSlot = 1;

    private SaveManager saveManager;

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();

        // Kiểm tra dữ liệu lưu cho mỗi slot
        for (int i = 0; i < 3; i++)
        {
            int slotIndex = i + 1;

            // Kiểm tra nếu có dữ liệu lưu, nếu có thì tắt nút New Game và bật nút Continue và Delete
            bool hasSaveData = saveManager.HasSaveData(slotIndex);
            continueButtons[i].gameObject.SetActive(hasSaveData); 
            deleteButtons[i].gameObject.SetActive(hasSaveData);     

            // Tắt nút New Game nếu có dữ liệu
            newGameButtons[i].gameObject.SetActive(!hasSaveData);

            // Gán sự kiện cho các nút
            newGameButtons[i].onClick.AddListener(() => StartNewGame(slotIndex));
            continueButtons[i].onClick.AddListener(() => ContinueGame(slotIndex));
            deleteButtons[i].onClick.AddListener(() => DeleteSaveData(slotIndex)); 
        }
    }

    // Phương thức cho việc bắt đầu game mới
    public void StartNewGame(int saveSlot)
    {
        selectedSaveSlot = saveSlot;  // Lưu slot đã chọn
        saveManager.SaveGame(Vector3.zero, "StartScene", selectedSaveSlot);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Làng");
    }

    public void ContinueGame(int saveSlot)
    {
        selectedSaveSlot = saveSlot;  // Lưu slot đã chọn
        SaveData data = saveManager.LoadGame(selectedSaveSlot);
        if (data != null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(data.currentScene);
        }
    }

    // Phương thức để xóa dữ liệu của save slot
    public void DeleteSaveData(int saveSlot)
    {
        saveManager.DeleteSaveData(saveSlot); 
        continueButtons[saveSlot - 1].gameObject.SetActive(false);  
        deleteButtons[saveSlot - 1].gameObject.SetActive(false);    
        newGameButtons[saveSlot - 1].gameObject.SetActive(true);   
        Debug.Log("Dữ liệu đã được xóa cho save slot " + saveSlot);
    }
}
