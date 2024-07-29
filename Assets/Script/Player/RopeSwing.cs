using UnityEngine;

public class RopeSwing : MonoBehaviour
{
    public float swingForce = 10f; // Lực để đu dây
    private bool isSwinging = false;
    private Rigidbody2D rb;
    private HingeJoint2D RopehingeJoint;
    private GameObject rope;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isSwinging)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            rb.AddForce(new Vector2(horizontalInput * swingForce, 0));

            if (Input.GetKeyDown(KeyCode.Space))
            {
                DetachFromRope();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope") && !isSwinging)
        {
            AttachToRope(collision.gameObject);
        }
    }

    void AttachToRope(GameObject ropeObject)
    {
        isSwinging = true;
        rb.velocity = Vector2.zero; // Dừng chuyển động
        rb.gravityScale = 0; // Tắt trọng lực

        // Tạo HingeJoint để gắn vào dây
        RopehingeJoint = gameObject.AddComponent<HingeJoint2D>();
        RopehingeJoint.connectedBody = ropeObject.GetComponent<Rigidbody2D>();
        RopehingeJoint.autoConfigureConnectedAnchor = false;
        RopehingeJoint.connectedAnchor = transform.position - ropeObject.transform.position;

        // Lưu lại tham chiếu đến đối tượng dây
        rope = ropeObject;
    }

    void DetachFromRope()
    {
        isSwinging = false;
        rb.gravityScale = 1; // Khôi phục trọng lực
        Destroy(RopehingeJoint); // Loại bỏ HingeJoint
        RopehingeJoint = null;
        rope = null;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
        {
            DetachFromRope();
        }
    }
}
