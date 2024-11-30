using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float runSpeed = 3f;

    float horizontal;
    float vertical;
    bool facingRight;

    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(horizontal * runSpeed, vertical * runSpeed, 0.0f);

        // Tính toán vị trí mới
        Vector3 newPosition = transform.position + movement * Time.deltaTime;

        // Giới hạn di chuyển theo trục X và Y
        newPosition.x = Mathf.Clamp(newPosition.x, -20.13764f, 20.20948f);
        newPosition.y = Mathf.Clamp(newPosition.y, -5.5f, -2.21f);

        // Cập nhật vị trí mới
        transform.position = newPosition;

        // Gọi hàm Flip nếu cần
        Flip(horizontal);
    }

    private void Flip(float horizontal)
    {
        if (horizontal < 0 && !facingRight || horizontal > 0 && facingRight)
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
