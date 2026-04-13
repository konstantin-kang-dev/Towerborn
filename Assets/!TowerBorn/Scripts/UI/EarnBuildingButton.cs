using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum EarnBuildingButtonState
{
    Disabled, Active, FullFilled
}

public class EarnBuildingButton : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Button button;
    [SerializeField] Image buttonImage;
    Color initialColor;
    [SerializeField] Color fullFilledColor;

    [SerializeField] TextMeshPro earnTMP;
    [SerializeField] Animator earnTMPAnimator;

    void Awake()
    {
        initialColor = buttonImage.color;
    }

    public void AddListener(UnityAction action)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    public void SetState(EarnBuildingButtonState state)
    {
        switch (state)
        {
            case EarnBuildingButtonState.Disabled:
                canvas.gameObject.SetActive(false);
                break;
            case EarnBuildingButtonState.Active:
                canvas.gameObject.SetActive(true);
                buttonImage.color = initialColor;
                break;
            case EarnBuildingButtonState.FullFilled:
                canvas.gameObject.SetActive(true);
                buttonImage.color = fullFilledColor;
                break;
            default:
                break;
        }
    }

    Coroutine earnAnimatorWaiter = null;
    public void SetEarnText(string text)
    {
        earnTMP.gameObject.SetActive(true);
        earnTMP.text = text;
        earnTMPAnimator.Play("FadeLift", 0, 0f);

        if (earnAnimatorWaiter != null) StopCoroutine(earnAnimatorWaiter);
        earnAnimatorWaiter = StartCoroutine(WaitForEarnTextAnimated(1.3f));
    }

    IEnumerator WaitForEarnTextAnimated(float duration)
    {
        yield return new WaitForSeconds(duration);
        earnTMP.gameObject.SetActive(false);
    }
}