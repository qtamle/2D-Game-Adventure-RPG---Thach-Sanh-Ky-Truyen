using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackEnemy : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 10f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component missing from this game object.");
        }
    }

    public void ApplyKnockback(Vector2 direction)
    {
        if (rb != null)
        {
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
