using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCT : MonoBehaviour
{
    public GameObject active;
    public Active activeTimeline;
    private void Start()
    {
        active.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        active.SetActive(true);
        if (collision.gameObject.CompareTag("Player"))
        {
            active.SetActive(false);
            activeTimeline.SetAndPlayTimeline(3);
        }
    }
}
