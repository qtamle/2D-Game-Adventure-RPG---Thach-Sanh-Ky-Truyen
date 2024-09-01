using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollision : MonoBehaviour
{
    public Transform handRetract;
    public float speedRetract = 10f;

    private bool isRetracting = false;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hand"))
        {
            Debug.Log($"{gameObject.name} đã va chạm với {other.gameObject.name}.");
            isRetracting = true;
        }
    }

    private void Update()
    {
        if (isRetracting)
        {
            Debug.Log($"Di chuyển từ {transform.position} đến {handRetract.position}");
            transform.position = Vector3.MoveTowards(transform.position, handRetract.position, speedRetract * Time.deltaTime);

            if (Vector3.Distance(transform.position, handRetract.position) <= 0.1f)
            {
                Debug.Log($"{gameObject.name} đã thu về vị trí ban đầu.");
                Destroy(gameObject, 5f);
            }
            Destroy(gameObject, 5f);
        }
    }
}
