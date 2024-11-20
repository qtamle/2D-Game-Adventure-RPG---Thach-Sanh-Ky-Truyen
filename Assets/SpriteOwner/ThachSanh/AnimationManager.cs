using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public AnyStateAnimator animator;

    private void Start()
    {

        string tag = gameObject.tag;

        AnyStateAnimation[] animations;
        if(tag  == "Player")
        {
            animations = new AnyStateAnimation[]
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
        }
        else if (tag == "Snake")
        {
            animations = new AnyStateAnimation[]
            {
                new AnyStateAnimation(thachsanh.BODY, "Snake_Idle"),
                new AnyStateAnimation(thachsanh.BODY, "Snake_Dash"),
                new AnyStateAnimation(thachsanh.BODY, "Snake_Tail"),
                new AnyStateAnimation(thachsanh.BODY, "Snake_FireSteam"),
                new AnyStateAnimation(thachsanh.BODY, "Snake_FirePillar"),
                new AnyStateAnimation(thachsanh.BODY, "Snake_Projectile"),
                new AnyStateAnimation(thachsanh.BODY, "Snake_Die"),
            };
        }
        else
        {
            Debug.LogWarning($"Tag '{tag}' không được nhận diện. Không tạo hoạt ảnh.");
            return;
        }

        animator = GetComponent<AnyStateAnimator>();
        if (animator == null)
        {
            Debug.LogError("AnyStateAnimator component is missing on this GameObject.");
            return;
        }
        animator.AddAnimations(animations);
    }
}
