using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustController : MonoBehaviour
{
    [SerializeField] ParticleSystem movementParticle;

    [Range(0, 10)]
    [SerializeField] int occurAfterVelocity;

    [Range(0, 0.2f)]
    [SerializeField] float dustFormationPreiod;

    [SerializeField] Rigidbody2D rb;

    float counter;

    private void Update()
    {
        counter += Time.deltaTime;

        if (Mathf.Abs(rb.velocity.x) > occurAfterVelocity)
        {
            if (counter > dustFormationPreiod)
            {
                movementParticle.Play();
                counter = 0;
            }
        }
    }
}
