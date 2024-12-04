using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActiveCocTinh : MonoBehaviour
{
    public GameObject active;
    private bool isActive;

    private void Start()
    {
        active.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isActive)
            {
                SceneManager.LoadScene("Toad 1");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        active.SetActive(true);
        if (collision.gameObject.CompareTag("Player"))
        {
            isActive = true;
            active.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        active.SetActive(false);
        isActive = false;
    }
}
