using System.Collections;
using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
    public GameObject fishingMinigameUI; // Tham chiếu tới GameObject cha của minigame
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

    private void Start()
    {
        float barWidth = moveBar.localScale.x;
        barLeftLimit = moveBar.position.x - (barWidth / 2);
        barRightLimit = moveBar.position.x + (barWidth / 2);

        fishingMinigameUI.SetActive(false); // Ẩn UI minigame khi bắt đầu game
    }

    public void SetCurrentFish(Fish fish)
    {
        currentFish = fish;
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

    private void StopMinigame()
    {
        isFishingActive = false;
        StartCoroutine(HideMinigameAfterDelay(2f)); // Ẩn UI sau 2 giây khi hoàn thành minigame
    }

    private IEnumerator HideMinigameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        fishingMinigameUI.SetActive(false); // Ẩn UI minigame
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

    private void CheckCatchSuccess()
    {
        if (Mathf.Abs(movingObject.position.x - randomObject.position.x) < 0.1f)
        {
            Debug.Log("Câu cá thành công! Bạn đã câu được: " + currentFish.fishName);
            StopMinigame(); // Dừng và ẩn minigame sau 2 giây
        }
        else
        {
            Debug.Log("Câu cá thất bại, thử lại!");
        }
    }
}
