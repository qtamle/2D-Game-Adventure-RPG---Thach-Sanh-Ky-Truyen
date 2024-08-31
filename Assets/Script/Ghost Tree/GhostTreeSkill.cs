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
    public float handRetractSpeed = 5f;
    private Transform playerTransform;
    private GameObject leftHand;
    private GameObject rightHand;
    private bool handsCollided = false;

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

            float waitTime = Random.Range(3f, 5f);
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

        AddColliderAndDebug(leftHand);
        AddColliderAndDebug(rightHand);

        Vector3 leftHandStart = leftHandStartPosition.position;
        Vector3 rightHandStart = rightHandStartPosition.position;
        Vector3 smashPoint = handSmashPoint.position;

        while (Vector3.Distance(leftHand.transform.position, smashPoint) > 0.1f &&
               Vector3.Distance(rightHand.transform.position, smashPoint) > 0.1f)
        {
            if (handsCollided) break;

            leftHand.transform.position = Vector3.MoveTowards(leftHand.transform.position, smashPoint, handMoveSpeed * Time.deltaTime);
            rightHand.transform.position = Vector3.MoveTowards(rightHand.transform.position, smashPoint, handMoveSpeed * Time.deltaTime);

            yield return null;
        }

        if (!handsCollided)
        {
            leftHand.transform.position = smashPoint;
            rightHand.transform.position = smashPoint;
        }

        yield return new WaitForSeconds(0.1f);

        if (handsCollided)
        {
            StartCoroutine(HandleHandsCollision());
        }
        else
        {
            while (Vector3.Distance(leftHand.transform.position, leftHandStart) > 0.1f ||
                   Vector3.Distance(rightHand.transform.position, rightHandStart) > 0.1f)
            {
                leftHand.transform.position = Vector3.MoveTowards(leftHand.transform.position, leftHandStart, handRetractSpeed * Time.deltaTime);
                rightHand.transform.position = Vector3.MoveTowards(rightHand.transform.position, rightHandStart, handRetractSpeed * Time.deltaTime);

                yield return null;
            }

            leftHand.transform.position = leftHandStart;
            rightHand.transform.position = rightHandStart;
        }
    }

    private void AddColliderAndDebug(GameObject hand)
    {
        if (hand.GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning($"{hand.name} does not have a Collider2D component. Adding BoxCollider2D.");
            hand.AddComponent<BoxCollider2D>();
        }
        else
        {
            Debug.Log($"{hand.name} has Collider2D.");
        }

        if (hand.GetComponent<Rigidbody2D>() == null)
        {
            Debug.LogWarning($"{hand.name} does not have a Rigidbody2D component. Adding Rigidbody2D.");
            Rigidbody2D rb = hand.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;  // Sử dụng isKinematic = true để không ảnh hưởng đến vật lý.
        }
        else
        {
            Debug.Log($"{hand.name} has Rigidbody2D.");
        }
    }

    private IEnumerator HandleHandsCollision()
    {
        // Wait for a short moment before starting retraction
        yield return new WaitForSeconds(0.5f);

        // Stop hands and start retracting
        while (Vector3.Distance(leftHand.transform.position, leftHandStartPosition.position) > 0.1f ||
               Vector3.Distance(rightHand.transform.position, rightHandStartPosition.position) > 0.1f)
        {
            leftHand.transform.position = Vector3.MoveTowards(leftHand.transform.position, leftHandStartPosition.position, handRetractSpeed * Time.deltaTime);
            rightHand.transform.position = Vector3.MoveTowards(rightHand.transform.position, rightHandStartPosition.position, handRetractSpeed * Time.deltaTime);

            yield return null;
        }

        leftHand.transform.position = leftHandStartPosition.position;
        rightHand.transform.position = rightHandStartPosition.position;

        Destroy(leftHand);
        Destroy(rightHand);
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
