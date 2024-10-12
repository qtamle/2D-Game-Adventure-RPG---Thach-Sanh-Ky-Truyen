using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCrab : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject crab;

    private bool hasSpawnedFirst = false;
    private void Start()
    {
        StartCoroutine(spawnCrab());
    }

    private IEnumerator spawnCrab()
    {
        while (true)
        {
            float spawnDelay;

            if (!hasSpawnedFirst)
            {
                spawnDelay = Random.Range(1f, 2f);
                hasSpawnedFirst = true;
            }
            else
            {
                spawnDelay = Random.Range(8f, 10f);
            }

            yield return new WaitForSeconds(spawnDelay);

            int randomIndex = Random.Range(0, spawnPoints.Length);
            Instantiate(crab, spawnPoints[randomIndex].position, Quaternion.identity);

        }
    }
}
