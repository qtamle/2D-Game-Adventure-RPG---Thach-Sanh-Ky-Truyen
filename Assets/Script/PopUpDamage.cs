using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpDamage : MonoBehaviour
{
    private Rigidbody2D rb;
    private TMP_Text damageTxt;

    public float InitialYVelocity = 7f;
    public float InitialXVelocity = 3f;
    public float lifeTime = 0.8f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        damageTxt = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        float randomSize = Random.Range(0.5f, 1.5f);
        damageTxt.fontSize *= randomSize;

        rb.velocity = new Vector2(Random.Range(-InitialXVelocity, InitialYVelocity), InitialYVelocity);
        Destroy(gameObject, lifeTime);
    }

    public void SetMessage(string message)
    {
        damageTxt.SetText(message);
    }
}
