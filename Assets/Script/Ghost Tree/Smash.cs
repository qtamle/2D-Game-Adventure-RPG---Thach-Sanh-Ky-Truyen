using System.Collections;
using UnityEngine;

public class Smash : MonoBehaviour
{
    public float smashRotationSpeed = 90f;
    public float zRotationChangeRate = 0.5f;

    public PointCheck pointCheck;

    private bool rotateClockwise;
    private float initialZRotation;
    private bool isLeftSide;

    private void Start()
    {
        if (pointCheck == null)
        {
            Debug.LogError("PointCheck component không được gán. Vui lòng gán nó qua Inspector.");
        }

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"), true);

        StartCoroutine(RotateAndCheckForCollision());
    }

    public void SetRotationDirection(bool clockwise, float initialRotation, bool isLeftSide)
    {
        rotateClockwise = clockwise;
        initialZRotation = initialRotation;
        transform.rotation = Quaternion.Euler(0, 0, initialRotation);
        this.isLeftSide = isLeftSide;
    }

    private IEnumerator RotateAndCheckForCollision()
    {
        float zRotation = initialZRotation;

        while (true)
        {
            float rotationChange = smashRotationSpeed * Time.deltaTime;

            if (isLeftSide)
            {
                // Xoay giảm dần về giá trị zRotation tối thiểu
                if (zRotation > -180f) // Đặt giá trị zRotation cho bên trái
                {
                    zRotation -= rotationChange;
                    if (zRotation < -180f)
                    {
                        zRotation = -180f;
                    }
                }
            }
            else
            {
                // Xoay tăng dần về giá trị zRotation tối đa
                if (zRotation < 180f) // Đặt giá trị zRotation cho bên phải
                {
                    zRotation += rotationChange;
                    if (zRotation > 180f)
                    {
                        zRotation = 180f;
                    }
                }
            }

            transform.rotation = Quaternion.Euler(0, 0, zRotation);

            if (isLeftSide && zRotation <= -180f)
            {
                break;
            }
            if (!isLeftSide && zRotation >= 180f)
            {
                break;
            }

            yield return null;
        }

        // Destroy the object after 5 seconds
        Destroy(gameObject, 5f);
    }
}
