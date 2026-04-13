
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnhancedAudioController : MonoBehaviour
{
    [SerializeField] public SfxType sfxType = SfxType.None;
    AudioSource audioSource;
    [SerializeField] List<AudioClip> clips = new List<AudioClip>();

    float initialVolume = 0;
    float currentVolume = 1f;
    [SerializeField] float minVolumeMultiplier = 0.8f;
    [SerializeField] float maxVolumeMultiplier = 1.1f;

    int prevKey = 0;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        initialVolume = audioSource.volume;
    }

    public void SetVolume(float value)
    {
        currentVolume = value;
        audioSource.volume = initialVolume * currentVolume;
    }

    public void Play(float delay = 0f, string customAudioId = "")
    {
        if (clips.Count == 0) return;

        string audioId = string.IsNullOrWhiteSpace(customAudioId) ? gameObject.name : customAudioId;

        if (!AudioManager.Instance.CanPlay($"{audioId}")) return;

        int key = 0;
        if(clips.Count > 1)
        {
            key = ProjectUtils.RandomIntExcept(0, clips.Count, prevKey);
        }

        float totalVolume = initialVolume * currentVolume;
        float randomVolume = Random.Range(totalVolume * minVolumeMultiplier, totalVolume * maxVolumeMultiplier);

        audioSource.volume = randomVolume;
        audioSource.clip = clips[key];

        prevKey = key;

        if(delay > 0f)
        {
            StartCoroutine(PlayRoutine(delay));
        }
        else
        {
            audioSource.Play();
        }
    }

    IEnumerator PlayRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Play();
    }
}
