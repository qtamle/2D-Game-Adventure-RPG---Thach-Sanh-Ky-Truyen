using UnityEngine;
using System.Collections;

public class SpikeCollision : MonoBehaviour
{
    public float damageAmount = 10f;
    public float destroyDelay = 1f;  

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(damageAmount, 0.2f, 1f, 0.1f);
            }
        }
        else
        {
            StartCoroutine(DestroyAfterDelay(destroyDelay));
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
