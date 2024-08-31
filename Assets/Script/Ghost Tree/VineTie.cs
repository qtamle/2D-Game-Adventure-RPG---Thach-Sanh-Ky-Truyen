using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vine : MonoBehaviour
{
    public float holdDuration = 5f;
    public float missDuration = 1f;

    private bool isHitPlayer = false;

    private void Start()
    {
        StartCoroutine(Drop());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isHitPlayer = true;

            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();

            if (playerMovement != null)
            {
                playerMovement.enabled = false;

                StartCoroutine(ReleasePlayer(playerMovement));
            }
        }
    }

    private IEnumerator ReleasePlayer(PlayerMovement playerMovement)
    {
        yield return new WaitForSeconds(holdDuration);

        playerMovement.enabled = true;
        Destroy(gameObject);
    }

    private IEnumerator Drop()
    {
        yield return new WaitForSeconds(missDuration);

        if (!isHitPlayer)
        {
            Destroy(gameObject);
        }
    }

}
