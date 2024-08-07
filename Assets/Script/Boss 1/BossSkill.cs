using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill : MonoBehaviour
{
    [Header("Skill Dash")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    private Rigidbody2D rb;
    private bool isDashing = false;
    private float dashTime;
    private Vector2 moveDirection;
    public bool isFacingRight = true;

    [Header("Skill Fire")]
    public Transform firePoint1; 
    public Transform firePoint2; 
    public Transform firePoint3; 
    public Transform firePoint4;
    public GameObject bullet;
    public Transform fireOrigin;
    private Vector3 newScale = new Vector3(6.007486f, 7.745857f, 2.3428f);
    private Vector3 oldScale = new Vector3(6.007486f, 1.405722f, 2.3428f);
    public float speedBullet = 15f;

    [Header("Skill Fire Up")]
    public GameObject smallBullet;
    public int numberSmallBullet;
    public Vector2 minSpawnBullet = new Vector2(-7f, 8.59f);
    public Vector2 maxSpawnBullet = new Vector2(12.87f, 8.59f);
    public float bulletDropSpeed;

    [Header("Skill Spike")]
    public GameObject spikePrefabs;
    public Transform[] spkieSpawn;
    public float timeHideBoss = 2f;
    private SpriteRenderer spriteRenderer;
    private Vector3 positionBoss;
    public float spikeSkillDuration = 10f;
    public float spikeLifeTime = 2f;
    public float spikeRiseDuration = 1f;

    [Header("Skill Poison")]
    public GameObject bulletPoison;
    public GameObject poisonPrefabs;
    public Transform poisonPosition;
    public Transform poisonFirePointLeft;
    public float poisonDuration = 5f;

    [Header("Camera Shake")]
    private CameraShake shake;
    private CameraShake shake1;

    private void Start()
    {
        shake = GameObject.FindGameObjectWithTag("Shake").GetComponent<CameraShake>();
        shake1= GameObject.FindGameObjectWithTag("Shake").GetComponent<CameraShake>();
        rb = GetComponent<Rigidbody2D>();
        moveDirection = Vector2.right;
        spriteRenderer = GetComponent<SpriteRenderer>();
        positionBoss = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing)
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                EndDash();
            }
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            ScaleBoss();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            OldScaleBoss();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(FireProjectiles());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(SpikeSkill());
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(StartPoisonSkill());
        }
    }

    private void ScaleBoss()
    {
        transform.localScale = newScale;
    }

    private void OldScaleBoss()
    {
        transform.localScale = oldScale;
    }

    private void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;
        rb.velocity = moveDirection * dashSpeed;
    }

    private void EndDash()
    {
        isDashing = false;
        rb.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Turn"))
        {
            shake1.CamShake1();
            Flip();
            EndDash();
        }
    }

    private void Flip()
    {
        moveDirection *= -1;
        isFacingRight = !isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private IEnumerator FireProjectiles()
    {
        FireProjectile(firePoint1.position);
        yield return new WaitForSeconds(1f);
        FireProjectile(firePoint2.position);
        yield return new WaitForSeconds(1f);
        FireProjectile(firePoint3.position);
        yield return new WaitForSeconds(1f);
        FireProjectile(firePoint4.position);
        yield return new WaitForSeconds(2f);
        StartCoroutine(SmallBulletDrop());
    }

    private void FireProjectile(Vector3 targetPosition)
    {
        GameObject bulletFire = Instantiate(bullet, fireOrigin.position, Quaternion.identity);
        Vector2 direction = (targetPosition - fireOrigin.position).normalized;
        bulletFire.GetComponent<Rigidbody2D>().velocity = direction * speedBullet;  // Đặt vận tốc cho viên đạn
        Destroy(bulletFire, 5f);
    }

    private IEnumerator SmallBulletDrop()
    {
        Vector2 minSpawn, maxSpawn;

        if (isFacingRight)
        {
            minSpawn = minSpawnBullet;
            maxSpawn = maxSpawnBullet;
        }
        else
        {
            minSpawn = new Vector2(-12.92f, 8.31f);
            maxSpawn = new Vector2(4.97f, 8.31f);
        }

        for (int i = 0; i < numberSmallBullet; i++)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(minSpawn.x, maxSpawn.x),
                minSpawn.y,
                fireOrigin.position.z
            );

            GameObject smallProjectile = Instantiate(smallBullet, spawnPosition, Quaternion.identity);
            smallProjectile.GetComponent<Rigidbody2D>().velocity = Vector2.down * bulletDropSpeed;
            Destroy(smallProjectile, 10f);
            yield return new WaitForSeconds(0.5f); 
        }
    }
    private IEnumerator SpikeSkill()
    {
        spriteRenderer.enabled = false;

        // Thực hiện kỹ năng trong thời gian spikeSkillDuration
        float endTime = Time.time + spikeSkillDuration;

        while (Time.time < endTime)
        {
            foreach (Transform spawnPoint in spkieSpawn)
            {
                GameObject spike = Instantiate(spikePrefabs, spawnPoint.position, Quaternion.identity);
                StartCoroutine(RiseSpike(spike.transform));
                Destroy(spike, spikeLifeTime); // Xóa gai sau spikeLifetime giây

                // Đợi để gai tiếp theo có thể mọc lên
                yield return new WaitForSeconds(2.1f);
            }
        }

        // Hiện lại sprite
        spriteRenderer.enabled = true;
    }

    private IEnumerator RiseSpike(Transform spikeTransform)
    {
        Vector3 startPosition = new Vector3(spikeTransform.position.x, spikeTransform.position.y - 2f, spikeTransform.position.z);
        Vector3 endPosition = spikeTransform.position;

        float elapsedTime = 0f;

        shake.CamShake();

        while (elapsedTime < spikeRiseDuration)
        {
            spikeTransform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / spikeRiseDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spikeTransform.position = endPosition;
    }

    private IEnumerator StartPoisonSkill()
    {
        // Chọn vị trí bắn và tạo độc dựa trên hướng của Boss
        Transform targetPosition = isFacingRight ? poisonPosition : poisonFirePointLeft;

        // Bắn viên đạn độc
        GameObject poisonBullet = Instantiate(bulletPoison, fireOrigin.position, Quaternion.identity);
        Vector2 direction = (targetPosition.position - fireOrigin.position).normalized;
        poisonBullet.GetComponent<Rigidbody2D>().velocity = direction * speedBullet;
        Destroy(poisonBullet, 2f);

        yield return new WaitForSeconds(1f);

        // Tạo khu vực độc tại vị trí mục tiêu
        GameObject poisonArea = Instantiate(poisonPrefabs, targetPosition.position, Quaternion.identity);
        Destroy(poisonArea, poisonDuration);
    }

}
