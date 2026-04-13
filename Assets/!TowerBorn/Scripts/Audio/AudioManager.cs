using System;
using System.Collections.Generic;
using UnityEngine;

public enum SfxType
{
    None = 0,
    Coin = 1,
    EnemyHit = 2,
    EnemyDeath = 3,

    UIClick = 100,
    UISwitch = 101,
    UISlideOpen = 102,
    UISlideClose = 103,
    UISlideClickOpen = 104,
    UISlideClickClose = 105,
}

public class AudioManager : MonoBehaviour
{
    [Serializable]
    public class AudioCategory
    {
        public string categoryName;
        public int maxSoundsPerInterval;
        public float interval;

        [HideInInspector]
        public List<float> playTimestamps = new List<float>();
    }

    public static AudioManager Instance { get; private set; }

    [Header("Audio categories")]
    [SerializeField]
    private AudioCategory[] audioCategories = new AudioCategory[] { };

    [Header("Categories Default settings")]
    [SerializeField] private int defaultMaxSounds = 1;
    [SerializeField] private float defaultInterval = 0.1f;

    private Dictionary<string, AudioCategory> categoryDict;
    public AudioSource MusicAS { get; private set; }
    [SerializeField] AudioClip mainMusicAC;

    Dictionary<SfxType, EnhancedAudioController> sfxsCached = new Dictionary<SfxType, EnhancedAudioController>();

    public event Action<float> OnSfxVolumeChange;
    public event Action<float> OnMusicVolumeChange;

    public bool IsInitialized { get; private set; } = false;
    private void Awake()
    {
        MusicAS = GetComponent<AudioSource>();
        InitializeCategoryDictionary();

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            EnhancedAudioController enhancedAudioController = child.GetComponent<EnhancedAudioController>();
            if (enhancedAudioController == null) continue;

            sfxsCached.Add(enhancedAudioController.sfxType, enhancedAudioController);
        }

        Instance = this;
    }
    public void Init()
    {
        PlayMainMusic();

        ResourcesManager.OnResourceAdded += (Resource resource) =>
        {
            switch (resource.resourceName)
            {
                default:
                    sfxsCached[SfxType.Coin].Play();
                    break;
            }
        };

        IsInitialized = true;   
    }

    public void PlayMainMusic()
    {
        MusicAS.clip = mainMusicAC;
        MusicAS.loop = true;
        MusicAS.Play();
    }

    public void PlaySFX(SfxType sfxType)
    {
        if (!sfxsCached.ContainsKey(sfxType)) return;

        sfxsCached[sfxType].Play();
    }

    public void SetSFXVolume(float volume)
    {
        foreach (var sfxBlock in sfxsCached)
        {
            EnhancedAudioController sfx = sfxBlock.Value;
            sfx.SetVolume(volume);
        }

        OnSfxVolumeChange?.Invoke(volume);
    }
    public void SetMusicVolume(float volume)
    {
        MusicAS.volume = volume / 10f;

        OnMusicVolumeChange?.Invoke(volume);
    }

    #region AUDIO_CATEGORIES

    private void InitializeCategoryDictionary()
    {
        categoryDict = new Dictionary<string, AudioCategory>();
        foreach (var category in audioCategories)
        {
            if (!string.IsNullOrEmpty(category.categoryName))
            {
                categoryDict[category.categoryName] = category;
            }
        }
    }

    public bool CanPlay(string audioFileName)
    {
        if (string.IsNullOrEmpty(audioFileName))
            return false;

        AudioCategory category = GetCategoryByFileName(audioFileName);

        if (category == null)
        {
            return CanPlayDefault(audioFileName);
        }

        float currentTime = Time.time;
        category.playTimestamps.RemoveAll(timestamp =>
            currentTime - timestamp > category.interval);

        if (category.playTimestamps.Count < category.maxSoundsPerInterval)
        {
            category.playTimestamps.Add(currentTime);
            return true;
        }

        return false;
    }

    private AudioCategory GetCategoryByFileName(string audioFileName)
    {
        string lowerFileName = audioFileName.ToLower();

        foreach (var kvp in categoryDict)
        {
            if (lowerFileName.StartsWith(kvp.Key.ToLower()))
            {
                return kvp.Value;
            }
        }

        return null;
    }

    private bool CanPlayDefault(string audioFileName)
    {
        if (!defaultPlayTimestamps.ContainsKey(audioFileName))
        {
            defaultPlayTimestamps[audioFileName] = new List<float>();
        }

        var timestamps = defaultPlayTimestamps[audioFileName];
        float currentTime = Time.time;

        timestamps.RemoveAll(t => currentTime - t > defaultInterval);

        if (timestamps.Count < defaultMaxSounds)
        {
            timestamps.Add(currentTime);
            return true;
        }

        return false;
    }

    private Dictionary<string, List<float>> defaultPlayTimestamps = new Dictionary<string, List<float>>();

    public void ClearAllTimestamps()
    {
        foreach (var category in audioCategories)
        {
            category.playTimestamps.Clear();
        }
        defaultPlayTimestamps.Clear();
    }

    public void GetCategoryStats(string prefix)
    {
        if (categoryDict.TryGetValue(prefix, out AudioCategory category))
        {
            Debug.Log($"Category {prefix}: {category.playTimestamps.Count}/{category.maxSoundsPerInterval} sounds");
        }
    }

    public void SetCategoryLimit(string prefix, int maxSounds, float interval)
    {
        if (categoryDict.TryGetValue(prefix, out AudioCategory category))
        {
            category.maxSoundsPerInterval = maxSounds;
            category.interval = interval;
        }
    }

    #endregion

#if UNITY_EDITOR

    private void OnGUI()
    {
        if (!Application.isPlaying) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 500));
        GUILayout.Label("Audio Categories Status:");

        foreach (var category in audioCategories)
        {
            float currentTime = Time.time;
            int activeCount = category.playTimestamps.RemoveAll(t => currentTime - t > category.interval);
            activeCount = category.playTimestamps.Count;

            string status = $"{category.categoryName}: {activeCount}/{category.maxSoundsPerInterval}";
            GUILayout.Label(status);
        }

        GUILayout.EndArea();
    }
#endif

    void Update()
    {
        
    }
}
