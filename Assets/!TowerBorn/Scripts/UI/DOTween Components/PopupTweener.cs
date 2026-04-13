using DG.Tweening;
using System;
using UnityEngine;

public class PopupTweener : MonoBehaviour
{
    CanvasGroup canvasGroup;

    RectTransform rectTransform;

    Vector2 initialPos;
    float moveUpOffset = 100f;

    Sequence currentSequence;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPos = transform.position;

        canvasGroup = GetComponent<CanvasGroup>();

    }
    public void Show(Action callback = null)
    {
        gameObject.SetActive(true);

        currentSequence = DOTween.Sequence();

        Tween fadeInAnim = canvasGroup
            .DOFade(1f, 0.2f);

        Tween moveUpAnim = transform
            .DOMove(initialPos, 0.2f)
            .SetEase(Ease.OutBack, 2.5f);

        currentSequence.Append(fadeInAnim).Join(moveUpAnim).OnComplete(()=> callback?.Invoke());

    }

    public void Hide(Action callback = null)
    {
        currentSequence = DOTween.Sequence();

        Tween fadeOutAnim = canvasGroup
            .DOFade(0f, 0.2f);

        Tween moveDownAnim = transform
            .DOMove(initialPos - new Vector2(0, moveUpOffset), 0.2f)
            .SetEase(Ease.InBack, 2.5f);

        currentSequence.Append(fadeOutAnim).Join(moveDownAnim).OnComplete(() =>
        {
            callback?.Invoke();
            gameObject.SetActive(false);
        });
    }


    public bool IsAnimationPlaying() => currentSequence != null && currentSequence.active;

    public void ClearPlayingAnimation()
    {
        if (IsAnimationPlaying())
        {
            currentSequence.Kill();
        }
    }

}
