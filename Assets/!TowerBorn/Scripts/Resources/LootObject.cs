using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootObject : MonoBehaviour
{
    [SerializeField] private float animationTime = 0.8f;
    public RewardTarget Target;

    [SerializeField] Transform movingTransform = null;
    [SerializeField] Animator iconAnimator = null;
    int valueToAdd = 0;

    public void Init(RewardTarget target, int value)
    {
        movingTransform = transform;
        Target = target;
        valueToAdd = value;

        movingTransform
            .DOMove(Target.transform.position, animationTime)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                Target.AddValueAndAnimate(valueToAdd);

                Destroy(gameObject);
            });
    }
}
