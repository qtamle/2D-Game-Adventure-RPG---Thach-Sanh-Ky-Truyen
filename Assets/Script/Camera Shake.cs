using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Animator animator;

    public void CamShake()
    {
        animator.SetTrigger("shake");
    }

    public void CamShake1()
    {
        animator.SetTrigger("shake1");
    }
    public void CamShakeGhostTree()
    {
        animator.SetTrigger("shakeGhost");
    }

    public void AttackShake()
    {
        animator.SetTrigger("attackShake");
    }

    public void ToadJumpShake()
    {
        animator.SetTrigger("toadJump");
    }
    
    public void EagleShake()
    {
        animator.SetTrigger("EagleShake");
    }
}
