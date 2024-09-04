using UnityEngine;

public class Bug : MonoBehaviour
{
    public float speed = 3f;
    private Collider2D boundsCollider;

    private Vector2 direction;

    public Collider2D BoundsCollider
    {
        get => boundsCollider;
        set
        {
            boundsCollider = value;
            SetRandomDirection();
        }
    }

    private void Start()
    {
        int bugLayer = LayerMask.NameToLayer("Bug");
        int bubbleLayer = LayerMask.NameToLayer("Bubble");

        Physics2D.IgnoreLayerCollision(bugLayer, bubbleLayer, true);
        Physics2D.IgnoreLayerCollision(bugLayer, bugLayer, true);
    }

    void Update()
    {
        if (boundsCollider == null) return;

        Move();
        CheckBounds();
    }

    private void SetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
    }

    private void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void CheckBounds()
    {
        if (!boundsCollider.OverlapPoint(transform.position))
        {
            SetRandomDirection();
        }
    }
}
