using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeam : MonoBehaviour
{
    public GameObject lightBeamPrefab;
    public float beamSpeed = 10f;
    public Vector2 beamDirection = new Vector2(1, 0);

    void Start()
    {
        GameObject lightBeam = Instantiate(lightBeamPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = lightBeam.GetComponent<Rigidbody2D>();
        rb.velocity = beamDirection.normalized * beamSpeed;
        Destroy(gameObject);
    }
}
