using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingMinigame : MonoBehaviour
{
    public GameObject fishingMinigameUI;
    public GameObject resultPanel;
    public TMP_Text fishNameText;
    public TMP_Text rarityText;
    public TMP_Text coinText;
    public Image fishImageDisplay;
    public Button backToVillageButton;
    public Button fishAgainButton;

    public Transform moveBar;
    public Transform movingObject;
    public Transform randomObject;

    public float baseMoveSpeed = 2f;
    private bool isMovingRight = true;
    private bool isFishingActive = false;
    private float barLeftLimit;
    private float barRightLimit;

    private Fish currentFish;
    private float moveSpeed;

    private FishingController fishingController; 

    private void Start()
    {
        fishingController = FindObjectOfType<FishingController>(); 

        float barWidth = moveBar.localScale.x;
        barLeftLimit = moveBar.position.x - (barWidth / 2);
        barRightLimit = moveBar.position.x + (barWidth / 2);

        fishingMinigameUI.SetActive(false); 
        resultPanel.SetActive(false); 
    }

    public void SetCurrentFish(Fish fish)
    {
        currentFish = fish;
    }

    private void ShowResultPanel()
    {
        resultPanel.SetActive(true);
        fishNameText.text = $"Chúc mừng bạn đã câu được: {currentFish.fishName}";
        fishImageDisplay.sprite = currentFish.fishImage;
        rarityText.text = $"Độ hiếm: {currentFish.rarity}"; 
        coinText.text = $"Số coin nhận được: {currentFish.coins}";
        backToVillageButton.onClick.RemoveAllListeners();
        backToVillageButton.onClick.AddListener(BackToVillage);
        fishAgainButton.onClick.RemoveAllListeners();
        fishAgainButton.onClick.AddListener(FishAgain);
    }

    private void BackToVillage()
    {
        Debug.Log("Quay lại làng!");
        resultPanel.SetActive(false);
        fishingMinigameUI.SetActive(false);
        // Logic quay lại làng
    }

    private void FishAgain()
    {
        Debug.Log("Câu tiếp!");
        currentFish = fishingController.fishList[Random.Range(0, fishingController.fishList.Count)];
        SetCurrentFish(currentFish);
        resultPanel.SetActive(false);
        StartMinigame();
    }

    private void CheckCatchSuccess()
    {
        if (Mathf.Abs(movingObject.position.x - randomObject.position.x) < 0.1f)
        {
            Debug.Log("Câu cá thành công! Bạn đã câu được: " + currentFish.fishName);
            isFishingActive = false;
            ShowResultPanel();
        }
        else
        {
            Debug.Log("Câu cá thất bại, thử lại!");
        }
    }

    public void StartMinigame()
    {
        if (currentFish != null)
        {
            moveSpeed = baseMoveSpeed + (1f / currentFish.rarity) * 3f;
        }

        float randomPosition = Random.Range(barLeftLimit, barRightLimit);
        randomObject.position = new Vector3(randomPosition, randomObject.position.y, randomObject.position.z);

        movingObject.position = new Vector3(barLeftLimit, movingObject.position.y, movingObject.position.z);

        isMovingRight = true;
        isFishingActive = true;
        fishingMinigameUI.SetActive(true); // Hiển thị UI minigame khi bắt đầu
    }

    void Update()
    {
        if (isFishingActive)
        {
            float moveDirection = isMovingRight ? 1 : -1;
            movingObject.position += Vector3.right * moveSpeed * moveDirection * Time.deltaTime;

            if (movingObject.position.x >= barRightLimit)
            {
                isMovingRight = false;
            }
            else if (movingObject.position.x <= barLeftLimit)
            {
                isMovingRight = true;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CheckCatchSuccess();
            }
        }
    }
}
