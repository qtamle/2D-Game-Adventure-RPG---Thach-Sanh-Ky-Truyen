using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    [SerializeField] private Transform bow;
    [SerializeField] private float bowDistance;

    [Header("Bullet")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed;

    [Header("Player")]
    [SerializeField] private Transform player;

    private bool bowFacingRight = false;
    public bool isAiming = false;

    private PlayerMovement playerMovement;
    private bool playerFacingRight = false;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
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

        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
            // Disable PlayerMovement script
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
                Debug.Log("PlayerMovement disabled");
            }
            Debug.Log("Aiming started");
        }
        if (Input.GetMouseButtonUp(1))
        {
            if (isAiming)
            {
                isAiming = false;
                Shoot(direction);
                // Enable PlayerMovement script
                if (playerMovement != null)
                {
                    playerMovement.enabled = true;
                    Debug.Log("PlayerMovement enabled");
                }
                Debug.Log("Aiming ended and shot fired");
            }
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
