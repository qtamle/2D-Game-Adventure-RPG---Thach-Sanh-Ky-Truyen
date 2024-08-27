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
    }

    private void JumpToTree()
    {
        transform.position = Vector3.MoveTowards(transform.position, treePosition.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, treePosition.position) < 0.1f)
        {
            Debug.Log("Rắn đã nhảy lên cây và ẩn mình!");
            hasJumped = true;
            StartCoroutine(WaitBeforeAttack());
        }
    }
    private IEnumerator WaitBeforeAttack()
    {
        yield return new WaitForSeconds(2f);

        int skillIndex = Random.Range(0, 10);

        if (skillIndex < 4) // 40% xác suất
        {
            if (!isShootingInProgress)
            {
                isShooting = true;
                isAttacking = false;
                Debug.Log("Rắn bắt đầu phun đạn!");
                maxProjectiles = Random.Range(5, 9);
                StartCoroutine(ShootProjectiles());
            }
        }
        else
        {
            targetPosition = playerTransform.position;
            isAttacking = true;
            isShooting = false;
            Debug.Log("Rắn bắt đầu tấn công!");
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

        yield return new WaitForSeconds(5f);

        isReturning = false;
        hasJumped = false;

        // Sau khi trở về cây, bắt đầu chọn kỹ năng mới
        StartCoroutine(WaitBeforeAttack());
    }

    private IEnumerator ShootProjectiles()
    {
        isShootingInProgress = true; // Đánh dấu rằng coroutine đang chạy

        Debug.Log("Bắt đầu phun đạn...");
        yield return new WaitForSeconds(2f); 

        int numberOfShots = 3; 

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

        StartCoroutine(ReturnToTree());
    }
}
