using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleSkill : MonoBehaviour
{
    [Header("Wind-cut")]
    public GameObject windCut;
    public float speedWind;
    public Transform spawnPoint;
    private Transform player;

    [Header("Movement")]
    public Transform playerToMove; 
    public float flySpeed = 5f; 
    public float diveSpeed = 10f; 
    public float offScreenDistance = 5f; 
    public float overshootDistance = 2f;
    private Vector2[] corners;
    private Vector2 selectedCorner;
    private Vector2 currentDivePosition;
    private Vector2 startFallPosition;
    public Transform fallTarget;

    [Header("Tornado Skill")]
    public GameObject tornadoPrefab;
    public float tornadoDuration = 3f;
    public float pullForce = 5f;
    private GameObject currentTornado;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        corners = new Vector2[4];
        corners[0] = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)) + Vector3.left * offScreenDistance; // Góc trên trái
        corners[1] = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0)) + Vector3.right * offScreenDistance; // Góc trên phải
        corners[2] = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)) + Vector3.left * offScreenDistance; // Góc dưới trái
        corners[3] = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)) + Vector3.right * offScreenDistance; // Góc dưới phải

        SelectRandomCorner();
        currentDivePosition = selectedCorner;
        startFallPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.2f, 0)); 

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(ShootWindCut());
        }

        if (Input.GetKeyDown(KeyCode.I)) 
        {
            StartCoroutine(ActivateTornadoSkill());
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(EagleDiveAttack(4));
        }
    }

    // bắn vết cắt gió
    public IEnumerator ShootWindCut()
    {
        ShootSingleWindCut(new Vector2(-1, -1));
        yield return new WaitForSeconds(1f);

        ShootSingleWindCut(new Vector2(1, -1));
        yield return new WaitForSeconds(1f);

        ShootDualWindCut();
    }

    private void ShootSingleWindCut(Vector2 direction)
    {
        direction = direction.normalized;

        GameObject windcut = Instantiate(windCut, spawnPoint.position, Quaternion.identity);
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        windcut.transform.rotation = Quaternion.Euler(0,0, angle - 90);

        Rigidbody2D rb = windcut.GetComponent<Rigidbody2D>();
        rb.velocity = direction * speedWind;

        Destroy(windcut, 2f);
    }

    private void ShootDualWindCut()
    {
        ShootSingleWindCut(new Vector2(1,-1));
        ShootSingleWindCut(new Vector2(-1,-1));
    }

    // đại bàng lao tới
    private void SelectRandomCorner()
    {
        selectedCorner = corners[Random.Range(0, corners.Length)];
    }

    IEnumerator EagleDiveAttack(int maxDives)
    {
        int diveCount = 0;

        while (diveCount < maxDives)
        {
            while (Vector2.Distance(transform.position, currentDivePosition) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, currentDivePosition, flySpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            // Lưu vị trí cuối cùng của player
            Vector2 playerPosition = player.position;

            // Tính toán điểm vượt qua vị trí player
            Vector2 directionToPlayer = (playerPosition - (Vector2)transform.position).normalized; 
            Vector2 overshootPosition = playerPosition + directionToPlayer * overshootDistance; 

            float diveTime = 0f;
            while (diveTime < 4f && Vector2.Distance(transform.position, overshootPosition) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, overshootPosition, diveSpeed * Time.deltaTime);
                diveTime += Time.deltaTime;
                yield return null;
            }

            transform.position = overshootPosition;
            yield return new WaitForSeconds(2f);
            currentDivePosition = overshootPosition;

            diveCount++;
        }
        float fallDuration = 3f; 
        float elapsedTime = 0f;
        Vector2 startFallPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.2f, 0)); 

        while (elapsedTime < fallDuration)
        {
            float t = elapsedTime / fallDuration;
            transform.position = Vector2.Lerp(startFallPosition, fallTarget.position, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = fallTarget.position;
    }

    // lốc xoáy
    private IEnumerator ActivateTornadoSkill()
    {
        Vector2 screenCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        currentTornado = Instantiate(tornadoPrefab, screenCenter, Quaternion.identity);

        float elapsedTime = 0f;
        while (elapsedTime < tornadoDuration)
        {
            elapsedTime += Time.deltaTime;

            if (player != null)
            {
                Vector2 playerPosition = player.position;
                Vector2 tornadoPosition = currentTornado.transform.position;
                Vector2 directionToTornado = (tornadoPosition - playerPosition).normalized;

                player.position = new Vector2(
                    Mathf.MoveTowards(player.position.x, tornadoPosition.x, pullForce * Time.deltaTime),
                    player.position.y
                );
            }

            yield return null;
        }

        Destroy(currentTornado);
    }
}
