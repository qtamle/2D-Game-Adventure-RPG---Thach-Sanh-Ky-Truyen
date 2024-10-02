using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;
public class CameraShake : MonoBehaviour
{
    public Animator animator;
    public ShakeData spikeShake;

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

    public void EagleShakeFall()
    {
        animator.SetTrigger("fall");
    }

    public void GolemShakeDash()
    {
        animator.SetTrigger("dash");
    }

    public void GolemStompShake()
    {
        animator.SetTrigger("stomp");
    }

    public void GolemSmashShake()
    {
        animator.SetTrigger("smash");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CameraShakerHandler.Shake(spikeShake);
        }
    }
}
