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
    private float dashTimeCounter;
    public float dashDurationCombo;

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

    [Header("Falling Stone")]
    public Transform[] fallingStone;
    public GameObject fallStone;

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
    private bool isSkillActived = false;
    private bool isStandingStill = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);

        StartCoroutine(RandomSkillRoutine());
    }
    IEnumerator RandomSkillRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            if (!isSkillActived && !isStandingStill)
            {
                int randomSkill = Random.Range(0, 100); 

                if (randomSkill < 35) // 35% cho DashCoroutine
                {
                    Debug.Log("tong bth");
                    yield return StartCoroutine(DashCoroutine());
                }
                else if (randomSkill < 60) // 25% cho GolemCombo (35% + 25%)
                {
                    Debug.Log("combo");
                    yield return StartCoroutine(GolemCombo());
                }
                else if (randomSkill < 80) // 20% cho SpawnSpikes (60% + 20%)
                {
                    Debug.Log("spike");
                    yield return StartCoroutine(SpawnSpikes());
                }
                else if (randomSkill < 90) // 10% cho JumpAndStomp (80% + 10%)
                {
                    Debug.Log("nhay dam");
                    yield return StartCoroutine(JumpAndStomp());
                }
                else // 10% cho ThrowRollingStone (90% + 10%)
                {
                    Debug.Log("nem da lan");
                    yield return StartCoroutine(ThrowRollingStone());
                }

                yield return new WaitForSeconds(3f);
            }
            yield return null;
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
        StartCoroutine(DashCoroutine());
    }

    IEnumerator DashCoroutine()
    {
        isSkillActived = true;
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
        isSkillActived = false; 
    }

    void StopDash()
    {
        isDashing = false;
        rb.velocity = Vector2.zero;
        isPerformingSkill = false;
        isSkillActived = true;
    }

    /*
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

            Destroy(rock, 5f);
        }

    }*/

    // gai đá
    IEnumerator SpawnSpikes()
    {
        isSkillActived = true;

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }

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

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }
        isSkillActived = false;
    }

    //combo
    IEnumerator DashForwad()
    {
        isSkillActived = true;
        isDashing = true;
        dashTimeCounter = dashDurationCombo;

        while (dashTimeCounter > 0)
        {
            rb.velocity = new Vector2((facingRight ? 1 : -1) * dashSpeed, rb.velocity.y);
            dashTimeCounter -= Time.deltaTime;
            yield return null;
        }

        StopDash();
        isSkillActived = false;
    }

    IEnumerator JumpAndStomp()
    {
        isSkillActived = true;

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }

        yield return new WaitForSeconds(stompDelay);

        lastPlayerPosition = player.position;
        Vector2 directionToPlayer = (lastPlayerPosition - (Vector2)transform.position).normalized;
        rb.velocity = new Vector2(directionToPlayer.x * Mathf.Abs(jumpForce), jumpForce);

        yield return new WaitUntil(() => Mathf.Abs(transform.position.x - lastPlayerPosition.x) < 0.5f);
        rb.velocity = new Vector2(0, -Mathf.Abs(jumpForce * 5f));
        yield return new WaitUntil(() => rb.velocity.y == 0);

        rb.velocity = Vector2.zero;
        isSkillActived = false;
    }

    IEnumerator ThrowRollingStone()
    {
        isSkillActived = true;
        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }
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

        Destroy(stone, 3f);
        isSkillActived = false;
    }

    IEnumerator GolemCombo()
    {
        isSkillActived = true;
        yield return DashForwad();

        yield return new WaitForSeconds(1f);

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }

        yield return JumpAndStomp();

        yield return new WaitForSeconds(0.5f);

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }

        yield return ThrowRollingStone();

        isSkillActived = false;

    }

    // rơi đá
    private void SpawnFallingStones()
    {
        for (int i = 0; i < fallingStone.Length; i++)
        {
            Vector2 stoneSpawnPosition = fallingStone[i].position;

            GameObject rock = Instantiate(fallStone, stoneSpawnPosition, Quaternion.identity);

            Rigidbody2D rockRb = rock.GetComponent<Rigidbody2D>();
            rockRb.gravityScale = 1;

            Destroy(rock, 5f);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("TurnOn") && isPerformingSkill)
        {
            StopDash();
            SpawnFallingStones();
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
        isStandingStill = true;
        yield return new WaitForSeconds(8f);
        isStandingStill = false;

        if ((facingRight && player.position.x < transform.position.x) || (!facingRight && player.position.x > transform.position.x))
        {
            Flip();
        }
    }
}