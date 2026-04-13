using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class EnhancedText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp;
    [SerializeField] ParticleSystem vfx;
    Coroutine pulseRoutine = null;

    private void Awake()
    {

    }

    public void SetText(string text)
    {
        tmp.text = text;
    }

    public void SetTextWithPulse(string text)
    {
        if(pulseRoutine != null) StopCoroutine(pulseRoutine);
        pulseRoutine = StartCoroutine(PulseRoutine(text));
    }

    private IEnumerator PulseRoutine(string text)
    {
        if (vfx != null)
        {
            vfx.Play();
        }

        float animTime = 0.1f;
        float passedTime = 0;

        Vector3 startScale = new Vector3(1, 1, 1);
        Vector3 scaleDelta = new Vector3(0.1f, 0.1f, 0);

        bool isTextSet = false;

        while (passedTime < animTime)
        {
            passedTime += Time.deltaTime;
            transform.localScale = startScale + scaleDelta * Mathf.Sin(Mathf.PI + Mathf.PI * passedTime / animTime);

            if (passedTime >= animTime / 2f && !isTextSet)
            {
                tmp.text = text;
                isTextSet = true;
            }
            yield return new WaitForFixedUpdate();
        }

        pulseRoutine = null;
    }
}