using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingController : MonoBehaviour
{
    public List<Fish> fishList;
    public FishingMinigame fishingMinigame;
    private Fish currentFish;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(StartFishingWithDelay(1.5f)); // Đợi 1.5 giây trước khi bắt đầu minigame
        }
    }

    private IEnumerator StartFishingWithDelay(float delay)
    {
        fishingMinigame.fishingMinigameUI.SetActive(false); // Ẩn UI minigame
        yield return new WaitForSeconds(delay);

        currentFish = fishList[Random.Range(0, fishList.Count)];

        fishingMinigame.SetCurrentFish(currentFish);
        fishingMinigame.StartMinigame();
    }
}
