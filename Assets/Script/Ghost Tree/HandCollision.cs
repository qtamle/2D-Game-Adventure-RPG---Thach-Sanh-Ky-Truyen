using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

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
    //private CameraShake cameraShake;
    public LayerMask playerLayer;
    public ShakeData handShake;
    private void Start()
    {
        Vector3 startPosition = transform.position;
        startPosition.z = 10f;
        transform.position = startPosition;

       // cameraShake = GameObject.FindGameObjectWithTag("Shake").GetComponent<CameraShake>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Hand"))
        {
            AudioManager.Instance.PlaySFX(0);

            CameraShakerHandler.Shake(handShake);
            Debug.Log($"{gameObject.name} đã va chạm với {other.gameObject.name}.");
            isRetracting = true;

            Vector3 particlePosition = particleSpawnPoint.position;
            particlePosition.z = 10f;

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

        Destroy(gameObject, 2f);
    }
    private void Update()
    {
        if (isRetracting)
        {
            Vector3 targetPosition = handRetract.position;
            targetPosition.z = 10f;

            Vector3 currentPosition = transform.position;
            currentPosition.z = 10f; 
            transform.position = Vector3.MoveTowards(currentPosition, targetPosition, speedRetract * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
            {
                Destroy(gameObject, 2f);
            }
        }
    }
}
