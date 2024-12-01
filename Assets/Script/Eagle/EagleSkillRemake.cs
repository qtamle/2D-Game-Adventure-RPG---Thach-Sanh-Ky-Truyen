using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class EagleSkillRemake : MonoBehaviour
{
    [Header("Hovering")]
    public float hoverHeight = 1.2f;
    public float hoverSpeed = 2f;
    public bool isHovering = true;
    private Vector3 originalPosition;

    [Header("Wind Cut")]
    public GameObject windCut;
    public float windSpeed;
    public float windDuration;
    public LayerMask targetLayer;
    private bool isWindCutActive = false;

    [Header("Feather Shot")]
    public GameObject featherPrefab;
    public float featherSpeed;
    public int featherCount = 4;
    public float spreadAngle = 30f;
    private bool isShootingFeathers = false;

    [Header("Pick up")]
    public float teleportSpeed;
    public float teleportWaitTime;
    private Vector3 lastPlayerPosition;
    public Transform layerGround;

    [Header("Tornado Skill")]
    public GameObject tornadoPrefab;
    public float tornadoDuration = 3f;
    public float pullForce = 5f;
    public Transform tornadoSpawn;
    private GameObject currentTornado;

    [Header("Skyfall")]
    public float descentHeight = 2f;
    public float ascentHeight = 5f;
    public float skyfallSpeed;
    public float fastAscentSpeed;
    private Vector3 playerPosition;
    public float attackRadius;

    [Header("Land At Position")]
    public Transform landingPosition;
    public float landingSpeed = 1f;
    private bool isLanding = false;

    [Header("Other")]
    public Transform player;
    public Transform attackSkyfall;
    public LayerMask playerMask;
    public ParticleSystem smoke;
    public Transform smokeSpawn;

    [Header("Camera Shake")]
    public ShakeData stompShake;
    public ShakeData fallShake;

    private bool hasDamaged = false;
    private bool isSkillActive = false;
    private List<int> skillList = new List<int> {0,1,2,3,4};
    private int lastSkillIndex = -1;
    private bool isDowned = false;
    private bool isSkyfallActive = false;
    private bool isRandomSkillActive = false;

    [SerializeField] EagleHealthbar eagleHealth;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalPosition = transform.position;

        // bỏ qua layer
        int myLayer = gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");

        Physics2D.IgnoreLayerCollision(myLayer, playerLayer, true);

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

    private IEnumerator SkillRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            if (eagleHealth.shield <= 0)
            {
                Debug.Log("Shield đã hết, tạm dừng random skill để thực hiện SkyfallSkillAlt");
                isSkillActive = true;
                yield return StartCoroutine(SkyfallSkillAlt());
                isSkillActive = false;
            }
            else if (!isSkillActive && !isLanding && eagleHealth.health > 0 && !isDowned)
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
    }

    private int GetRandomSkill()
    {
        int skillIndex;
        do
        {
            GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

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
            skillIndex = skillList[Random.Range(0, skillList.Count)];
        } while (skillIndex == lastSkillIndex);

        lastSkillIndex = skillIndex;
        return skillIndex;
    }

    private IEnumerator ActivateSkill(int skillIndex)
    {
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

        if (eagleHealth.shield <= 0)
        {
            Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt");
            isSkyfallActive = false;
            isSkillActive = false; 
            yield return StartCoroutine(SkyfallSkillAlt());
            yield break;
        }

        isSkillActive = true;

        switch(skillIndex)
        {
            case 0:
                Debug.Log("Skill Wind Cut");
                yield return WindCutSkill();
                break;
            case 1:
                Debug.Log("Skill Feather");
                yield return ShootFeathers();
                break;
            case 2:
                Debug.Log("Skill Pickup Player");
                yield return PickupPlayer();
                break;
            case 3:
                Debug.Log("Skill Tornado");
                yield return ActivateTornadoSkill();
                break;
            case 4:
                Debug.Log("Skill Skyfall");
                yield return SkyfallSkill();
                break;
            default:
                Debug.Log("Skill ra khỏi tầm random");
                break;
        }
        isSkillActive = false;
    }

    // hovering
    private void Hover()
    {
        if (isHovering)
        {
            float newY = originalPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
            transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    // wind cut
    private IEnumerator WindCutSkill()
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
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            if (eagleHealth.shield <= 0)
            {
                Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt");
                isSkyfallActive = false;
                yield return StartCoroutine(SkyfallSkillAlt());
                yield break;  
            }

            isWindCutActive = true;

            GameObject wind = Instantiate(windCut, transform.position, Quaternion.identity);

            Vector3 playerPosition = player.transform.position;
            Vector3 direction = (playerPosition - transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            wind.transform.rotation = Quaternion.Euler(0, 0, angle + 95f);

            StartCoroutine(MoveWind(wind, direction));

            yield return new WaitUntil(() => !isWindCutActive || eagleHealth.shield <= 0);

            if (eagleHealth.shield <= 0)
            {
                Debug.Log("Shield đã hết trong lúc sử dụng Wind Cut, chuyển sang SkyfallSkillAlt");
                isSkyfallActive = false;
                yield return StartCoroutine(SkyfallSkillAlt());
            }
        }
        else
        {
            Debug.LogWarning("Player not found! The wind cut skill won't be performed.");
        }
    }

    private IEnumerator MoveWind(GameObject skill, Vector3 direction)
    {
        if (skill == null) yield break;

        float timeElapsed = 0f;

        while (timeElapsed < windDuration)
        {
            if (skill != null)
            {
                skill.transform.position += direction * windSpeed * Time.deltaTime;
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (skill != null)
        {
            Destroy(skill);
        }

        isWindCutActive = false;
    }

    // shoot feather
    private IEnumerator ShootFeathers()
    {
        if (eagleHealth.shield <= 0)
        {
            Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt trong ShootFeathers");
            isSkyfallActive = false;
            yield return StartCoroutine(SkyfallSkillAlt());
            yield break;
        }

        isShootingFeathers = true;

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            Vector2 lastKnownPlayerPosition = player.transform.position;
            Vector2 directionToPlayer = (lastKnownPlayerPosition - (Vector2)transform.position).normalized;

            float startAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - spreadAngle / 2f;

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
            // Bắn đợt đầu tiên
            for (int i = 0; i < featherCount; i++)
            {
                // Kiểm tra shield trong suốt quá trình bắn
                if (eagleHealth.shield <= 0)
                {
                    Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt trong quá trình bắn đợt đầu");
                    isSkyfallActive = false;
                    yield return StartCoroutine(SkyfallSkillAlt());
                    yield break;  
                }

                float currentAngle = startAngle + (spreadAngle / (featherCount - 1)) * i;
                Vector2 direction = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
                ShootFeather(direction);
            }

            yield return new WaitForSeconds(1.5f);

            // Kiểm tra lại shield sau đợt bắn đầu tiên
            if (eagleHealth.shield <= 0)
            {
                Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt sau đợt bắn đầu tiên");
                isSkyfallActive = false;
                yield return StartCoroutine(SkyfallSkillAlt());
                yield break;
            }

            float secondAngleStart = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 50f / 2f;

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
            // Bắn đợt thứ hai
            for (int i = 0; i < featherCount; i++)
            {
                // Kiểm tra lại shield trong suốt quá trình bắn đợt thứ hai
                if (eagleHealth.shield <= 0)
                {
                    Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt trong quá trình bắn đợt thứ hai");
                    isSkyfallActive = false;
                    yield return StartCoroutine(SkyfallSkillAlt());
                    yield break;  
                }

                float currentAngle = secondAngleStart + ((spreadAngle + 20f) / (featherCount - 1)) * i;
                Vector2 direction = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
                ShootFeather(direction);
            }
        }
        else
        {
            Debug.LogWarning("Player not found! Feathers cannot be shot.");
        }

        isShootingFeathers = false;
    }

    private void ShootFeather(Vector2 direction)
    {
        GameObject feather = Instantiate(featherPrefab, transform.position, Quaternion.identity);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        feather.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = feather.GetComponent<Rigidbody2D>();
        rb.velocity = direction * featherSpeed;

        Destroy(feather, 2f);
    }

    // pick up player
    private IEnumerator PickupPlayer()
    {
        if (eagleHealth.shield <= 0)
        {
            Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt");
            isSkyfallActive = false;
            yield return StartCoroutine(SkyfallSkillAlt());
            yield break;
        }

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            Debug.Log("Last player position: " + lastPlayerPosition);

            // Di chuyển tới vị trí player
            while (Vector3.Distance(transform.position, lastPlayerPosition) > 0.1f)
            {
                if (eagleHealth.shield <= 0)
                {
                    Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt");
                    isSkyfallActive = false;
                    yield return StartCoroutine(SkyfallSkillAlt());
                    yield break;
                }
                Vector3 lastPlayerPosition = player.transform.position;
                transform.position = Vector3.MoveTowards(transform.position, lastPlayerPosition, teleportSpeed * Time.deltaTime);

                Collider2D[] colliders = Physics2D.OverlapCircleAll(attackSkyfall.position, attackRadius);
                bool hasDamaged = false;
                foreach (var collider in colliders)
                {
                    if (collider.CompareTag("Player") && !hasDamaged)
                    {
                        Debug.Log("Đã va chạm với Player khi hạ cánh.");
                        PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                        if (playerMovement != null)
                        {
                            playerMovement.TakeDamage(15f, 0.5f, 0.65f, 0.1f);
                        }
                        hasDamaged = true;
                    }
                }

                // Kiểm tra nếu đã chạm đất ngay trong quá trình di chuyển
                if (IsGrounded())
                {
                    ParticleSystem smokeEffect = Instantiate(smoke, smokeSpawn.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
                    StartCoroutine(DestroyAfterTime(smokeEffect, 5f));
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

                    Debug.Log("Chạm đất, dừng lại 0,5 giây");
                    yield return new WaitForSeconds(0.2f);
                    break; 
                }

                yield return null;
            }

            // Bay chéo thẳng
            Vector3 targetPosition = transform.position + new Vector3(50f, 50f, 0f);
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                if (eagleHealth.shield <= 0)
                {
                    Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt");
                    isSkyfallActive = false;
                    yield return StartCoroutine(SkyfallSkillAlt());
                    yield break;
                }
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, teleportSpeed * Time.deltaTime);
                yield return null;
            }
            Play(0);
            StartCoroutine(LandAtPosition());
        }
        else
        {
            Debug.LogWarning("Player not found! The teleport skill won't be performed.");
        }
    }

    private bool IsGrounded()
    {

        Collider2D hit = Physics2D.OverlapCircle(layerGround.position, 1f, LayerMask.GetMask("Ground"));
        return hit != null;

    }

    // hạ cánh khi xong skill lướt
    private IEnumerator LandAtPosition()
    {
        if (eagleHealth.shield <= 0)
        {
            Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt");
            isSkyfallActive = false;
            yield return StartCoroutine(SkyfallSkillAlt());
            yield break; 
        }

        isLanding = true;
        Vector3 spawnPosition = landingPosition.position;
        transform.position = spawnPosition;

        float landingTime = 4.5f;
        float elapsedTime = 0f;

        Vector3 initialPosition = transform.position;

        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

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
        while (elapsedTime < landingTime)
        {
            if (eagleHealth.shield <= 0)
            {
                Debug.Log("Shield đã hết trong quá trình hạ cánh, chuyển sang SkyfallSkillAlt");
                isSkyfallActive = false;
                yield return StartCoroutine(SkyfallSkillAlt());
                yield break; 
            }
            float lerpValue = Mathf.Clamp01(elapsedTime / (landingTime / landingSpeed));
            transform.position = Vector3.Lerp(initialPosition, new Vector3(initialPosition.x, 0f, initialPosition.z), lerpValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1.2f);
        isLanding = false;
    }

    // lốc xoáy
    private IEnumerator ActivateTornadoSkill()
    {
        if (eagleHealth.shield <= 0)
        {
            Destroy(currentTornado);
            Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt");
            isSkyfallActive = false;
            yield return StartCoroutine(SkyfallSkillAlt());
            yield break;
        }

        yield return new WaitForSeconds(1f);

        currentTornado = Instantiate(tornadoPrefab, tornadoSpawn.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));

        float elapsedTime = 0f;
        float initialPullForce = pullForce;
        float velocity = 0f;

        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(7);
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
        while (elapsedTime < tornadoDuration)
        {
            if (eagleHealth.shield <= 0)
            {
                Destroy(currentTornado);
                Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt");
                isSkyfallActive = false;
                yield return StartCoroutine(SkyfallSkillAlt());
                yield break;
            }

            elapsedTime += Time.deltaTime;

            if (player != null)
            {
                Vector2 tornadoPosition = currentTornado.transform.position;

                float smoothPullForce = Mathf.SmoothDamp(initialPullForce, pullForce, ref velocity, 0.3f);

                // Kéo player về phía tornado
                player.position = new Vector2(
                    Mathf.MoveTowards(player.position.x, tornadoPosition.x, smoothPullForce * Time.deltaTime),
                    player.position.y
                );
            }

            yield return null;
        }
        Destroy(currentTornado);
    }

    // lao thẳng xuống
    public IEnumerator SkyfallSkill()
    {
        if (isSkyfallActive)
        {
            yield break; // Tránh gọi lại khi kỹ năng đã được kích hoạt
        }

        isSkyfallActive = true;

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            while (eagleHealth.shield > 0)
            {
                playerPosition = player.transform.position;
                float descentTime = descentHeight / skyfallSpeed;
                float elapsedDescent = 0f;
                Vector3 originalPosition = transform.position;

                // Di chuyển xuống
                while (elapsedDescent < descentTime)
                {
                    if (eagleHealth.shield <= 0)
                    {
                        Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt");
                        isSkyfallActive = false;  
                        yield return StartCoroutine(SkyfallSkillAlt()); 
                        yield break; 
                    }

                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(originalPosition.x, originalPosition.y - descentHeight, originalPosition.z), skyfallSpeed * Time.deltaTime);
                    elapsedDescent += Time.deltaTime;
                    yield return null;
                }

                // Di chuyển lên
                Vector3 ascentTarget = new Vector3(originalPosition.x, originalPosition.y + ascentHeight, originalPosition.z);
                float ascentTime = ascentHeight / fastAscentSpeed;
                float elapsedAscent = 0f;

                while (elapsedAscent < ascentTime)
                {
                    if (eagleHealth.shield <= 0)
                    {
                        Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt");
                        isSkyfallActive = false; 
                        yield return StartCoroutine(SkyfallSkillAlt()); 
                        yield break;
                    }

                    transform.position = Vector3.MoveTowards(transform.position, ascentTarget, fastAscentSpeed * Time.deltaTime);
                    elapsedAscent += Time.deltaTime;
                    yield return null;
                }

                // Di chuyển đến vị trí mới
                playerPosition = player.transform.position;
                Vector3 spawnPosition = new Vector3(playerPosition.x, playerPosition.y + 35f, playerPosition.z);
                transform.position = spawnPosition;

                float fallSpeed = 40f;
                bool hasDamaged = false;
                while (Vector3.Distance(transform.position, playerPosition) > 0.1f)
                {
                    if (eagleHealth.shield <= 0)
                    {
                        Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt");
                        isSkyfallActive = false;  
                        yield return StartCoroutine(SkyfallSkillAlt()); 
                        yield break; 
                    }

                    transform.position = Vector3.MoveTowards(transform.position, playerPosition, fallSpeed * Time.deltaTime);
                    yield return null;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(attackSkyfall.position, attackRadius);
                    foreach (var collider in colliders)
                    {
                        if (collider.CompareTag("Player") && !hasDamaged)  
                        {
                            Debug.Log("Đã va chạm với Player khi hạ cánh.");
                            PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                            if (playerMovement != null)
                            {
                                playerMovement.TakeDamage(15f, 0.5f, 0.65f, 0.1f);
                            }
                            hasDamaged = true; 
                        }
                    }

                    if (IsGrounded())
                    {
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
                        if (eagleHealth.shield <= 0)
                        {
                            ParticleSystem smokeEffect = Instantiate(smoke, smokeSpawn.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
                            StartCoroutine(DestroyAfterTime(smokeEffect, 5f));
                            CameraShakerHandler.Shake(fallShake);
                            isDowned = true;
                            Debug.Log("Đã gục trong 7 giây");
                            yield return new WaitForSeconds(7f);
                            isDowned = false;
                        }
                        else
                        {
                            ParticleSystem smokeEffect = Instantiate(smoke, smokeSpawn.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
                            StartCoroutine(DestroyAfterTime(smokeEffect, 5f));
                            CameraShakerHandler.Shake(stompShake);
                            Debug.Log("Chạm đất, dừng lại 0,5 giây");
                            yield return new WaitForSeconds(0.2f);
                        }
                        break;
                    }
                }
                // Bay chéo thẳng
                Vector3 targetPosition = transform.position + new Vector3(50f, 50f, 0f);
                while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    if (eagleHealth.shield <= 0)
                    {
                        Debug.Log("Shield đã hết, chuyển sang SkyfallSkillAlt");
                        isSkyfallActive = false;  
                        yield return StartCoroutine(SkyfallSkillAlt()); 
                        yield break; 
                    }

                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, teleportSpeed * Time.deltaTime);
                    yield return null;
                }
                Play(0);
                StartCoroutine(LandAtPosition());
                break;
            }
        }
        else
        {
            Debug.LogWarning("Player not found! Skyfall skill won't be performed.");
        }

        isSkyfallActive = false;
    }

    public IEnumerator SkyfallSkillAlt()
    {
        if (isSkyfallActive)
        {
            yield break; 
        }

        isSkyfallActive = true;

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // Di chuyển xuống với tốc độ giảm dần
            float descentTime = descentHeight / skyfallSpeed;
            float elapsedDescent = 0f;
            Vector3 originalPosition = transform.position;

            while (elapsedDescent < descentTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(originalPosition.x, originalPosition.y - descentHeight, originalPosition.z), skyfallSpeed * Time.deltaTime);
                elapsedDescent += Time.deltaTime;
                yield return null;
            }

            // Di chuyển lên nhanh hơn
            Vector3 ascentTarget = new Vector3(originalPosition.x, originalPosition.y + ascentHeight, originalPosition.z);
            float ascentTime = ascentHeight / fastAscentSpeed;
            float elapsedAscent = 0f;

            while (elapsedAscent < ascentTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, ascentTarget, fastAscentSpeed * Time.deltaTime);
                elapsedAscent += Time.deltaTime;
                yield return null;
            }

            // Di chuyển đến vị trí của player
            playerPosition = player.transform.position;
            Vector3 spawnPosition = new Vector3(playerPosition.x, playerPosition.y + 35f, playerPosition.z);
            transform.position = spawnPosition;

            float fallSpeed = 35f;
            bool hasDamaged = false;
            while (Vector3.Distance(transform.position, playerPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerPosition, fallSpeed * Time.deltaTime);
                yield return null;

                Collider2D[] colliders = Physics2D.OverlapCircleAll(attackSkyfall.position, attackRadius);
                foreach (var collider in colliders)
                {
                    if (collider.CompareTag("Player") && !hasDamaged)
                    {
                        Debug.Log("Đã va chạm với Player khi hạ cánh.");
                        PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                        if (playerMovement != null)
                        {
                            playerMovement.TakeDamage(20f, 0.5f, 0.65f, 0.1f);
                        }
                        hasDamaged = true;
                    }
                }

                if (IsGrounded())
                {
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
                    if (eagleHealth.shield <= 0)
                    {
                        ParticleSystem smokeEffect = Instantiate(smoke, smokeSpawn.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
                        StartCoroutine(DestroyAfterTime(smokeEffect, 5f));
                        CameraShakerHandler.Shake(fallShake);
                        isDowned = true;
                        Debug.Log("Đã gục trong 7 giây");
                        yield return new WaitForSeconds(7f);
                        isDowned = false;
                    }
                    else
                    {
                        ParticleSystem smokeEffect = Instantiate(smoke, smokeSpawn.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
                        StartCoroutine(DestroyAfterTime(smokeEffect, 5f));
                        CameraShakerHandler.Shake(stompShake);
                        Debug.Log("Chạm đất, dừng lại 0,5 giây");
                        yield return new WaitForSeconds(0.2f);
                    }
                    break;
                }
            }
            Vector3 targetPosition = transform.position + new Vector3(50f, 50f, 0f);
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, teleportSpeed * Time.deltaTime);
                yield return null;
            }
            Play(0);
            StartCoroutine(LandAtPosition());
        }
        else
        {
            Debug.LogWarning("Player not found! SkyfallSkillAlt won't be performed.");
        }

        isSkyfallActive = false;
    }

    private void Play(int soundIndex)
    {
        GameObject audioManagerObject = GameObject.FindWithTag("AudioManager");

        if (audioManagerObject != null)
        {
            AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();

            if (audioManager != null)
            {
                audioManager.PlaySFX(soundIndex);
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

    IEnumerator DestroyAfterTime(ParticleSystem ps, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(ps.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(layerGround.position, 1f);

        if (attackSkyfall != null)
        {
            Gizmos.color = Color.red; 
            Gizmos.DrawWireSphere(attackSkyfall.position, attackRadius); 
        }

    }

}
