using UnityEngine;
using System.Collections;

public class SpikeCollision : MonoBehaviour
{
    public float damageAmount = 10f;
    public float destroyDelay = 1f;  
    public bool hasDamaged = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null && !hasDamaged)
            {
                player.TakeDamage(damageAmount, 2f, 0.65f, 0.1f);
            }
            hasDamaged = true;
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
