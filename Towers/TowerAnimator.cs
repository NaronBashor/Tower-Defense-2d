using System.Collections.Generic;
using UnityEngine;

public class TowerAnimator : MonoBehaviour
{
    private List<Animator> animators = new List<Animator>();

    public void SetAnimators(List<Animator> animatorComponents)
    {
        animators = animatorComponents;
    }

    public void PlayAttackAnimation()
    {
        foreach (var animator in animators) {
            if (animator != null) {
                animator.SetTrigger("Attack");
            }
        }
    }
}
