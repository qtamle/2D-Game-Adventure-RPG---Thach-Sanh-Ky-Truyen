using System.Collections;
using UnityEngine;

public class SnakePhase2 : MonoBehaviour
{
    public Transform playerTransform;
    public float speed = 5f;
    public Transform treePosition;
    public Transform shootingPosition;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    private Vector3 originalPosition;
    private bool hasJumped = false;
    private bool isShooting = false;
    private bool isShootingInProgress = false;
    private bool isExtendingNeck = false;

    public GameObject fireStreamPrefab;
    public Transform fireStreamSpawnPosition;
    public float fireStreamSpeed = 5f;

    private Rigidbody2D rb;

    public GameObject neckPrefab;
    private GameObject currentNeck;
    void Start()
    {
        originalPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!hasJumped)
        {
            JumpToTree();
        }
    }

    private void JumpToTree()
    {
        Vector3 adjustedTreePosition = new Vector3(treePosition.position.x, treePosition.position.y + 2f, treePosition.position.z);

        transform.position = Vector3.MoveTowards(transform.position, adjustedTreePosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, adjustedTreePosition) < 0.01f)
        {
            Debug.Log("Rắn đã nhảy lên cây và ẩn mình!");
            hasJumped = true;
            StartCoroutine(WaitBeforeAttack());
        }
    }
    private IEnumerator WaitBeforeAttack()
    {
        yield return new WaitForSeconds(3f);

        if (!isShootingInProgress && !isExtendingNeck)
        {
            // Random skill để sử dụng
            int skillIndex = Random.Range(1, 4);
            Debug.Log("Skill random được chọn: " + skillIndex); 

            if (skillIndex == 1)
            {
                isShootingInProgress = true;
                Debug.Log("Rắn bắt đầu phun lửa!");
                StartCoroutine(BlowFireStream());
            }
            else if (skillIndex == 2)
            {
                isShootingInProgress = true;
                Debug.Log("Rắn bắt đầu phun đạn!");
                StartCoroutine(ShootProjectiles());
            }
            else if (skillIndex == 3 && !isExtendingNeck)
            {
                isExtendingNeck = true;
                Debug.Log("Rắn bắt đầu kéo dài cổ!");
                StartCoroutine(ExtendNeck(40, 5));
            }
            else
            {
                Debug.Log("Không có skill nào được kích hoạt."); 
            }
        }
        else
        {
            Debug.Log("Rắn hiện đang thực hiện một skill khác, không thể random skill mới."); 
        }
    }


    public void Jump()
    {
        if (!hasJumped)
        {
            Vector3 adjustedTreePosition = new Vector3(treePosition.position.x, treePosition.position.y + 2f, treePosition.position.z);
            StartCoroutine(JumpAndStay(adjustedTreePosition));
        }
    }

    private IEnumerator JumpAndStay(Vector3 targetPosition)
    {
        float jumpHeight = 2f; 
        float jumpDuration = 1f; 
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        while (elapsedTime < jumpDuration)
        {
            float yOffset = Mathf.Sin((elapsedTime / jumpDuration) * Mathf.PI) * jumpHeight; 
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / jumpDuration) + new Vector3(0, yOffset, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; 
        hasJumped = true;
        Debug.Log("Rắn đã nhảy lên cây và đứng trên đó!");

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.gravityScale = 0;
            rb.simulated = false;
        }
    }

    private IEnumerator ShootProjectiles()
    {
        Debug.Log("Bắt đầu phun đạn...");
        yield return new WaitForSeconds(2f);

        int numberOfShots = Random.Range(3, 6);

        for (int shot = 0; shot < numberOfShots; shot++)
        {
            int projectilesInFan = Random.Range(4, 6);
            float angleSpread = 110f;
            float angleStep = angleSpread / (projectilesInFan - 1);
            float startAngle = -angleSpread / 2;

            Debug.Log($"Đợt {shot + 1}: Bắn {projectilesInFan} viên đạn");

            for (int i = 0; i < projectilesInFan; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, shootingPosition.position, Quaternion.identity);

                float currentAngle = startAngle + (i * angleStep);
                Vector3 direction = Quaternion.Euler(0, 0, currentAngle) * Vector3.down;

                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.velocity = direction * projectileSpeed;

                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

                    Debug.Log("Viên đạn được phun với tốc độ: " + rb.velocity);
                }
                else
                {
                    Debug.LogError("Không tìm thấy Rigidbody2D trên prefab của viên đạn!");
                }

                Destroy(projectile, 3f);
            }

            Debug.Log("Hoàn tất đợt bắn thứ " + (shot + 1));
            yield return new WaitForSeconds(3f);
        }

        Debug.Log("Hoàn tất phun đạn.");
        isShootingInProgress = false;

        StartCoroutine(WaitBeforeAttack());
    }

    private IEnumerator BlowFireStream()
    {
        GameObject fireStream = Instantiate(fireStreamPrefab, fireStreamSpawnPosition.position, Quaternion.Euler(55, 90, 0));
        Transform fireStreamTransform = fireStream.transform;

        Quaternion startRotation = Quaternion.Euler(55, 90, 0);
        Quaternion endRotation = Quaternion.Euler(115, 90, 0);

        float duration = 5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            fireStreamTransform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            fireStreamTransform.position = Vector3.MoveTowards(fireStreamTransform.position, fireStreamSpawnPosition.position + Vector3.up * 5f, fireStreamSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fireStreamTransform.rotation = endRotation;
        Destroy(fireStream);
        Debug.Log("Ngọn lửa đã hoàn tất và bị hủy.");

        isShootingInProgress = false;
        StartCoroutine(WaitBeforeAttack());
    }
    private IEnumerator ExtendNeck(float speed, float duration)
    {
        if (currentNeck == null)
        {
            currentNeck = Instantiate(neckPrefab, treePosition.position, Quaternion.identity);
        }

        Vector3 targetPosition = playerTransform.position;

        currentNeck.transform.position = treePosition.position;

        Vector3 direction = (targetPosition - treePosition.position).normalized;
        float distance = Vector3.Distance(treePosition.position, targetPosition);

        float elapsedTime = 0f;
        float extraLengthFactor = 1.8f;
        float maxLength = distance * extraLengthFactor;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 

        while (elapsedTime < maxLength / speed)
        {
            elapsedTime += Time.deltaTime;

            float currentLength = Mathf.Lerp(1f, maxLength, elapsedTime / (maxLength / speed));
            currentNeck.transform.localScale = new Vector3(currentLength, 1f, 1f);
            currentNeck.transform.rotation = Quaternion.Euler(0, 0, angle);  

            yield return null;
        }

        currentNeck.transform.localScale = new Vector3(maxLength, 1f, 1f);
        currentNeck.transform.rotation = Quaternion.Euler(0, 0, angle);

        Debug.Log("Cổ rắn đã kéo dài tới vị trí của người chơi!");

        yield return new WaitForSeconds(0.5f);

        elapsedTime = 0f;

        // Thu cổ rắn về cùng hướng kéo ra
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float currentLength = Mathf.Lerp(maxLength, 1f, elapsedTime / duration);
            currentNeck.transform.localScale = new Vector3(currentLength, 1f, 1f);

            currentNeck.transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }

        currentNeck.transform.localScale = new Vector3(1f, 1f, 1f);

        Destroy(currentNeck, 1f);
        currentNeck = null;

        Debug.Log("Cổ rắn đã quay về và bị hủy.");

        isExtendingNeck = false;
        StartCoroutine(WaitBeforeAttack());
    }

}
