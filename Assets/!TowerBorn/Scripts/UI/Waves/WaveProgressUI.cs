using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveProgressUI : MonoBehaviour
{
    public static WaveProgressUI Instance { get; private set; }
    SimpleShowHideAnimator simpleUIAnimator;
    [SerializeField] Slider waveSlider;
    [SerializeField] EnhancedText waveText;

    [Header("Slider Animation")]
    [SerializeField] float sliderTransitionSpeed = 2f;
    [SerializeField] bool useSmoothing = true;

    [Header("VFX")]
    [SerializeField] ParticleSystem waveCompleteVFX;
    [SerializeField] ParticleSystem waveLoseVFX;

    [Header("WaveInfo")]
    [SerializeField] public WaveInfoUI waveInfoUI;

    public bool isInitialized = false;

    private Coroutine sliderCoroutine;
    private float targetSliderValue;

    private void Awake()
    {
        simpleUIAnimator = GetComponent<SimpleShowHideAnimator>();
        Instance = this;
    }

    void Start()
    {

    }

    public void Init()
    {
        isInitialized = true;
        targetSliderValue = waveSlider.value;
    }

    public void SetVisibility(bool value, float delay = 0f)
    {
        StartCoroutine(VisibilityRoutine(value, delay));
    }

    IEnumerator VisibilityRoutine(bool value, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (value)
        {
            simpleUIAnimator.Show();
        }
        else
        {
            simpleUIAnimator.Hide();
        }
    }

    public void SetSliderValue(float value)
    {
        targetSliderValue = Mathf.Clamp01(value);

        if (!useSmoothing)
        {
            waveSlider.value = targetSliderValue;
            return;
        }

        if (sliderCoroutine != null)
        {
            StopCoroutine(sliderCoroutine);
        }

        sliderCoroutine = StartCoroutine(SmoothSliderTransition());
    }

    public void SetSliderValueInstant(float value)
    {
        targetSliderValue = Mathf.Clamp01(value);
        waveSlider.value = targetSliderValue;

        if (sliderCoroutine != null)
        {
            StopCoroutine(sliderCoroutine);
            sliderCoroutine = null;
        }
    }

    private IEnumerator SmoothSliderTransition()
    {
        float startValue = waveSlider.value;
        float endValue = targetSliderValue;
        float elapsedTime = 0f;
        float duration = Mathf.Abs(endValue - startValue) / sliderTransitionSpeed;

        if (duration <= 0)
        {
            waveSlider.value = endValue;
            yield break;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            float easedT = EaseInOutCubic(t);

            waveSlider.value = Mathf.Lerp(startValue, endValue, easedT);

            yield return null;
        }

        waveSlider.value = endValue;
        sliderCoroutine = null;
    }

    private float EaseInOutCubic(float t)
    {
        if (t < 0.5f)
        {
            return 4f * t * t * t;
        }
        else
        {
            float p = 2f * t - 2f;
            return 1f + p * p * p / 2f;
        }
    }

    public void SetWaveText(string text, bool withPulse = false)
    {
        if (withPulse)
        {
            waveText.SetTextWithPulse(text);
        }
        else
        {
            waveText.SetText(text);
        }
    }

    public void HandleWaveComplete()
    {
        PlayCompleteVFX();
    }

    public void PlayCompleteVFX()
    {
        waveCompleteVFX.Play();
        StartCoroutine(PulseRoutine());
    }

    public void HandleWaveLose()
    {
        PlayLoseVFX();
    }

    public void PlayLoseVFX()
    {
        waveLoseVFX.Play();
        StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        if (waveCompleteVFX != null)
        {
            waveCompleteVFX.Play();
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
    }
}