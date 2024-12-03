using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActiveCocTinh : MonoBehaviour
{
    public GameObject active;
    public Active activeTimeline;
    private void Start()
    {
        active.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        active.SetActive(true);
        if (collision.gameObject.CompareTag("Player"))
        {
            active.SetActive(false);
            activeTimeline.SetAndPlayTimeline(2);
            //SceneManager.LoadScene("Toad 1");
        }
    }
}
