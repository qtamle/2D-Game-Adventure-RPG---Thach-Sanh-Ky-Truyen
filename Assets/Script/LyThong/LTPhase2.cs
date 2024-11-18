using System.Collections;
using UnityEngine;

public class LTPhase2 : MonoBehaviour
{
    [Header("Summon Lightning")]
    public GameObject explosionPrefab; 
    public GameObject lightningPrefab;
    public Transform player;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundCheckRadius = 0.5f;
    private bool isOnGround;

    private void Update()
    {
        isOnGround = CheckGround();

        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(SummonLightning());
        }
    }

    // summon lightning
    private IEnumerator SummonLightning()
    {
        int summonCount = Random.Range(5, 7);

        for (int i = 0; i < summonCount; i++)
        {
            Vector3 playerPosition = player.position;

            Vector3 explosionPosition = new Vector3(playerPosition.x, 18.73f, playerPosition.z);
            GameObject explosion = Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);

            Destroy(explosion, 1f);

            yield return new WaitForSeconds(0.5f);

            Vector3 lightningPosition = new Vector3(playerPosition.x, 23f, playerPosition.z);
            GameObject lightning = Instantiate(lightningPrefab, lightningPosition, Quaternion.Euler(90f,0f,0f));

            Destroy(lightning, 1f);

            yield return new WaitForSeconds(1f);
        }
    }


    private bool CheckGround()
    {
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

        return hit != null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
