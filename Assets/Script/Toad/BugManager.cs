using UnityEngine;

public class BugSpawner : MonoBehaviour
{
    public GameObject bugPrefab; 
    public Collider2D spawnArea; 
    public int numberOfBugs = 15; 

    void Start()
    {
        SpawnBugs();
    }

    void SpawnBugs()
    {
        for (int i = 0; i < numberOfBugs; i++)
        {
            Vector2 spawnPosition = GetRandomPositionInCollider(spawnArea);
            GameObject bug = Instantiate(bugPrefab, spawnPosition, Quaternion.identity);
            Bug flyingBug = bug.GetComponent<Bug>();
            if (flyingBug != null)
            {
                flyingBug.BoundsCollider = spawnArea;
            }
        }
    }

    private Vector2 GetRandomPositionInCollider(Collider2D collider)
    {
        Bounds bounds = collider.bounds;
        Vector2 randomPosition = new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
        return randomPosition;
    }
}
