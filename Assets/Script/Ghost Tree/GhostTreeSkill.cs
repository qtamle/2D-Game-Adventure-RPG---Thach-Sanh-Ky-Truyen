using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GhostTreeSkill : MonoBehaviour
{
    [SerializeField] public Animator anim;
    [Header("Vine")]
    public GameObject vineObject;
    public GameObject warningPrefab;
    public float timeAppears = 1f;
    public float warningDuration = 1f;
    public float groundCheckDistance = 10f;
    public float warningOffsetY = 1f;
    private bool isVineSpawned = false;

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
    public ParticleSystem leaf;
    public Transform leafSpawn;

    [Header("Spike")]
    public GameObject largeSpikePrefab;
    public float spikeSpeed = 10f;
    public Transform spawnPoint;

    [HideInInspector]
    private Transform playerTransform;
    private bool isUsingSkill = false;
    private void Start()
    {
        numberOfSpawns = Random.Range(2, 3);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(ManageSkills());

        ParticleSystem leafPrefab = Instantiate(leaf, leafSpawn.position, Quaternion.identity);
    }
    private IEnumerator ManageSkills()
    {
        while (true)
        {
            if (!isUsingSkill) 
            {
                Debug.Log("Waiting before executing a new skill...");
                yield return new WaitForSeconds(2f);

                int skillIndex = Random.Range(0,4);
                Debug.Log($"Executing skill {skillIndex}");

                isUsingSkill = true;

                switch (skillIndex)
                {
                    case 0:
                        yield return StartCoroutine(AnimVine());
                        yield return StartCoroutine(SpawnVine());
                        break;
                    case 1:
                        yield return StartCoroutine(AnimHands());
                        yield return StartCoroutine(MoveHands());
                        break;
                    case 2:
                        StartCoroutine(SpawnMiniMonsters());
                        break;
                    case 3:
                        yield return StartCoroutine(ShootSpikes());
                        break;
                }

                isUsingSkill = false;

                float waitTime = Random.Range(1f, 2.2f);
                Debug.Log($"Waiting {waitTime} seconds before the next skill.");
                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }
    }

    private IEnumerator SpawnVine()
    {
        if (VineTie.isVineActive)
        {
            Debug.Log("Vine hiện tại vẫn tồn tại, không thể spawn Vine mới.");
            yield break;
        }

        VineTie vineTie = FindObjectOfType<VineTie>();
        if (vineTie != null && vineTie.isHitPlayer)
        {
            Debug.Log("Player đang bị trói, không thể spawn vine");
            yield break;
        }

        int vineSpawnCount = Random.Range(1, 1);

        for (int i = 0; i < vineSpawnCount; i++)
        {
            Vector3 spawnPosition = playerTransform.position;
            Vector3 groundPosition = Vector3.zero;

            // Kiểm tra mặt đất dưới người chơi
            RaycastHit2D groundCheck = Physics2D.Raycast(spawnPosition, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

            if (groundCheck.collider != null)
            {
                // Lấy vị trí mặt đất
                groundPosition = new Vector3(spawnPosition.x, groundCheck.point.y, spawnPosition.z);
                Vector3 warningPosition = new Vector3(groundPosition.x, groundPosition.y + warningOffsetY, groundPosition.z);
                GameObject warning = Instantiate(warningPrefab, warningPosition, Quaternion.identity);

                yield return new WaitForSeconds(warningDuration);
                Destroy(warning);
                yield return new WaitForSeconds(timeAppears);

                GameObject vine = Instantiate(vineObject, groundPosition, Quaternion.identity);
                vine.layer = LayerMask.NameToLayer("Ground");
            }
            else
            {
                Debug.Log("No ground detected below the player.");
            }

            yield return new WaitForSeconds(3f);
        }
    }

    private IEnumerator ResetVineSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        isVineSpawned = false;
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
            Destroy(leftHand, 3f);
        }

        if (rightHand != null)
        {
            rightHand.transform.position = smashPoint;
            Destroy(rightHand, 3f);
        }

        yield return new WaitForSeconds(2f);

    }

    private IEnumerator SpawnMiniMonsters()
    {
       
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < numberOfSpawns; i++)
        {
            yield return StartCoroutine(AnimSpawn());

            float randomX = Random.Range(minSpawnX, maxSpawnX);
            Vector3 spawnPosition = new Vector3(randomX, spawnY, spawnZ);

            Instantiate(sapling, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(2f);
        }
    }
    private IEnumerator ShootSpikes()
    {
        
        Debug.Log("Starting spike shooting coroutine...");

        int spikeCount = Random.Range(1, 6);

        for (int i = 0; i < spikeCount; i++)
        {
            yield return StartCoroutine(AnimSpike());

            Debug.Log("Shooting spike...");

            Vector3 spawnPosition = spawnPoint.position;

            spawnPosition.z = 10f;

            Debug.Log($"Player Position: {playerTransform.position}");
            Debug.Log($"Spawn Position: {spawnPosition}");

            GameObject largeSpike = Instantiate(largeSpikePrefab, spawnPosition, Quaternion.identity);

            Rigidbody2D rb = largeSpike.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 direction = (playerTransform.position - spawnPosition).normalized;
                rb.velocity = new Vector2(direction.x, direction.y) * spikeSpeed;

                Debug.Log($"Spike Velocity: {rb.velocity}");
            }
            else
            {
                Debug.LogError("No Rigidbody2D found on the large spike prefab!");
            }

            yield return new WaitForSeconds(3f);
        }

        Debug.Log("Finished shooting spikes.");
    }

    IEnumerator AnimVine()
    {
        anim.SetTrigger("Vine");
        yield return new WaitForSeconds(2f);
    }

    IEnumerator AnimHands()
    {
        anim.SetTrigger("Hands");
        yield return new WaitForSeconds(2.2f);
    }

    IEnumerator AnimSpike()
    {
        anim.SetTrigger("Spike");
        yield return new WaitForSeconds(2f);
    }

    IEnumerator AnimSpawn()
    {
        anim.SetTrigger("Spam");
        yield return new WaitForSeconds(2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, groundCheckDistance);
    }
}
