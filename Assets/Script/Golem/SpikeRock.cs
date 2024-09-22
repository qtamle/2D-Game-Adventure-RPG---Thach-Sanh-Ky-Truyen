using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeRock : MonoBehaviour
{
    [Header("Spike Movement")]
    public float riseSpeed = 1f; 
    public float riseDistance = 1f; 
    public float fallSpeed = 1f; 
    public float extraFallDistance = 1f; 
    public float destroyDelay = 3f;

    private PlayerMovement playerMovement;
    public LayerMask groundMask;
    private Vector2 groundPosition;

    private bool isDamaged = false;

    private void Start()
    {
        groundPosition = GetGroundPosition();
        if (groundPosition != Vector2.zero)
        {
            transform.position = groundPosition; 
            StartCoroutine(SpikeRoutine());
        }
        else
        {
            Debug.LogError("Not found ground");
        }
    }

    private Vector2 GetGroundPosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, groundMask);

        if (hit.collider != null)
        {
            // Trả về điểm hit từ Raycast
            return hit.point;
        }
        return Vector2.zero;
    }

    private IEnumerator SpikeRoutine()
    {
        yield return StartCoroutine(MoveSpike(groundPosition, groundPosition + Vector2.up * riseDistance, riseSpeed));

        yield return StartCoroutine(MoveSpike(transform.position, groundPosition - Vector2.up * extraFallDistance, fallSpeed));
        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }

    private IEnumerator MoveSpike(Vector2 from, Vector2 to, float speed)
    {
        float distance = Vector2.Distance(from, to);
        float duration = distance / speed; 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = to;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isDamaged)
        {
            playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(10f, 0.5f, 0.65f, 0.1f);
                isDamaged = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 10f);

        Vector2 ground = GetGroundPosition();
        if (ground != Vector2.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(ground, 0.1f); 

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(ground, ground - Vector2.up * extraFallDistance);
        }
    }
}
