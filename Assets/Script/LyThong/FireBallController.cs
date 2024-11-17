using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    public float growSpeed = 1f;
    public float maxScale;

    private Vector3 initialScale;

    private void Start()
    {
        initialScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (transform.localScale.x < maxScale)
        {
            transform.localScale += Vector3.one * growSpeed * Time.deltaTime;
        }
    }
}
