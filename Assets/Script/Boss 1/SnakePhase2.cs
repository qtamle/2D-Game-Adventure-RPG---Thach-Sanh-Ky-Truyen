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
    public float projectileInterval = 2f;

    private Vector3 targetPosition;
    private Vector3 originalPosition;
    private bool hasJumped = false;
    private bool isAttacking = false;
    private bool isReturning = false;
    private bool isShooting = false;

    private int projectileCount = 0;
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
            if (isShooting)
            {
                if (projectileCount < maxProjectiles)
                {
                    StartCoroutine(ShootProjectiles());
                }
                else
                {
                    isShooting = false;
                    StartCoroutine(ReturnToTree());
                }
            }
            else
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
        yield return new WaitForSeconds(5f);

        switch (Random.Range(0, 2))
        {
            case 0:
                targetPosition = playerTransform.position;
                isAttacking = true;
                Debug.Log("Rắn bắt đầu tấn công!");
                break;
            case 1:
                isShooting = true;
                isAttacking = false;
                Debug.Log("Rắn bắt đầu phun đạn!");
                maxProjectiles = Random.Range(5, 9);
                StartCoroutine(ShootProjectiles()); 
                break;
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
    }
    private IEnumerator ShootProjectiles()
    {
        Debug.Log("Bắt đầu phun đạn...");
        yield return new WaitForSeconds(2f); 

        while (isShooting && projectileCount < maxProjectiles)
        {
            Debug.Log("Phun viên đạn thứ " + (projectileCount + 1));
            GameObject projectile = Instantiate(projectilePrefab, shootingPosition.position, Quaternion.identity);
            Vector3 direction = (playerTransform.position - shootingPosition.position).normalized;
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

            Destroy(projectile, 5f);
            projectileCount++;
            yield return new WaitForSeconds(projectileInterval);
        }

        Debug.Log("Hoàn tất phun đạn.");
    }
}
