using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingController : MonoBehaviour
{
    public List<Fish> fishList; 
    public FishingMinigame fishingMinigame;
    public Fish currentFish;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(StartFishingWithDelay(1.5f)); 
        }
    }

    public IEnumerator StartFishingWithDelay(float delay)
    {
        fishingMinigame.fishingMinigameUI.SetActive(false); 
        yield return new WaitForSeconds(delay);

        currentFish = fishList[Random.Range(0, fishList.Count)];

        fishingMinigame.SetCurrentFish(currentFish);
        fishingMinigame.StartMinigame();
    }
}

