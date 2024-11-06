using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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

    [Header("Feather Shot")]
    public GameObject featherPrefab;
    public float featherSpeed;
    public int featherCount = 4;
    public float spreadAngle = 30f;

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

    [Header("Land At Position")]
    public Transform landingPosition;
    public float landingSpeed = 1f;

    [Header("Other")]
    public Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalPosition = transform.position;

        // bỏ qua layer
        int myLayer = gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");

        Physics2D.IgnoreLayerCollision(myLayer, playerLayer, true);

    }

    private void Update()
    {
        /*if (isHovering)
        {
            float newY = originalPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
            transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);
        }
        else
        {
            transform.position = originalPosition;
        }*/

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P key pressed, starting PickupPlayer skill");
            StartCoroutine(SkyfallSkill());
        }
    }

    // wind cut
    private void WindCutSkill()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null) 
        {
            GameObject wind = Instantiate(windCut, transform.position, Quaternion.identity);

            Vector3 playerPosition = player.transform.position;
            Vector3 direction = (playerPosition - transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            wind.transform.rotation = Quaternion.Euler(0, 0, angle - 110); 

            StartCoroutine(MoveWind(wind, direction)); 
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
    }

    // shoot feather
    private IEnumerator ShootFeathers()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null) 
        {
            Vector2 lastKnownPlayerPosition = player.transform.position;

            Vector2 directionToPlayer = (lastKnownPlayerPosition - (Vector2)transform.position).normalized;

            float startAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - spreadAngle / 2f;

            for (int i = 0; i < featherCount; i++)
            {
                float currentAngle = startAngle + (spreadAngle / (featherCount - 1)) * i;
                Vector2 direction = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
                ShootFeather(direction);
            }

            yield return new WaitForSeconds(1f);
        }
        else
        {
            Debug.LogWarning("Player not found! Feathers cannot be shot.");
        }
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
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            Debug.Log("Last player position: " + lastPlayerPosition);

            // Di chuyển tới vị trí player
            while (Vector3.Distance(transform.position, lastPlayerPosition) > 0.1f)
            {
                Vector3 lastPlayerPosition = player.transform.position;
                transform.position = Vector3.MoveTowards(transform.position, lastPlayerPosition, teleportSpeed * Time.deltaTime);

                // Kiểm tra nếu đã chạm đất ngay trong quá trình di chuyển
                if (IsGrounded())
                {
                    Debug.Log("Chạm đất, dừng lại 0,5 giây");
                    yield return new WaitForSeconds(0.5f);
                    break; 
                }

                yield return null;
            }

            // Bay chéo thẳng
            Vector3 targetPosition = transform.position + new Vector3(50f, 50f, 0f);
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, teleportSpeed * Time.deltaTime);
                yield return null;
            }

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

    private IEnumerator LandAtPosition()
    {
        Vector3 spawnPosition = landingPosition.position;
        transform.position = spawnPosition;

        float landingTime = 5f; 
        float elapsedTime = 0f;

        Vector3 initialPosition = transform.position;

        while (elapsedTime < landingTime)
        {
            float lerpValue = Mathf.Clamp01(elapsedTime / (landingTime / landingSpeed));  

            transform.position = Vector3.Lerp(initialPosition, new Vector3(initialPosition.x, 0f, initialPosition.z), lerpValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // lốc xoáy
    private IEnumerator ActivateTornadoSkill()
    {
        yield return new WaitForSeconds(1f);

        currentTornado = Instantiate(tornadoPrefab, tornadoSpawn.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));

        float elapsedTime = 0f;
        float initialPullForce = pullForce;
        float velocity = 0f;

        while (elapsedTime < tornadoDuration)
        {
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
    private IEnumerator SkyfallSkill()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerPosition = player.transform.position; 

            float descentTime = descentHeight / skyfallSpeed;
            float elapsedDescent = 0f;
            Vector3 originalPosition = transform.position;

            // Di chuyển xuống
            while (elapsedDescent < descentTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(originalPosition.x, originalPosition.y - descentHeight, originalPosition.z), skyfallSpeed * Time.deltaTime);
                elapsedDescent += Time.deltaTime;
                yield return null;
            }

            Vector3 ascentTarget = new Vector3(originalPosition.x, originalPosition.y + ascentHeight, originalPosition.z);
            float ascentTime = ascentHeight / fastAscentSpeed;
            float elapsedAscent = 0f;

            // Di chuyển lên
            while (elapsedAscent < ascentTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, ascentTarget, fastAscentSpeed * Time.deltaTime);
                elapsedAscent += Time.deltaTime;
                yield return null;
            }

            playerPosition = player.transform.position;  

            Vector3 spawnPosition = new Vector3(playerPosition.x, playerPosition.y + 35f, playerPosition.z);
            transform.position = spawnPosition; 

            float fallSpeed = 25f;  
            while (Vector3.Distance(transform.position, playerPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerPosition, fallSpeed * Time.deltaTime);
                yield return null;

                if (IsGrounded())
                {
                    Debug.Log("Chạm đất, dừng lại 0,5 giây");
                    yield return new WaitForSeconds(0.5f);
                    break;
                }
            }
            // Bay chéo thẳng
            Vector3 targetPosition = transform.position + new Vector3(50f, 50f, 0f);
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, teleportSpeed * Time.deltaTime);
                yield return null;
            }

            StartCoroutine(LandAtPosition());

        }
        else
        {
            Debug.LogWarning("Player not found! Skyfall skill won't be performed.");
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(layerGround.position, 1f);  

    }

}
