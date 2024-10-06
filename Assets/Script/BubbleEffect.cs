using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEffect : MonoBehaviour
{
    public float rotationSpeed = 30f;


    void Update()
    {

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
