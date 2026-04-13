using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SimpleShowHideAnimator : MonoBehaviour
{
    [Header("[SimpleShowHideAnimator] Show(); Hide();")]
    public Animator animator {  get; private set; }

    [SerializeField] string showAnimationName = "Show";
    [SerializeField] string hideAnimationName = "Hide";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Show()
    {
        animator.CrossFade(showAnimationName, 0.15f, 0, 0f);
    }
    public void Hide()
    {
        animator.CrossFade(hideAnimationName, 0.15f, 0, 0f);
    }
}