using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemSkill : MonoBehaviour
{
    [Header("Dash")]
    public float dashSpeed;
    public float dashDuration;
    private bool isDashing = false;
    private bool facingRight = true;
    private float dashTimeCounter; // thời gian giảm dần khi dash

    [Header("Throw Stone")]
    public GameObject rockPrefab;
    public float throwForce;
    public float throwDelay;

    [Header("Spikes")]
    public GameObject spikePrefab;
    public float spikeDelay;
    public float spikeDistance;
    public int maxSpikes = 5;
    public Transform spikeStartPoint;

    [Header("Other")]
    public LayerMask golemLayer;
    public LayerMask playerLayer;


    private Rigidbody2D rb;
    private Transform player;
    private Vector2 lastPlayerPosition;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartDash();
        }

        if (isDashing)
        {
            rb.velocity = new Vector2((facingRight ? 1 : -1) * dashSpeed, rb.velocity.y);

            dashTimeCounter -= Time.deltaTime;
            if (dashTimeCounter <= 0) 
            {
                StopDash();
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(ThrowStone());
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            StartCoroutine(SpawnSpikes());
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // tông về phía trước
    void StartDash()
    {
        isDashing = true;
        dashTimeCounter = dashDuration;
    }

    void StopDash()
    {
        isDashing = false;
        rb.velocity = Vector2.zero;
    }

    IEnumerator ThrowStone()
    {
        int numThrows = Random.Range(3, 6);

        for (int i = 0; i < numThrows; i++)
        {
            lastPlayerPosition = player.position;

            yield return new WaitForSeconds(2f);

            GameObject rock = Instantiate(rockPrefab, transform.position, Quaternion.identity);

            Vector2 throwDirection = (lastPlayerPosition - (Vector2)transform.position).normalized;

            if (!facingRight)
            {
                throwDirection.x = -Mathf.Abs(throwDirection.x);
            }

            Rigidbody2D rockRb = rock.GetComponent<Rigidbody2D>();

            rockRb.velocity = new Vector2(throwDirection.x * throwForce, 0);

            yield return new WaitForSeconds(throwDelay);
        }
    }
    IEnumerator SpawnSpikes()
    {
        yield return new WaitForSeconds(1f);

        if (spikeStartPoint == null)
        {
            Debug.LogError("SpikeStartPoint transform not assigned!");
            yield break;
        }

        float startingPosition;
        if (facingRight)
        {
            startingPosition = spikeStartPoint.position.x - spikeDistance * (maxSpikes - 1) / 2;
        }
        else
        {
            startingPosition = spikeStartPoint.position.x + spikeDistance * (maxSpikes - 1) / 2;
        }

        for (int i = 0; i < maxSpikes; i++)
        {
            Vector2 spawnPosition;
            if (facingRight)
            {
                spawnPosition = new Vector2(startingPosition + i * spikeDistance, spikeStartPoint.position.y);
            }
            else
            {
                spawnPosition = new Vector2(startingPosition - i * spikeDistance, spikeStartPoint.position.y);
            }

            Instantiate(spikePrefab, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(spikeDelay);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("TurnOn"))
        {
            StopDash();
            Flip();
        }
    }

}
