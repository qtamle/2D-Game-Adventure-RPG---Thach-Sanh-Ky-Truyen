using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Dialogue : MonoBehaviour
{
    public GameObject dialogueCanvas;

    // text content
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;

    // content
    [SerializeField] private string[] speaker;
    [SerializeField] private string[] dialogue;

    private bool dialogueActivated;
    private int step;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) && dialogueActivated == true)
        {
            if (step >= speaker.Length)
            {
                dialogueCanvas.SetActive(false);
                step = 0;
            }
            else
            {
                dialogueCanvas.SetActive(true);
                speakerText.text = speaker[step];
                dialogueText.text = dialogue[step];
                step += 1;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            dialogueActivated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        dialogueActivated = false;
        dialogueCanvas.SetActive(false);
    }
}
