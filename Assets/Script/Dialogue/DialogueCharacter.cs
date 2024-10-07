using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Dialogue/Character")]
public class DialogueCharacter : ScriptableObject
{
    public string characterName;
    public Sprite[] portraits;
    [Range(0f, 3f)] public float pitch = 1f;
}
