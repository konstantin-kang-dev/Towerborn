using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardTarget : MonoBehaviour
{
    [SerializeField] private ResourceName rewardTargetType;

    Coroutine pulseCoroutine = null;
    [SerializeField] private Transform targetTransform = null;
    [NonSerialized] public Vector3 targetPosition = Vector3.zero;

    [Header("VFX")]
    [SerializeField] ParticleSystem vfx;

    void Start()
    {
        if (targetTransform == null)
            targetTransform = transform;

        targetPosition = targetTransform.position;
    }

    public void AddValueAndAnimate(int valueToAdd = 0)
    {
        ResourcesManager.AddResource(rewardTargetType, valueToAdd);

        ProjectUtils.Vibrate();

        if (pulseCoroutine == null)
        {
            pulseCoroutine = StartCoroutine(PulseAnimating());
        }
    }

    private IEnumerator PulseAnimating()
    {
        if (vfx != null)
        {
            vfx.Play();
        }

        float animTime = 0.1f;
        float passedTime = 0;

        Vector3 startScale = new Vector3(1, 1, 1);
        Vector3 scaleDelta = new Vector3(0.1f, 0.1f, 0);

        while (passedTime < animTime)
        {
            passedTime += Time.deltaTime;
            transform.localScale = startScale + scaleDelta * Mathf.Sin(Mathf.PI + Mathf.PI * passedTime / animTime);
            yield return new WaitForFixedUpdate();
        }
        transform.localScale = startScale;

        pulseCoroutine = null;
    }
}
