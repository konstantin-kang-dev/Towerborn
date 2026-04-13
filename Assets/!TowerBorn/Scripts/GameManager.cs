using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerBorn.SaveSystem;

[Serializable]
public enum GameState
{
    Playing, Paused, Idle
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    float timerInterval = 1;
    float timer = 0;
    public event Action OnTimerUpdate;

    public GameState GameState;

    public bool IsInitialized { get; private set; } = false;
    public bool IsSceneInitialized { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause) SavesManager.SaveAll();
    }

    private void OnApplicationQuit()
    {
        SavesManager.SaveAll();
    }

    void Start()
    {
        InitializeScene();

        if (IsSceneInitialized)
        {
            SavesManager.OnSavesLoaded += ApplySaves;
            SavesManager.OnFirstLaunch += FirstLaunchHandler;
        }
        SavesManager.LoadAll();
        SavesManager.IsFirstLaunch();

        if (IsSceneInitialized)
        {
            Init();
        }
    }

    void InitializeScene()
    {
        List<string> failedInits = new List<string>();

        SafeInit(() => BuildingsDB.Instance?.Init(), "BuildingsDB", failedInits);
        SafeInit(() => EnemiesDB.Instance.Init(), "EnemiesDB", failedInits);
        SafeInit(() => WavesGenerator.Instance.Init(), "WavesGenerator", failedInits);
        SafeInit(() => BuildingSelectionManager.Instance.Init(), "BuildingSelectionManager", failedInits);
        SafeInit(() => WavesManager.Instance.Init(), "WavesManager", failedInits);
        SafeInit(() => EnemyManager.Instance.Init(), "EnemyManager", failedInits);
        SafeInit(() => BuildingsManager.Instance.Init(), "BuildingsManager", failedInits);
        SafeInit(() => WaveProgressUI.Instance.Init(), "WaveProgressUI", failedInits);
        SafeInit(() => AudioManager.Instance.Init(), "AudioManager", failedInits);
        SafeInit(() => ResearchUI.Instance.Init(), "ResearchUI", failedInits);
        SafeInit(() => DeckUI.Instance.Init(), "DeckUI", failedInits);

        if (failedInits.Count > 0)
        {
            Debug.LogError($"Initialization finished with errors in: {string.Join(", ", failedInits)}");
        }
        else
        {
            Debug.Log("All systems initalized successfully!");
            IsSceneInitialized = true;
        }
    }
    void SafeInit(Action initAction, string systemName, List<string> failedList)
    {
        try
        {
            initAction?.Invoke();
            Debug.Log($"{systemName} initialized successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in initialization {systemName}: {e.Message}");
            failedList.Add(systemName);
        }
    }

    void PostInitialization()
    {
        
    }
    void Init()
    {
        IsInitialized = true;
    }

    public void StartGame()
    {
        Debug.Log($"Started game!");

        GameState = GameState.Playing;
        LivesManager.Instance.ResetLives();
        WavesManager.Instance.StartWavesManagement();
        ResourcesManager.AddResource(ResourceName.Gold, 300);
        
        DeckUI.Instance.SetVisibility(true);

        PlayerStats.Instance.ResetStats();
        PlayerStats.Instance.StartTimer();
    }

    public void ResetGame()
    {
        SavesManager.DeleteAllSaves();
        Application.Quit();
    }

    public void EndGame()
    {
        Debug.Log($"Ended game!");

        GameState = GameState.Idle;
        BuildingSelectionManager.Instance.DeselectBuilding();
        BuildingsManager.Instance.DestroyAllBuildings();
        WavesManager.Instance.StopWavesManagement();
        GameStartUI.Instance.ResetGame();
        ResourcesManager.SetResource(ResourceName.Gold, 0);

        DeckUI.Instance.ResetDeck();
        DeckUI.Instance.SetVisibility(false);

        PlayerStats.Instance.StopTimer();
    }

    void ApplySaves()
    {
        Debug.Log("GameManager apply saves");
        ApplySettingsSave();
        ApplyProgressSave();
        ApplyResourcesSave();
        ApplyBuildingsSave();

        PostInitialization();
    }

    void ApplyProgressSave()
    {
        WavesManager.Instance.ApplySave();
        ResearchUI.Instance.ApplySave();
        BuildingsDB.Instance.FilterUnlockedConfigs();
    }

    void ApplySettingsSave()
    {
        GameSettings.ApplySave();
    }

    void ApplyResourcesSave()
    {
        ResourcesManager.ApplySave();
    }
    void ApplyBuildingsSave()
    {
        BuildingsManager.Instance.ApplySave();
    }

    public void FirstLaunchHandler()
    {
        BuildingsDB.Instance.UnlockDefaultCards();
    }

    public void SetTimescale(float timescale)
    {
        Time.timeScale = timescale;
    }


    private void FixedUpdate()
    {
        if(timer < timerInterval)
        {
            timer += Time.fixedDeltaTime;
        }
        else
        {
            timer = 0;
            OnTimerUpdate?.Invoke();
        }
    }

}
