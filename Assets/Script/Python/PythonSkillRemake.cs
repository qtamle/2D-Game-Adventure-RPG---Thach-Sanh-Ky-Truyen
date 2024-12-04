using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PythonSkillRemake : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] public Animator anim;

    [Header("Health")]
    public HealthBarBoss healthBarBoss; // Tham chiếu tới HealthBarBoss
    private bool isAlive = true; // Trạng thái sống/chết

    [Header("Dash")]
    public float speed = 5f;
    public Transform damageAreaTransform;
    public float damageRadius;
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

    [Header("Camera Shake")]
    public ShakeData spikeShake;

    private bool hasDamaged = false;
    private bool isSkillActive = false;
    private List<int> skillList = new List<int> { 0, 1, 2, 3, 4 };
    private int lastSkillIndex = -1;
    private bool isDowned = false;
    private bool isSkyfallActive = false;
    private bool isRandomSkillActive = false;

    [Header("TimelineCheck")]
    public int timelineIndexToPlay; // Chỉ mục của timeline cần kiểm tra và chạy
    private TimelineManager timelineManager; // Tham chiếu đến TimelineManager

    //private void Awake()
    //{
    //    healthBarBoss = GetComponent<HealthBarBoss>();
    //}
    private IEnumerator Start()
    {
        // Chạy Timeline đầu tiên và đợi cho đến khi hoàn tất
        yield return StartCoroutine(ActiveTimeline1());

        anim = GetComponentInParent<Animator>();
        // Bỏ qua layer 
        int myLayer = gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");

        anim.SetTrigger("Show_Up");
        Physics2D.IgnoreLayerCollision(myLayer, playerLayer, true);

        player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(SkillRoutine());

        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlayBackgroundMusic(0);
                audioManager.PlayEnvironmentMusic(0);
            }
            else
            {
                Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'AudioManager'.");
        }
    }

    private IEnumerator ActiveTimeline1()
    {
        timelineManager = FindObjectOfType<TimelineManager>();

        if (timelineManager == null)
        {
            Debug.LogError("Không tìm thấy TimelineManager trong scene!");
            yield break;  // Dừng coroutine nếu không tìm thấy TimelineManager
        }

        // Kiểm tra xem Timeline đã chạy chưa
        if (timelineManager.HasTimelinePlayed(timelineManager.timelines[timelineIndexToPlay].name))
        {
            Debug.Log($"Timeline {timelineManager.timelines[timelineIndexToPlay].name} đã chạy trước đó, không chạy lại.");
            // Nếu đã chạy rồi, set PlayableDirector thành inactive
            timelineManager.directors[timelineIndexToPlay].gameObject.SetActive(false);
        }
        else
        {
            // Nếu chưa chạy, tiến hành chạy
            timelineManager.PlayTimeline(timelineIndexToPlay);

            // Đăng ký sự kiện khi Timeline kết thúc
            timelineManager.directors[timelineIndexToPlay].stopped += (PlayableDirector d) =>
            {
                OnTimelineComplete(d);
            };

            // Chờ cho đến khi timeline hoàn thành
            yield return new WaitUntil(() => timelineManager.directors[timelineIndexToPlay].state != PlayState.Playing);

            // Sau khi timeline hoàn thành, có thể thực hiện các hành động tiếp theo ở đây
            Debug.Log($"Timeline {timelineManager.timelines[timelineIndexToPlay].name} đã hoàn thành.");
        }
    }

    private void OnTimelineComplete(PlayableDirector director)
    {
        Debug.Log($"Timeline {director.playableAsset.name} đã kết thúc.");
        // Thực hiện các hành động sau khi timeline hoàn tất
        director.stopped -= (PlayableDirector d) => OnTimelineComplete(d); // Hủy đăng ký sự kiện
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

        while (isAlive && healthBarBoss.targetHealth > 0)
        {
            if (!isSkillActive && !isRandomSkillActive)
            {
                isRandomSkillActive = true;
                int skillIndex = GetRandomSkill();
                yield return StartCoroutine(ActivateSkill(skillIndex));
                isRandomSkillActive = false;
                yield return new WaitForSeconds(Random.Range(2f, 3f));
            }
            else
            {
                yield return null;
            }
        }

        // Nếu isAlive là false hoặc health <= 0, kết thúc kỹ năng
        if (healthBarBoss.targetHealth <= 0)
        {
            Debug.Log("Dieeeeee");
            isAlive = false;
            anim.SetTrigger("Snake_Die");
            StopAllCoroutines(); // Dừng toàn bộ kỹ năng
            Destroy(gameObject, 2f);
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
        return 0;
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

    private void FlipCharacter()
    {
        if (lastPlayerPosition.x < transform.position.x)
        {
            // Quay phải
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (lastPlayerPosition.x > transform.position.x)
        {
            // Quay trái
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Lướt
    private IEnumerator Dash()
    {
        isSkillActive = true;

        
        yield return new WaitForSeconds(1f);
        if (!isDashing)
        {
            isDashing = true;
            FlipCharacter();
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        
        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");
        anim.SetTrigger("Snake_Dash");
        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(0);
            }
            else
            {
                Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'AudioManager'.");
        }
        float distance = 19f; 

        // Lướt tới vị trí cuối cùng của player, cộng thêm khoảng cách (tính cả hướng trái/phải)
        float direction = Mathf.Sign(lastPlayerPosition.x - transform.position.x); 
        Vector3 targetPosition = new Vector3(lastPlayerPosition.x - direction * distance, transform.position.y, transform.position.z);
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

        yield return new WaitForSeconds(0.7f);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(damageAreaTransform.position, damageRadius);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // Gây sát thương cho player
                Debug.Log("Đã va chạm với Player khi dash và gây sát thương.");

                PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                StatusEffects status = collider.GetComponentInChildren<StatusEffects>();
                if (playerMovement != null)
                {
                    playerMovement.TakeDamage(15f, 0.5f, 0.65f, 0.1f);
                    status.ApplyBleed();
                }
            }
        }
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

        isSkillActive = false;
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
        isSkillActive = true;
       
        FlipCharacter();
        Vector3 targetPosition = lastPlayerPosition;
        anim.SetTrigger("Snake_FireSteam");
        yield return new WaitForSeconds(2f);
        
        // Tạo luồng lửa từ điểm bắt đầu
        GameObject fireStream = Instantiate(fireStreamPrefab, fireStreamStartPoint.position, Quaternion.Euler(90f,90f,90f), fireStreamStartPoint);

        // Tính toán hướng phun và gán hướng này cho luồng lửa
        Vector3 directionToTarget = targetPosition - fireStreamStartPoint.position;
        fireStream.transform.rotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

        // Bắt đầu việc phun lửa
        StartCoroutine(FireStreamRoutine(fireStream));
    }

    private IEnumerator FireStreamRoutine(GameObject fireStream)
    {
        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(4);
            }
            else
            {
                Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'AudioManager'.");
        }
        yield return new WaitForSeconds(1f);

        float duration = 1f; 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(fireStream);

        isSkillActive = false;
    }

    // cột lửa
    public IEnumerator ActivateFirePillarSkill()
    {
        isSkillActive = false;
        anim.SetTrigger("Snake_FirePillar");
        Vector3 targetPosition = new Vector3(lastPlayerPosition.x, -10f, lastPlayerPosition.z);

        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(1);
            }
            else
            {
                Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'AudioManager'.");
        }
        GameObject explosion = Instantiate(explosionPrefab, targetPosition, Quaternion.identity);
        yield return new WaitForSeconds(explosionDuration);
        Destroy(explosion);

        yield return new WaitForSeconds(1f);
        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(2);
            }
            else
            {
                Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'AudioManager'.");
        }
        GameObject firePillar = Instantiate(firePillarPrefab, targetPosition, Quaternion.Euler(-90f, 0f, 0f));

        StartCoroutine(FirePillarRoutine(firePillar));
    }

    private IEnumerator FirePillarRoutine(GameObject firePillar)
    {
        Vector3 startPosition = firePillar.transform.position;
        Vector3 endPosition = new Vector3(firePillar.transform.position.x, firePillar.transform.position.y + 50f, firePillar.transform.position.z);

        float elapsedTime = 0f;
        while (elapsedTime < firePillarDuration)
        {
            elapsedTime += Time.deltaTime;
            firePillar.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / firePillarDuration);
            yield return null;
        }

        yield return new WaitForSeconds(firePillarDuration);

        Destroy(firePillar);

        isSkillActive = false;
    }

    // quả cầu lửa
    private IEnumerator LaunchProjectile()
    {
        isSkillActive = true;
        // Tạo projectile tại firePoint
        anim.SetTrigger("Snake_Projectile");
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        float elapsedTime = 0f;
        Vector3 startPosition = projectile.transform.position;
        Vector3 targetPosition = startPosition + Vector3.up * 100f;

        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(3);
            }
            else
            {
                Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag 'AudioManager'.");
        }
        while (elapsedTime < launchDuration)
        {
            float moveSpeed = Mathf.Lerp(0, projectileSpeed, elapsedTime / launchDuration);
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        projectile.transform.position = targetPosition;

        Destroy(projectile,1f);
        isSkillActive = false;
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
        float fallSpeed = 60f;
        while (fireball.transform.position.y > targetPosition.y)
        {
            fireball.transform.position = Vector3.MoveTowards(fireball.transform.position, targetPosition, fallSpeed * Time.deltaTime);
            yield return null;
        }
        fireball.transform.position = targetPosition;
    }

    // tấn công bằng đuôi
    private IEnumerator TailStrong(Vector3 playerPosition)
    {
        anim.SetTrigger("Snake_Tail");
        isSkillActive = true;
        yield return new WaitForSeconds(1f);
        StartCoroutine(GrowTail(playerPosition));
        isSkillActive = false;
    }

    private IEnumerator GrowTail(Vector3 initialPlayerPosition)
    {
        for (int i = 0; i < 1; i++)
        {
            // Lấy vị trí cuối cùng của player trước mỗi lần mọc đuôi
            Vector3 spawnPosition = new Vector3(initialPlayerPosition.x, initialPlayerPosition.y - 20f, initialPlayerPosition.z);
            GameObject tail = Instantiate(tailPrefab, spawnPosition, Quaternion.identity);

            Vector3 retractPosition = tail.transform.position;
            Vector3 targetPosition = new Vector3(tail.transform.position.x, tail.transform.position.y + 15f, tail.transform.position.z);

            float elapsedTime = 0f;

            GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

            if (audioManagerObject != null)
            {
                AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

                if (audioManager != null)
                {
                    audioManager.PlaySFX(6);
                }
                else
                {
                    Debug.LogError("AudioManager component not found on the GameObject with the tag 'AudioManager'.");
                }
            }
            else
            {
                Debug.LogError("No GameObject found with the tag 'AudioManager'.");
            }
            CameraShakerHandler.Shake(spikeShake);
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

    void OnDrawGizmosSelected()
    {
        // Kiểm tra nếu đối tượng và transform không null trước khi truy cập
        if (player != null && player.transform != null)
        {
            Gizmos.DrawWireSphere(player.transform.position, 1f);
        }
        else
        {
            Debug.LogWarning("Player or Player's Transform is null!");
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(damageAreaTransform.position, damageRadius);
    }

}
