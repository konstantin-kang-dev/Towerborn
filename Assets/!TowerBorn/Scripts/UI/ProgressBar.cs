using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] GameObject wrapper;
    [SerializeField] SpriteRenderer spriteRenderer;
    Color initialColor;
    float initialWidth;
    private Coroutine shineRoutine;

    private void Awake()
    {
        initialColor = spriteRenderer.color;
        initialWidth = spriteRenderer.size.x;
    }
    public void SetProgressValue(float progressValue)
    {
        StopAllCoroutines();
        if (spriteRenderer != null)
        {
            spriteRenderer.size = new Vector2(progressValue * initialWidth, spriteRenderer.size.y);
            StartCoroutine(ShineRoutine());
        }

    }

    public void SetVisibility(bool visibility)
    {
        wrapper.SetActive(visibility);
    }

    IEnumerator ShineRoutine()
    {
        float timer = 0;
        float duration = 0.2f;
        Color targetColor = Color.white;
        while (timer < duration)
        {
            Color color;
            if (timer < duration / 2f)
            {
                color = Color.Lerp(initialColor, targetColor, timer / (duration / 2f));
            }
            else
            {
                color = Color.Lerp(targetColor, initialColor, (timer - duration / 2f) / (duration / 2f));
            }
            spriteRenderer.color = color;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        spriteRenderer.color = initialColor;
    }

}