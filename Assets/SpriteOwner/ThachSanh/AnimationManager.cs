using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public AnyStateAnimator animator;

    private void Start()
    {
        AnyStateAnimation[] animations = new AnyStateAnimation[]
        {
            new AnyStateAnimation(thachsanh.BODY, "Body_Attack0"),
            new AnyStateAnimation(thachsanh.BODY, "Body_Attack1"),
            new AnyStateAnimation(thachsanh.BODY, "Body_Attack2"),
            new AnyStateAnimation(thachsanh.BODY, "Legs_Attack0"),
            new AnyStateAnimation(thachsanh.BODY, "Legs_Attack1"),
            new AnyStateAnimation(thachsanh.BODY, "Legs_Attack2"),
            new AnyStateAnimation(thachsanh.BODY, "Body_Idle"),
            new AnyStateAnimation(thachsanh.BODY, "Body_Run"),
            new AnyStateAnimation(thachsanh.LEGS, "Legs_Idle"),
            new AnyStateAnimation(thachsanh.LEGS, "Legs_Run")
        };

        animator = GetComponent<AnyStateAnimator>();
        animator.AddAnimations(animations);
    }
}
