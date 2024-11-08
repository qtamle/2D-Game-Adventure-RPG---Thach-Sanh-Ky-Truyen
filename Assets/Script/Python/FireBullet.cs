using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : MonoBehaviour
{
    public float damage = 10f;
    public GameObject explosionPrefab;
    public LayerMask groundLayer;
    public float yOffset = 3f;
    public float rotationSpeed;

    private void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(damage, 0.5f, 0.65f, 0.1f);
            }
            Destroy(gameObject);
        }

        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Vector3 hitPosition = collision.ClosestPoint(transform.position);
            hitPosition.y += yOffset;
            GameObject explosion = Instantiate(explosionPrefab, hitPosition, Quaternion.identity);
            Destroy(explosion, 1f);
            Destroy(gameObject);
        }
    }

}
