using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Header("Skill Dash")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    private Rigidbody2D rb;
    private bool isDashing = false;
    private float dashTime;
    private Vector2 moveDirection;
    public bool isFacingRight = true;
    private bool hasDamaged = false;

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
    public SpriteRenderer spriteRendererSpike;
    private Vector3 positionBoss;
    public float spikeSkillDuration = 12.6f;
    public float spikeLifeTime = 2f;
    public float spikeRiseDuration = 1f;
    public float spikeLowerDuration = 1f;
    private List<int> availableIndices;
    public GameObject warningMarkPrefab;
    public float damageAmount = 10f;

    private List<GameObject> warningMarks = new List<GameObject>();
    private List<GameObject> spikes = new List<GameObject>();

    [Header("Skill Poison")]
    public GameObject bulletPoison;
    public GameObject poisonPrefabs;
    public Transform poisonPosition;
    public Transform poisonFirePointLeft;
    public float poisonDuration = 5f;

    [Header("Camera Shake")]
    private CameraShake shake;
    private CameraShake shake1;

    private PlayerMovement playerMovement;

    private bool isPhase2Activated = false;
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        shake = GameObject.FindGameObjectWithTag("Shake").GetComponent<CameraShake>();
        shake1 = GameObject.FindGameObjectWithTag("Shake").GetComponent<CameraShake>();
        rb = GetComponent<Rigidbody2D>();
        moveDirection = Vector2.right;

        spriteRendererSpike = GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        positionBoss = transform.position;

        availableIndices = new List<int>();
        for (int i = 0; i < spkieSpawn.Length; i++)
        {
            availableIndices.Add(i);
        }

        StartCoroutine(StartSkillsWithDelay());

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);
    }


    private IEnumerator StartSkillsWithDelay()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(ExecuteRandomSkills());
    }

    private IEnumerator ExecuteRandomSkills()
    {
        while (!isPhase2Activated)
        {
            // Chọn kỹ năng ngẫu nhiên để thực hiện
            int skillIndex = Random.Range(0,3); 

            switch (skillIndex)
            {
                case 0:
                    yield return StartCoroutine(AutoDash());
                    break;
                case 1:
                    yield return StartCoroutine(FireProjectiles());
                    break;
                case 2:
                    yield return StartCoroutine(SpikeSkill());
                    break;
            }

            // Đợi 4 giây trước khi thực hiện kỹ năng tiếp theo
            yield return new WaitForSeconds(4f);
        }
    }

    private IEnumerator AutoDash()
    {
        if (isPhase2Activated) yield break;

        int dashCount = Random.Range(2, 5);

        for (int i = 0; i < dashCount; i++)
        {
            StartDash(); // Bắt đầu Dash
            yield return new WaitForSeconds(1f); 

            EndDash(); // Kết thúc Dash

            if (i < dashCount - 1) 
            {
                yield return new WaitForSeconds(0.5f); 
            }
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

        hasDamaged = false;
        StartCoroutine(CheckPlayerCollision());
    }

    private void EndDash()
    {
        isDashing = false;
        rb.velocity = Vector2.zero;
    }
    private IEnumerator CheckPlayerCollision()
    {
        while (isDashing)
        {
            // Kiểm tra va chạm với người chơi
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 10f);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    if (!hasDamaged)
                    {
                        if (playerMovement != null)
                        {
                            playerMovement.TakeDamage(10f, 1.5f, 0.65f, 0.1f);
                            hasDamaged = true;  
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
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
        if (isPhase2Activated) yield break;

        // Tìm đối tượng có tag là "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Bắn các viên đạn về phía vị trí của Player tại mỗi lần bắn
            for (int i = 0; i < 4; i++)
            {
                Vector3 playerPosition = player.transform.position; // Lưu vị trí hiện tại của Player
                FireProjectile(playerPosition);
                yield return new WaitForSeconds(2f);
            }

            // Bắn thêm một viên đạn lên vị trí chỉ định
            Vector3 finalPlayerPosition = player.transform.position; // Lưu vị trí cuối cùng của Player
            Vector3 designatedPosition = new Vector3(finalPlayerPosition.x, finalPlayerPosition.y + 10f, finalPlayerPosition.z);
            FireProjectile(designatedPosition);
            yield return new WaitForSeconds(1f);

            // Thả 20 viên đạn nhỏ từ trên cao
            numberSmallBullet = 20;
            StartCoroutine(SmallBulletDrop());
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }
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
        if (isPhase2Activated) yield break;

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
        if (isPhase2Activated) yield break;

        for (int i = 0; i < 6; i++)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                Vector3 playerPosition = player.transform.position;
                Vector3 spawnPosition;

                RaycastHit2D hit = Physics2D.Raycast(playerPosition, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

                if (hit.collider != null)
                {
                    spawnPosition = new Vector3(playerPosition.x, hit.point.y, playerPosition.z);
                }
                else
                {
                    spawnPosition = playerPosition;
                }

                Vector3 offset = new Vector3(Random.Range(-2f, 2f), 1f, 0);
                spawnPosition += offset;

                GameObject warningMark = Instantiate(warningMarkPrefab, spawnPosition, Quaternion.identity);
                warningMarks.Add(warningMark);

                yield return new WaitForSeconds(0.5f);

                GameObject spike = Instantiate(spikePrefabs, warningMark.transform.position, Quaternion.identity);
                spikes.Add(spike);

                SpikeCollision spikeCollision = spike.GetComponent<SpikeCollision>();
                if (spikeCollision == null)
                {
                    spikeCollision = spike.AddComponent<SpikeCollision>();
                }
                spikeCollision.damageAmount = 10f;

                StartCoroutine(RiseSpike(spike.transform));
                StartCoroutine(LowerAndDestroySpike(spike.transform));

                Destroy(warningMark);

                yield return new WaitForSeconds(2f);
            }
            else
            {
                Debug.LogWarning("Player not found!");
                break;
            }
        }
    }

    public void DestroyAllSpikeEffects()
    {
        foreach (GameObject warningMark in warningMarks)
        {
            if (warningMark != null)
            {
                Destroy(warningMark);
            }
        }
        warningMarks.Clear();

        foreach (GameObject spike in spikes)
        {
            if (spike != null)
            {
                Destroy(spike);
            }
        }
        spikes.Clear();
    }

    private int GetRandomIndex()
    {
        if (availableIndices.Count == 0)
        {
            for (int i = 0; i < spkieSpawn.Length; i++)
            {
                availableIndices.Add(i);
            }
        }

        int randomIndex = Random.Range(0, availableIndices.Count);
        int index = availableIndices[randomIndex];
        availableIndices.RemoveAt(randomIndex);

        return index;
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

    private IEnumerator LowerAndDestroySpike(Transform spikeTransform)
    {
        // Chờ spikeLifeTime trước khi bắt đầu hạ spike
        yield return new WaitForSeconds(spikeLifeTime - spikeLowerDuration);

        Vector3 startPosition = spikeTransform.position;
        Vector3 endPosition = new Vector3(spikeTransform.position.x, spikeTransform.position.y - 2f, spikeTransform.position.z);

        float elapsedTime = 0f;

        while (elapsedTime < spikeLowerDuration)
        {
            spikeTransform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / spikeLowerDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spikeTransform.position = endPosition;

        Destroy(spikeTransform.gameObject);
    }
    private IEnumerator StartPoisonSkill()
    {
        // Tìm đối tượng có tag là "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            float speedPosion = 10f;
            Vector3 playerPosition = player.transform.position;

            // Chọn vị trí bắn và tạo độc dựa trên hướng của Boss
            Transform targetPosition = isFacingRight ? poisonPosition : poisonFirePointLeft;

            // Bắn viên đạn độc về phía người chơi
            GameObject poisonBullet = Instantiate(bulletPoison, fireOrigin.position, Quaternion.identity);
            Vector2 direction = (playerPosition - fireOrigin.position).normalized;
            poisonBullet.GetComponent<Rigidbody2D>().velocity = direction * speedPosion;
            Destroy(poisonBullet, 2f);

            yield return new WaitForSeconds(2f);

            Vector3 adjustedPoisonPosition = new Vector3(playerPosition.x - 3f, playerPosition.y, playerPosition.z);

            // Tạo khu vực độc tại vị trí điều chỉnh
            GameObject poisonArea = Instantiate(poisonPrefabs, adjustedPoisonPosition, Quaternion.identity);
            Destroy(poisonArea, poisonDuration);
        }
    }

    public void ActivatePhase2()
    {
        isPhase2Activated = true;
        StopAllCoroutines(); 
    }
}
