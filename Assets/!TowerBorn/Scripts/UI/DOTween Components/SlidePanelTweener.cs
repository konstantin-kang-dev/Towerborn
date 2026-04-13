using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;

public class SlidePanelTweener : MonoBehaviour
{
    [SerializeField] RectTransform visiblePanelRect;
    [SerializeField] float moveYOffset = 150f;
    float rectHeight;

    private void Awake()
    {
        rectHeight = visiblePanelRect.rect.height;
    }
    public void Show(Action callback = null)
    {
        gameObject.SetActive(true);

        Tween slideAnim = visiblePanelRect
            .DOMoveY(-moveYOffset, 0.3f)
            .SetEase(Ease.OutBack)
            .OnComplete(()=> callback?.Invoke());

    }

    public void Hide(Action callback = null)
    {
        Tween slideAnim = visiblePanelRect
            .DOMoveY(-rectHeight, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(()=>
            {
                callback?.Invoke();
                gameObject.SetActive(false);
            });

    }
}