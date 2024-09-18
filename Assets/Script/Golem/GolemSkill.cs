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

    [Header("Jump Attack")]
    public float jumpForce;
    public float stompDelay;
    public float stomDuration;

    [Header("Other")]
    public LayerMask golemLayer;
    public LayerMask playerLayer;
    public GameObject rollingStone;
    public float throwForceStone;
    public Transform throwStoneRoll;

    private Rigidbody2D rb;
    private Transform player;
    private Vector2 lastPlayerPosition;
    private bool isPerformingSkill = false;

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

        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(GolemCombo());
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
        isPerformingSkill = true;
    }

    void StopDash()
    {
        isDashing = false;
        rb.velocity = Vector2.zero;
        isPerformingSkill = false;
    }

    // ném đá
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

    // gai đá
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

    //combo
    IEnumerator DashForwad()
    {
        isDashing = true;
        dashTimeCounter = dashDuration;
        isPerformingSkill = true;

        while (dashTimeCounter > 0)
        {
            rb.velocity = new Vector2((facingRight ? 1 : -1) * dashSpeed, rb.velocity.y);
            dashTimeCounter -= Time.deltaTime;
            yield return null;
        }

        StopDash();
    }

    IEnumerator JumpAndStomp()
    {
        yield return new WaitForSeconds(stompDelay);

        lastPlayerPosition = player.position;
        Vector2 directionToPlayer = (lastPlayerPosition - (Vector2)transform.position).normalized;
        rb.velocity = new Vector2(directionToPlayer.x * Mathf.Abs(jumpForce), jumpForce);

        yield return new WaitUntil(() => Mathf.Abs(transform.position.x - lastPlayerPosition.x) < 0.5f);
        rb.velocity = new Vector2(0, -Mathf.Abs(jumpForce * 5f));
        yield return new WaitUntil(() => rb.velocity.y == 0);

        rb.velocity = Vector2.zero;
    }

    IEnumerator ThrowRollingStone()
    {
        yield return new WaitForSeconds(1f);

        GameObject stone = Instantiate(rollingStone, throwStoneRoll.position, Quaternion.identity);

        Vector2 throwDirection = (player.position - transform.position).normalized;

        if (!facingRight)
        {
            throwDirection.x = -Mathf.Abs(throwDirection.x);
        }

        Rigidbody2D stoneRb = stone.GetComponent<Rigidbody2D>();
        stoneRb.gravityScale = 1;
        stoneRb.AddForce(new Vector2(throwDirection.x * throwForceStone, 0), ForceMode2D.Impulse);

        Collider2D stoneCollider = stone.GetComponent<Collider2D>();
        if (stoneCollider != null)
        {
            PhysicsMaterial2D rollingMaterial = new PhysicsMaterial2D
            {
                friction = 0.4f,
                bounciness = 0f
            };
            stoneCollider.sharedMaterial = rollingMaterial;
        }
    }

    IEnumerator GolemCombo()
    {
        yield return DashForwad();

        yield return new WaitForSeconds(1f);

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }

        yield return JumpAndStomp();

        yield return new WaitForSeconds(1f);

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }

        yield return ThrowRollingStone();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("TurnOn") && isPerformingSkill)
        {
            StopDash();
            rb.velocity = Vector2.zero;
            StartCoroutine(StandStillAndResumeSkill());
        }
        else if (collision.gameObject.CompareTag("TurnOn"))
        {
            StopDash();
            Flip();
        }
    }

    IEnumerator StandStillAndResumeSkill()
    {
        yield return new WaitForSeconds(3f);

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }

        if (isDashing)
        {
            StartDash(); 
        }
    }
}