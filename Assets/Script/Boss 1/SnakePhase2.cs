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

    private Vector3 targetPosition;
    private Vector3 originalPosition;
    private bool hasJumped = false;
    private bool isAttacking = false;
    private bool isReturning = false;
    private bool isShooting = false;
    private bool isShootingInProgress = false;

    private int maxProjectiles;

    public GameObject fireStreamPrefab;
    public Transform fireStreamSpawnPosition;
    public float fireStreamSpeed = 5f;
    public float fireStreamRotationSpeed = 30f;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (!hasJumped)
        {
            JumpToTree();
        }
        else if (isAttacking)
        {
            if (!isShooting && !isShootingInProgress)
            {
                MoveToTarget();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && hasJumped)
        {
            StartCoroutine(BlowFireStream());
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

        if (!isAttacking && !isShooting && !isShootingInProgress)
        {
            int skillIndex = Random.Range(0, 100); 

            if (skillIndex < 25) // 25% xác suất cho BlowFireStream
            {
                isShooting = false;
                isAttacking = false;
                Debug.Log("Rắn bắt đầu phun lửa!");
                StartCoroutine(BlowFireStream());
            }
            else if (skillIndex < 50) // 25% xác suất cho ShootProjectiles
            {
                isShooting = true;
                isAttacking = false;
                Debug.Log("Rắn bắt đầu phun đạn!");
                maxProjectiles = Random.Range(5, 9);
                StartCoroutine(ShootProjectiles());
            }
            else // 50% xác suất cho skill còn lại
            {
                targetPosition = playerTransform.position;
                isAttacking = true;
                isShooting = false;
                Debug.Log("Rắn bắt đầu tấn công!");
            }
        }
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Debug.Log("Rắn đã tới vị trí đã lưu và cắn!");
            StartCoroutine(ReturnToTree());
        }
    }

    private IEnumerator ReturnToTree()
    {
        isAttacking = false;
        isReturning = true;

        yield return new WaitForSeconds(2f);

        while (Vector3.Distance(transform.position, treePosition.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, treePosition.position, speed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("Rắn đã quay về vị trí trên cây.");

        yield return new WaitForSeconds(3f);

        isReturning = false;
        hasJumped = false;

        StartCoroutine(WaitBeforeAttack());
    }
    private IEnumerator ShootProjectiles()
    {
        isShootingInProgress = true;
        isShooting = true; 

        Debug.Log("Bắt đầu phun đạn...");
        yield return new WaitForSeconds(2f);

        int numberOfShots = Random.Range(3, 5);

        for (int shot = 0; shot < numberOfShots; shot++)
        {
            int projectilesInFan = Random.Range(5, 10);
            float angleSpread = 120f; // Góc phân tán của hình quạt
            float angleStep = angleSpread / (projectilesInFan - 1); // Bước góc giữa các viên đạn
            float startAngle = -angleSpread / 2; // Góc bắt đầu của hình quạt

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
        isShooting = false; 

        StartCoroutine(ReturnToTree());
    }
    private IEnumerator BlowFireStream()
    {
        isShootingInProgress = true; 

        GameObject fireStream = Instantiate(fireStreamPrefab, fireStreamSpawnPosition.position, Quaternion.Euler(55, 90, 0));
        Transform fireStreamTransform = fireStream.transform;

        Quaternion startRotation = Quaternion.Euler(55, 90, 0);
        Quaternion endRotation = Quaternion.Euler(115, 90, 0);

        float duration = 5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Cập nhật vị trí và rotation của luồng lửa
            fireStreamTransform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            fireStreamTransform.position = Vector3.MoveTowards(fireStreamTransform.position, fireStreamSpawnPosition.position + Vector3.up * 5f, fireStreamSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fireStreamTransform.rotation = endRotation;

        Destroy(fireStream);

        Debug.Log("Ngọn lửa đã hoàn tất và bị hủy.");

        isShootingInProgress = false; 

        StartCoroutine(ReturnToTree());
    }
}

