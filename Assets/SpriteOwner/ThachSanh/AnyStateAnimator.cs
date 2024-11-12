using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnyStateAnimator : MonoBehaviour
{
   public Animator animator;

    private Dictionary<string, AnyStateAnimation> animations = new Dictionary<string, AnyStateAnimation>();

    private string currentAnimationBody = string.Empty;
    private string currentAnimationLegs = string.Empty;

    private void Awake()
    {
        animator = GetComponent<Animator>();  
    }

    public void AddAnimations(params AnyStateAnimation[] newAnimations)
    {
        for (int i = 0; i < newAnimations.Length; i++)
        {
            if (!string.IsNullOrEmpty(newAnimations[i].Name))
            {
                animations.Add(newAnimations[i].Name, newAnimations[i]);
            }
            else
            {
                Debug.LogWarning("Animation Name is null or empty and was not added.");
            }
        }
    }

    public void TryPlayAnimation(string newAnimation)
    {
        switch (animations[newAnimation].AnimationTS)
        {
            case thachsanh.BODY:
                PlayAnimation(ref currentAnimationBody);
                break;
            case thachsanh.LEGS:
                PlayAnimation(ref currentAnimationLegs);
                break;
        }
        
        void PlayAnimation(ref string currentAnimation)
        {
            if (currentAnimation == "")
            {
                animations[newAnimation].Active = true;
                currentAnimation = newAnimation;
            }
            else if (currentAnimation != newAnimation)
            {
                animations[currentAnimation].Active = false;
                animations[newAnimation].Active = true;

                currentAnimation = newAnimation;
            }
        }
        Animate();
    }
    public void ResetAnimations(bool isRunning)
    {
        // Reset BODY and LEGS to Idle or Run depending on movement state
        string bodyAnimation = isRunning ? "Body_Run" : "Body_Idle";
        string legsAnimation = isRunning ? "Legs_Run" : "Legs_Idle";

        TryPlayAnimation(bodyAnimation);
        TryPlayAnimation(legsAnimation);
    }

    public void SetTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);  // Sử dụng SetTrigger cho trigger name
    }

    public void ResetTrigger(string triggerName)
    {
        animator.ResetTrigger(triggerName);  // Sử dụng ResetTrigger cho trigger name
    }

    public void SetInteger(string parameterName, int value)
    {
        animator.SetInteger(parameterName, value);
    }
    private void Animate()
    {
        foreach(string key in animations.Keys) 
        {
            animator.SetBool(key, animations[key].Active);
        }
    }


}

