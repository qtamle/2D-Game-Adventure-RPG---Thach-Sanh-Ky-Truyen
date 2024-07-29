using System.Collections;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    private BoxCollider2D ropeCollider;
    private Rigidbody2D rb2d;

    private void Start()
    {
        ropeCollider = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void DisableColliderTemporarily(float duration)
    {
        if (ropeCollider != null)
        {
            StartCoroutine(DisableColliderCoroutine(duration));
        }
    }

    private IEnumerator DisableColliderCoroutine(float duration)
    {
        ropeCollider.enabled = false; // Tắt collider
        yield return new WaitForSeconds(duration); // Đợi trong khoảng thời gian
        ropeCollider.enabled = true; // Bật collider trở lại
    }

    public void ResetRope()
    {
        rb2d.angularVelocity = 0f;
        rb2d.velocity = Vector2.zero;
    }
}
