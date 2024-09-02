using System.Collections;
using UnityEngine;

public class SmallSpike : MonoBehaviour
{
    public float damage = 5f;
    private bool hasHitPlayer = false;

    private void Start()
    {
        StartCoroutine(DestroyAfterTime(3f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(damage, 0.5f, 0.65f, 0.1f);
                hasHitPlayer = true; 
                StopCoroutine(DestroyAfterTime(3f)); 
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        if (!hasHitPlayer)
        {
            Destroy(gameObject);
        }
    }
}
