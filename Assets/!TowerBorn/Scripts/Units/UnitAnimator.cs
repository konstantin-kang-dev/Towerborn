using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public enum UnitAnimationState
{
    Idle, Run, Attack
}
[RequireComponent(typeof(Animator))]
public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] string idleAnimationName = "Idle";
    [SerializeField] string runAnimationName = "Run";
    [SerializeField] string attackAnimationName = "Attack";

    Coroutine attackCoroutine;
    private void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
    }

    private void Update()
    {

    }

    public void ChangeAnimation(UnitAnimationState state)
    {
        string targetAnimationName = idleAnimationName;
        switch (state)
        {
            case UnitAnimationState.Idle:
                targetAnimationName = idleAnimationName;
                break;
            case UnitAnimationState.Run:
                targetAnimationName = runAnimationName; 
                break;
            case UnitAnimationState.Attack:
                targetAnimationName = attackAnimationName;
                break;
        }

        animator.CrossFade(targetAnimationName, 0.2f, -1, 0f);
    }

    public void SetAnimatorSpeed(float speed)
    {
        animator.speed = speed;
    }

}
