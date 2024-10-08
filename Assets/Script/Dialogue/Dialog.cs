using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog : MonoBehaviour
{
    public string characterName;
    public Sprite characterPortrait;
    [TextArea(3, 10)] public string dialogueText;

    public Dialogue[] dialogues;
}
