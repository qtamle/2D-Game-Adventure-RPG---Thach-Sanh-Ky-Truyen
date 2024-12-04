using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActiveCocTinh : MonoBehaviour
{
    public GameObject active;
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
            SceneManager.LoadScene("Toad 1");
        }
    }
}