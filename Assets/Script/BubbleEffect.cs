using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEffect : MonoBehaviour
{
    public float scaleSpeed = 0.5f;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float speed;

    private Vector3 randomScale;

    void Start()
    {
        randomScale = transform.localScale;
    }

    void Update()
    {
        randomScale.x = Mathf.PingPong(Time.time * scaleSpeed, maxScale - minScale) + minScale;
        randomScale.y = Mathf.PingPong(Time.time * (scaleSpeed + 0.3f), maxScale - minScale) + minScale;
        randomScale.z = Mathf.PingPong(Time.time * (scaleSpeed + 0.6f), maxScale - minScale) + minScale;

        transform.localScale = randomScale;

        transform.Rotate(0, 0, speed * Time.deltaTime);

    }
}
