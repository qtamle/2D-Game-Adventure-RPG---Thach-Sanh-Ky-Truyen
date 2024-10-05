using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEffect : MonoBehaviour
{
    public float maxScale = 1.2f; // Kích thước lớn nhất của bong bóng
    public float minScale = 0.8f; // Kích thước nhỏ nhất của bong bóng
    public float scaleSpeed = 1f; // Tốc độ phình to và hóp lại
    public float floatSpeed = 0.1f; // Tốc độ nổi lên

    private Vector3 targetScale;

    void Start()
    {
        // Đặt kích thước ban đầu
        targetScale = transform.localScale;
    }

    void Update()
    {
        // Điều chỉnh kích thước bong bóng
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);

        // Nổi lên
        transform.position += new Vector3(0, floatSpeed * Time.deltaTime, 0);

        // Thay đổi kích thước bong bóng
        if (transform.localScale.x >= maxScale)
        {
            targetScale = new Vector3(minScale, minScale, 1);
        }
        else if (transform.localScale.x <= minScale)
        {
            targetScale = new Vector3(maxScale, maxScale, 1);
        }
    }
}
