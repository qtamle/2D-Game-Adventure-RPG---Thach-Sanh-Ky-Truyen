using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Dialogue : MonoBehaviour
{
    public GameObject MainDialogueCanvas;
    public GameObject SecondDialogueCanvas;

    // text content for main canvas
    [SerializeField] private TMP_Text MainspeakerText;
    [SerializeField] private TMP_Text MaindialogueText;

    // text content for second canvas
    [SerializeField] private TMP_Text SecondspeakerText;
    [SerializeField] private TMP_Text SeconddialogueText;

    // content
    [SerializeField] private string[] speaker;
    [SerializeField] private string[] dialogue;
    [SerializeField] private bool[] isMainSpeaker;  // Mảng mới để đánh dấu speaker thuộc canvas nào

    private bool dialogueActivated;
    private int step;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) && dialogueActivated == true)
        {
            Debug.Log("Alt");

            if (step >= speaker.Length)
            {

                // Tắt cả hai canvas sau khi đoạn hội thoại kết thúc
                MainDialogueCanvas.SetActive(false);
                SecondDialogueCanvas.SetActive(false);
                step = 0;
            }
            else
            {
                // Kiểm tra xem người nói hiện tại thuộc canvas nào
                if (isMainSpeaker[step])
                {
                    MainDialogueCanvas.SetActive(true);
                    SecondDialogueCanvas.SetActive(false);

                    // Cập nhật nội dung trên Main canvas
                    MainspeakerText.text = speaker[step];
                    MaindialogueText.text = dialogue[step];
                }
                else
                {
                    MainDialogueCanvas.SetActive(false);
                    SecondDialogueCanvas.SetActive(true);

                    // Cập nhật nội dung trên Second canvas
                    SecondspeakerText.text = speaker[step];
                    SeconddialogueText.text = dialogue[step];
                }

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
        MainDialogueCanvas.SetActive(false);
        SecondDialogueCanvas.SetActive(false);
        step = 0;
    }
}
