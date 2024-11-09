using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PythonSkillRemake : MonoBehaviour
{
    [Header("Dash")]
    public float speed = 5f;
    private Transform player;
    private bool isDashing = false;

    [Header("Tail")]
    public GameObject tailPrefab;
    public float tailGrowTime = 1.5f;
    public float tailRetractTime = 1.5f;
    private Vector3 spawnPosition;
    private Vector3 retractPosition;

    [Header("FireStream")]
    public GameObject fireStreamPrefab;
    public Transform fireStreamStartPoint;
    public float fireStreamDuration = 5f;
    public float rotationSpeed;

    [Header("Fire Pillar")]
    public GameObject firePillarPrefab;
    public float firePillarDuration = 3f;

    [Header("Explosion")]
    public GameObject explosionPrefab;
    public float explosionDuration = 1f;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 5f;
    public float launchDuration = 3f;

    [Header("Fireballs")]
    public GameObject fireballPrefab;
    public int minFireballs = 3;
    public int maxFireballs = 5;
    public float fireballFallHeight = 29.4f; // Độ cao bắt đầu rơi

    [Header("Other")]
    private Vector3 lastPlayerPosition;
    public Transform playerTransform;

    private bool hasDamaged = false;
    private bool isSkillActive = false;
    private List<int> skillList = new List<int> { 0, 1, 2, 3, 4 };
    private int lastSkillIndex = -1;
    private bool isDowned = false;
    private bool isSkyfallActive = false;
    private bool isRandomSkillActive = false;
    private void Start()
    {
        // Bỏ qua layer 
        int myLayer = gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");

        Physics2D.IgnoreLayerCollision(myLayer, playerLayer, true);

        player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(SkillRoutine());
    }

    private void Update()
    {
        // Cập nhật vị trí player liên tục
        if (player != null)
        {
            lastPlayerPosition = player.position;
        }

    }

    private IEnumerator SkillRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            if (!isSkillActive && !isRandomSkillActive)
            {
                isRandomSkillActive = true;
                int skillIndex = GetRandomSkill();
                yield return StartCoroutine(ActivateSkill(skillIndex));
                isRandomSkillActive = false;

                yield return new WaitForSeconds(Random.Range(3f, 4f));
            }
            else
            {
                yield return null;
            }
        }
    }

    private int GetRandomSkill()
    {
        int skillIndex;
        do
        {
            skillIndex = skillList[Random.Range(0, skillList.Count)];
        } while (skillIndex == lastSkillIndex);

        lastSkillIndex = skillIndex;
        return skillIndex;
    }

    private IEnumerator ActivateSkill(int skillIndex)
    {
        if (isSkillActive) yield break;

        isSkillActive = true;
        FlipCharacter();

        switch (skillIndex)
        {
            case 0:
                Debug.Log("Skill Dash");
                yield return Dash();
                break;
            case 1:
                Vector3 playerPosition = player.transform.position;
                Debug.Log("Skill Tail");
                yield return TailStrong(playerPosition);
                break;
            case 2:
                Debug.Log("Skill Fire Stream");
                yield return FireStreamSkill();
                break;
            case 3:
                Debug.Log("Skill Fire Pillar");
                yield return ActivateFirePillarSkill();
                break;
            case 4:
                Debug.Log("Skill Projectile");
                yield return LaunchProjectile();
                break;
            default:
                Debug.Log("Skill ra khỏi tầm random");
                break;
        }
        isSkillActive = false;
    }

    // Lướt
    private IEnumerator Dash()
    {
        yield return new WaitForSeconds(0.1f);
        if (!isDashing)
        {
            isDashing = true;
            FlipCharacter();
            StartCoroutine(DashCoroutine());
        }
    }

    private void FlipCharacter()
    {
        if (lastPlayerPosition.x > transform.position.x)
        {
            // Quay phải
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (lastPlayerPosition.x < transform.position.x)
        {
            // Quay trái
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private IEnumerator DashCoroutine()
    {
        // Lướt tới vị trí cuối cùng của player
        Vector3 targetPosition = new Vector3(lastPlayerPosition.x, transform.position.y, transform.position.z);
        Debug.Log($"Target Position: {targetPosition}");

        yield return new WaitForSeconds(1.5f);

        // Lướt đến vị trí player
        while (Mathf.Abs(transform.position.x - targetPosition.x) > 0.1f)
        {
            if (speed > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            }
            yield return null;
        }

        transform.position = targetPosition;
        isDashing = false;

        /*Debug.Log("Kết thúc lướt, bắt đầu mọc đuôi.");
        yield return new WaitForSeconds(1f);

        Vector3 tailPosition;
        if (transform.localScale.x > 0)  // Đối tượng quay phải
        {
            tailPosition = transform.position + new Vector3(10f, 0, 0);  // Vị trí đuôi phía bên phải
        }
        else  // Đối tượng quay trái
        {
            tailPosition = transform.position + new Vector3(-15f, 0, 0);  // Vị trí đuôi phía bên trái
        }
        Debug.Log($"Vị trí tạo đuôi: {tailPosition}");
        ActivateTailSkill(tailPosition);*/
    }

    // đuôi
    /*public void ActivateTailSkill(Vector3 position)
    {
        spawnPosition = new Vector3(position.x, position.y - 20f, position.z);

        GameObject tail = Instantiate(tailPrefab, spawnPosition, Quaternion.identity);

        retractPosition = tail.transform.position;

        StartCoroutine(GrowAndRetractTail(tail));
    }*/

    /*private IEnumerator GrowAndRetractTail(GameObject tail)
    {
        float elapsedTime = 0f;
        Vector3 targetPosition = new Vector3(tail.transform.position.x, tail.transform.position.y + 15f, tail.transform.position.z);

        // Tạo đuôi mọc lên
        while (elapsedTime < tailGrowTime)
        {
            elapsedTime += Time.deltaTime;
            tail.transform.position = Vector3.Lerp(retractPosition, targetPosition, elapsedTime / tailGrowTime);
            yield return null;
        }

        elapsedTime = 0f;

        // Tạo đuôi thu lại
        while (elapsedTime < tailRetractTime)
        {
            elapsedTime += Time.deltaTime;
            tail.transform.position = Vector3.Lerp(targetPosition, retractPosition, elapsedTime / tailRetractTime);
            yield return null;
        }

        Destroy(tail);
    }*/

    // phun lửa
    public IEnumerator FireStreamSkill()
    {
        FlipCharacter();

        Vector3 targetPosition = lastPlayerPosition;
        yield return new WaitForSeconds(1f);
        GameObject fireStream = Instantiate(fireStreamPrefab, fireStreamStartPoint.position, Quaternion.identity);

        Vector3 direction = (targetPosition - fireStreamStartPoint.position).normalized;

        float angle = -Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Kiểm tra hướng của nhân vật (trái hoặc phải)
        if (transform.localScale.x < 0) // Quay trái
        {
            // Nếu nhân vật quay trái, cộng thêm 180 độ vào góc
            fireStream.transform.rotation = Quaternion.Euler(angle - 130f, -90f, 0f);
        }
        else // Quay phải
        {
            // Nếu nhân vật quay phải, góc quay tính toán bình thường
            fireStream.transform.rotation = Quaternion.Euler(angle + 10f, 90f, 0f);
        }

        // Bắt đầu việc thổi luồng lửa
        StartCoroutine(FireStreamRoutine(fireStream));
    }

    private IEnumerator FireStreamRoutine(GameObject fireStream)
    {
        yield return new WaitForSeconds(1f);

        Vector3 direction = (lastPlayerPosition - fireStream.transform.position).normalized;

        float elapsedTime = 0f;
        Quaternion initialRotation = fireStream.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x - 70f, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * rotationSpeed;
            fireStream.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime);
            yield return null;
        }

        Destroy(fireStream);
    }

    // cột lửa
    public IEnumerator ActivateFirePillarSkill()
    {
        Vector3 targetPosition = new Vector3(lastPlayerPosition.x, -13f, lastPlayerPosition.z);

        GameObject explosion = Instantiate(explosionPrefab, targetPosition, Quaternion.identity);
        yield return new WaitForSeconds(explosionDuration);
        Destroy(explosion);

        GameObject firePillar = Instantiate(firePillarPrefab, targetPosition, Quaternion.Euler(-90f, 0f, 0f));

        StartCoroutine(FirePillarRoutine(firePillar));
    }

    private IEnumerator FirePillarRoutine(GameObject firePillar)
    {
        Vector3 startPosition = firePillar.transform.position;
        Vector3 endPosition = new Vector3(firePillar.transform.position.x, firePillar.transform.position.y + 10f, firePillar.transform.position.z);

        float elapsedTime = 0f;

        while (elapsedTime < firePillarDuration)
        {
            elapsedTime += Time.deltaTime;
            firePillar.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / firePillarDuration);
            yield return null;
        }

        yield return new WaitForSeconds(firePillarDuration);

        Destroy(firePillar);
    }

    // quả cầu lửa
    private IEnumerator LaunchProjectile()
    {
        // Tạo projectile tại firePoint
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        float elapsedTime = 0f;
        Vector3 startPosition = projectile.transform.position;
        Vector3 targetPosition = startPosition + Vector3.up * 100f;

        while (elapsedTime < launchDuration)
        {
            float moveSpeed = Mathf.Lerp(0, projectileSpeed, elapsedTime / launchDuration);
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        projectile.transform.position = targetPosition;

        Destroy(projectile);
        StartCoroutine(FireballsRain());
    }

    private IEnumerator FireballsRain()
    {
        int fireballCount = Random.Range(minFireballs, maxFireballs);

        for (int i = 0; i < fireballCount; i++)
        {
            Vector3 playerPosition = new Vector3(playerTransform.position.x, fireballFallHeight, playerTransform.position.z);
            Vector3 randomPosition = new Vector3(playerPosition.x + Random.Range(-2f, 2f), fireballFallHeight, playerPosition.z + Random.Range(-2f, 2f));
            GameObject fireball = Instantiate(fireballPrefab, randomPosition, Quaternion.identity);

            yield return new WaitForSeconds(1f);
            StartCoroutine(FireballFall(fireball, playerPosition));
        }
    }

    private IEnumerator FireballFall(GameObject fireball, Vector3 targetPosition)
    {
        float fallSpeed = 10f;
        while (fireball.transform.position.y > targetPosition.y)
        {
            fireball.transform.position = Vector3.MoveTowards(fireball.transform.position, targetPosition, fallSpeed * Time.deltaTime);
            yield return null;
        }
        fireball.transform.position = targetPosition;
    }

    // tấn công bằng đuôi 3 lần
    private IEnumerator TailStrong(Vector3 playerPosition)
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(GrowTail(playerPosition));
    }

    private IEnumerator GrowTail(Vector3 initialPlayerPosition)
    {
        for (int i = 0; i < 3; i++)
        {
            // Lấy vị trí cuối cùng của player trước mỗi lần mọc đuôi
            Vector3 spawnPosition = new Vector3(initialPlayerPosition.x, initialPlayerPosition.y - 20f, initialPlayerPosition.z);
            GameObject tail = Instantiate(tailPrefab, spawnPosition, Quaternion.identity);

            Vector3 retractPosition = tail.transform.position;
            Vector3 targetPosition = new Vector3(tail.transform.position.x, tail.transform.position.y + 15f, tail.transform.position.z);

            float elapsedTime = 0f;

            // Tạo đuôi mọc lên
            while (elapsedTime < tailGrowTime)
            {
                elapsedTime += Time.deltaTime;
                tail.transform.position = Vector3.Lerp(retractPosition, targetPosition, elapsedTime / tailGrowTime);
                yield return null;
            }

            elapsedTime = 0f;

            // Tạo đuôi thu lại
            while (elapsedTime < tailRetractTime)
            {
                elapsedTime += Time.deltaTime;
                tail.transform.position = Vector3.Lerp(targetPosition, retractPosition, elapsedTime / tailRetractTime);
                yield return null;
            }

            Destroy(tail);
            initialPlayerPosition = GetLastKnownPlayerPosition();

            yield return new WaitForSeconds(1f);
        }
    }

    private Vector3 GetLastKnownPlayerPosition()
    {
        return player.transform.position;
    }
}
