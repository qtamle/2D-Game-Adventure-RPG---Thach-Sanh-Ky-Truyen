using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    [SerializeField] private Transform bow;
    [SerializeField] private float bowDistance;
    [SerializeField] private float staminaBow;

    [Header("Bullet")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed;

    [Header("Player")]
    [SerializeField] private Transform player;

    private bool bowFacingRight = false;
    public bool isAiming = false;

    private PlayerMovement playerMovement;
    private Stamina stamina;
    private bool playerFacingRight = false;
    private bool isDrawing = false;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        stamina = GetComponent<Stamina>();
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (isAiming)
        {
            bow.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            bow.position = transform.position + Quaternion.Euler(0, 0, angle) * new Vector3(bowDistance, 0, 0);

            if (player != null)
            {
                if (mousePos.x < player.position.x && playerFacingRight)
                {
                    FlipPlayer();
                }
                else if (mousePos.x > player.position.x && !playerFacingRight)
                {
                    FlipPlayer();
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && stamina.CurrentStamina > staminaBow)
        {
            if (!isDrawing)
            {
                stamina.DecreaseStamina(staminaBow);
                StartCoroutine(DrawBow(direction));
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            if (isDrawing)
            {
                CancelDraw();
            }
            else if (isAiming)
            {
                StartCoroutine(ShootWithDelay(direction));
            }
        }
    }

    private IEnumerator DrawBow(Vector3 direction)
    {
        isDrawing = true;
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            Debug.Log("PlayerMovement disabled");
        }
        Debug.Log("Gồng cung...");

        float drawTime = 1f;  
        float elapsedTime = 0f;

        while (elapsedTime < drawTime)
        {
            if (Input.GetMouseButtonUp(1))
            {
                CancelDraw();
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isAiming = true;
        isDrawing = false;
        Debug.Log("Vào tư thế bắn");
    }

    private void CancelDraw()
    {
        isDrawing = false;
        isAiming = false;

        // Re-enable PlayerMovement if it was disabled
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            Debug.Log("PlayerMovement enabled");
        }

        Debug.Log("Hủy gồng cung, không bắn mũi tên");
    }

    private IEnumerator ShootWithDelay(Vector3 direction)
    {
        isAiming = false;
        Debug.Log("Đang bắn...");
        yield return new WaitForSeconds(0f);

        Shoot(direction);
        Debug.Log("Đã bắn xong!");

        // Enable PlayerMovement script
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            Debug.Log("PlayerMovement enabled");
        }
    }

    private void FlipPlayer()
    {
        playerFacingRight = !playerFacingRight;
        player.localScale = new Vector3(player.localScale.x * -1, player.localScale.y, player.localScale.z);
    }

    public void Shoot(Vector3 direction)
    {
        GameObject newArrow = Instantiate(arrowPrefab, bow.position, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
        newArrow.GetComponent<Rigidbody2D>().velocity = direction.normalized * arrowSpeed;
        Destroy(newArrow, 5f);
    }

    public bool GetPlayerFacingRight()
    {
        return playerFacingRight;
    }
}
