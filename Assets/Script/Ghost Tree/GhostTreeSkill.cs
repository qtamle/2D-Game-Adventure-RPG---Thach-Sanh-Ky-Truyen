using System.Collections;
using UnityEngine;

public class GhostTreeSkill : MonoBehaviour
{
    [Header("Vine")]
    public GameObject vineObject;
    public GameObject warningPrefab;
    public float timeAppears = 1.5f;
    public float warningDuration = 1f;
    public float groundCheckDistance = 10f;
    public float warningOffsetY = 1f;

    [Header("Hands")]
    public GameObject leftHandPrefab;
    public GameObject rightHandPrefab;
    public Transform leftHandStartPosition;
    public Transform rightHandStartPosition;
    public Transform handSmashPoint;
    public float handMoveSpeed = 5f;
    private GameObject leftHand;
    private GameObject rightHand;

    [Header("Spawn")]
    public GameObject sapling;
    public float minSpawnX;
    public float maxSpawnX;
    public float spawnY;
    public float spawnZ = 0f;
    private int numberOfSpawns;

    [Header("Spike")]
    public GameObject largeSpikePrefab;
    public float spikeSpeed = 10f;
    public Transform[] spawnPoints;

    [HideInInspector]
    private Transform playerTransform;
    private void Start()
    {
        numberOfSpawns = Random.Range(3, 8);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(ManageSkills());
    }

    private IEnumerator ManageSkills()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            int skillIndex = Random.Range(0, 4);
            switch (skillIndex)
            {
                case 0:
                    yield return StartCoroutine(SpawnVine());
                    break;
                case 1:
                    yield return StartCoroutine(MoveHands());
                    break;
                case 2:
                    StartCoroutine(SpawnMiniMonsters()); 
                    break;
                case 3:
                    yield return StartCoroutine(ShootSpikes());
                    break;
            }

            float waitTime = Random.Range(1f, 3f);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator SpawnVine()
    {
        Vector3 spawnPosition = playerTransform.position;
        Vector3 groundPosition = Vector3.zero;

        // Kiểm tra mặt đất dưới người chơi
        RaycastHit2D groundCheck = Physics2D.Raycast(spawnPosition, Vector2.down, groundCheckDistance, LayerMask.GetMask("Ground"));

        if (groundCheck.collider != null)
        {
            groundPosition = groundCheck.point;

            // Thêm offset theo trục y cho vị trí hiển thị của cảnh báo
            Vector3 warningPosition = new Vector3(groundPosition.x, groundPosition.y + warningOffsetY, groundPosition.z);

            // Hiện cảnh báo và dây leo
            GameObject warning = Instantiate(warningPrefab, warningPosition, Quaternion.identity);

            yield return new WaitForSeconds(warningDuration);

            Destroy(warning);

            yield return new WaitForSeconds(timeAppears);

            GameObject vine = Instantiate(vineObject, groundPosition, Quaternion.identity);
        }
        else
        {
            Debug.Log("No ground detected below the player.");
        }
    }

    private IEnumerator MoveHands()
    {
        leftHand = Instantiate(leftHandPrefab, leftHandStartPosition.position, Quaternion.identity);
        rightHand = Instantiate(rightHandPrefab, rightHandStartPosition.position, Quaternion.identity);

        Vector3 smashPoint = handSmashPoint.position;

        while ((leftHand != null && Vector3.Distance(leftHand.transform.position, smashPoint) > 0.1f) &&
               (rightHand != null && Vector3.Distance(rightHand.transform.position, smashPoint) > 0.1f))
        {
            if (leftHand != null)
            {
                leftHand.transform.position = Vector3.MoveTowards(leftHand.transform.position, smashPoint, handMoveSpeed * Time.deltaTime);
            }

            if (rightHand != null)
            {
                rightHand.transform.position = Vector3.MoveTowards(rightHand.transform.position, smashPoint, handMoveSpeed * Time.deltaTime);
            }

            yield return null;
        }

        if (leftHand != null)
        {
            leftHand.transform.position = smashPoint;
        }

        if (rightHand != null)
        {
            rightHand.transform.position = smashPoint;
        }
    }


    private IEnumerator SpawnMiniMonsters()
    {
        // Wait for 2 seconds before starting to spawn mini monsters
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < numberOfSpawns; i++)
        {
            float randomX = Random.Range(minSpawnX, maxSpawnX);
            Vector3 spawnPosition = new Vector3(randomX, spawnY, spawnZ);

            Instantiate(sapling, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(2f);
        }
    }
    private IEnumerator ShootSpikes()
    {
        Debug.Log("Starting spike shooting coroutine...");

        int spikeCount = Random.Range(1, 5);

        for (int i = 0; i < spikeCount; i++)
        {
            Debug.Log("Shooting spike...");

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 spawnPosition = spawnPoint.position;

            // Kiểm tra vị trí của người chơi và điểm spawn
            Debug.Log($"Player Position: {playerTransform.position}");
            Debug.Log($"Spawn Position: {spawnPosition}");

            GameObject largeSpike = Instantiate(largeSpikePrefab, spawnPosition, Quaternion.identity);

            Rigidbody2D rb = largeSpike.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 direction = (playerTransform.position - spawnPosition).normalized;
                rb.velocity = new Vector2(direction.x, direction.y) * spikeSpeed;

                // Kiểm tra vận tốc
                Debug.Log($"Spike Velocity: {rb.velocity}");
            }
            else
            {
                Debug.LogError("No Rigidbody2D found on the large spike prefab!");
            }

            // Thời gian chờ giữa các lần bắn spike nếu có nhiều spike.
            yield return new WaitForSeconds(3f);
        }

        Debug.Log("Finished shooting spikes.");
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, groundCheckDistance);
    }
}
