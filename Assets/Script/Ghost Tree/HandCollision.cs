using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollision : MonoBehaviour
{
    public Transform handRetract;
    public float speedRetract = 10f;
    public GameObject collisionParticlePrefab;
    public Transform particleSpawnPoint;
    public float damage = 10f;

    public bool isRetracting = false;
    public bool isAttack = false;

    private Rigidbody2D rb;
    private CameraShake cameraShake;
    public LayerMask playerLayer;
    private void Start()
    {
        cameraShake = GameObject.FindGameObjectWithTag("Shake").GetComponent<CameraShake>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Hand"))
        {
            cameraShake.CamShakeGhostTree();
            Debug.Log($"{gameObject.name} đã va chạm với {other.gameObject.name}.");
            isRetracting = true;
            GameObject shock = Instantiate(collisionParticlePrefab, particleSpawnPoint.position, Quaternion.identity);
            Destroy(shock, 2f);
        }
        if ((playerLayer & (1 << other.gameObject.layer)) != 0)
        {
            if (!isAttack)
            {
                PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
                if (player != null)
                {
                    player.TakeDamage(damage, 0f, 0f, 0f);
                    isAttack = true;
                }
            }
        }
    }

    private void Update()
    {
        if (isRetracting)
        {
            transform.position = Vector3.MoveTowards(transform.position, handRetract.position, speedRetract * Time.deltaTime);

            if (Vector3.Distance(transform.position, handRetract.position) <= 0.1f || isRetracting)
            {
                Destroy(gameObject, 3f);
            }
        }
    }
}
