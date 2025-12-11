using System.Collections.Generic;
using UnityEngine;

public class AnimationController  : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public void ChangeAnim(string animName)
    {
        animator.SetTrigger(animName);
    }
}