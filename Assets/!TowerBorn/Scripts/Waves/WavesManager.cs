using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using TowerBorn.SaveSystem;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.Rendering.STP;
using static UnityEngine.EventSystems.EventTrigger;

public class WavesManager : MonoBehaviour
{
    public static WavesManager Instance { get; private set; }

    public WaveConfig currentWaveConfig {get; private set;}

    public int currentWaveNumber = 1;

    Coroutine waveRoutine;

    public event Action<WaveConfig> OnWaveCompleted;
    public event Action<WaveConfig> OnWaveFailed;
    public event Action<WaveConfig> OnWaveEnd;

    public int currentWaveEnemiesTotal = 0;
    public int currentWaveEnemiesLeft = 0;

    public List<BaseUnit> spawnedEnemies = new List<BaseUnit>();

    [SerializeField] ParticleSystem portalVFX;
    float initialPortalScale = 1f;
    public float WaveProgress { get; private set; } = 0f;
    public bool IsInitialized { get; private set; } = false;

    private void Awake()
    {
        if(portalVFX != null)
        {
            initialPortalScale = portalVFX.transform.localScale.x;
            portalVFX.transform.localScale = new Vector3(0, 0, 0);
        }
        Instance = this;
    }

    public void Init()
    {
        currentWaveNumber = 1;
        IsInitialized = true;
    }

    private void FixedUpdate()
    {
        if (!IsInitialized) return;
    }
    public void StartWavesManagement()
    {
        Debug.Log($"Start wave management");
        spawnedEnemies.Clear();

        OnWaveEnd += WaveEndHandler;
        OnWaveCompleted += WaveCompletedHandler;
        OnWaveFailed += WaveLoseHandler;

        WaveProgressUI.Instance.SetVisibility(true);
        WaveProgressUI.Instance.SetSliderValue(0);

        ShowPortal();
        StartWave();
    }
    public void StopWavesManagement()
    {
        Debug.Log($"Stop wave management");
        foreach (BaseUnit enemy in spawnedEnemies.ToList())
        {
            if ((enemy as MonoBehaviour) == null) continue;
            enemy.Die();
        }

        spawnedEnemies.Clear();

        currentWaveNumber = 1;
        currentWaveConfig = null;

        HidePortal();

        OnWaveEnd -= WaveEndHandler;
        OnWaveCompleted -= WaveCompletedHandler;
        OnWaveFailed -= WaveLoseHandler;

        if (waveRoutine != null) StopCoroutine(waveRoutine);

        WaveProgressUI.Instance.SetVisibility(false);

        KillAllEnemies();
    }

    public void ShowPortal()
    {
        StartCoroutine(PortalScaleRoutine(true));
    }

    public void HidePortal()
    {
        StartCoroutine(PortalScaleRoutine(false));
    }
    IEnumerator PortalScaleRoutine(bool state)
    {
        float targetScale = state ? initialPortalScale : 0f;
        float scale = portalVFX.transform.localScale.x;

        while (portalVFX.transform.localScale.x != targetScale)
        {
            scale = Mathf.MoveTowards(scale, targetScale, 3f * Time.deltaTime);
            portalVFX.transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        portalVFX.transform.localScale = new Vector3(targetScale, targetScale, targetScale);
    }

    public void StartWave()
    {
        foreach (BaseUnit enemy in spawnedEnemies.ToList())
        {
            if ((enemy as MonoBehaviour) == null) continue;
            enemy.Die();
        }

        spawnedEnemies.Clear();

        currentWaveConfig = null;
        currentWaveConfig = WavesGenerator.Instance.GenerateRandomWave(currentWaveNumber);

        currentWaveEnemiesTotal = currentWaveConfig.GetTotalEnemies();
        currentWaveEnemiesLeft = currentWaveConfig.GetTotalEnemies();

        UpdateWavesText();

        if (waveRoutine != null) StopCoroutine(waveRoutine);

        waveRoutine = StartCoroutine(WaveCoroutine(currentWaveConfig));
    }

    public void EndWave()
    {
        if(currentWaveConfig == null) return;
        
        OnWaveEnd?.Invoke(currentWaveConfig);
    }

    public void SkipWaves(int count = 1)
    {
        int initialWaveNumber = currentWaveNumber;
        StopWavesManagement();
        if (waveRoutine != null) StopCoroutine(waveRoutine);

        int targetWaveNumber = initialWaveNumber + count;

        for (int i = 0; i < count; i++)
        {
            WaveConfig waveConfig = WavesGenerator.Instance.GenerateRandomWave(initialWaveNumber + i);

            for (int j = 0; j < waveConfig.enemiesCount; j++)
            {
                ResourcesManager.AddResource(ResourceName.Gold, waveConfig.rewardForKill);
            }
            ResourcesManager.AddResource(waveConfig.rewardResourceType, waveConfig.rewardAmount);
        }

        currentWaveNumber = targetWaveNumber;
        StartWavesManagement();
    }

    public void SetWave(int waveNumber = 1)
    {
        StopWavesManagement();

        int initialWaveNumber = currentWaveNumber;

        if (waveRoutine != null) StopCoroutine(waveRoutine);

        int targetWaveNumber = waveNumber;
        int diff = targetWaveNumber - currentWaveNumber;
        WaveConfig waveConfig = WavesGenerator.Instance.GenerateRandomWave(initialWaveNumber);

        for (int i = 0; i < diff; i++)
        {
            waveConfig = WavesGenerator.Instance.GenerateRandomWave(initialWaveNumber + i);

            for (int j = 0; j < waveConfig.enemiesCount; j++)
            {
                ResourcesManager.AddResource(ResourceName.Gold, waveConfig.rewardForKill);
            }
            ResourcesManager.AddResource(waveConfig.rewardResourceType, waveConfig.rewardAmount);
        }

        currentWaveNumber = targetWaveNumber;

        StartWavesManagement();
    }

    IEnumerator WaveCoroutine(WaveConfig config)
    {
        Debug.Log($"Started wave! {config.waveNumber}");

        float waveProgressUpdateDelay = Math.Clamp(config.spawnDelay, 0, 1f);

        yield return new WaitForSeconds(config.spawnDelay - waveProgressUpdateDelay);

        WaveProgressUI.Instance.SetSliderValue(0);

        yield return new WaitForSeconds(waveProgressUpdateDelay);

        for (int i = 0; i < config.enemiesCount; i++)
        {
            BaseUnit enemy = EnemyManager.Instance.SpawnEnemy(config.enemyConfig);
            spawnedEnemies.Add(enemy);

            enemy.OnDestroyed += HandleEnemyDeath;

            yield return new WaitForSeconds(config.spawnInterval);
        }

    }

    void WaveEndHandler(WaveConfig config)
    {
        Debug.Log($"Ended wave! {config.waveNumber}");
        if (waveRoutine != null) StopCoroutine(waveRoutine);

        foreach (BaseUnit enemy in spawnedEnemies.ToList())
        {
            if ((enemy as MonoBehaviour) == null) continue;
            enemy.OnDestroyed -= HandleEnemyDeath;
            enemy.Die();
        }

        spawnedEnemies.Clear();

        if(GameManager.Instance.GameState == GameState.Playing)
        {
            StartWave();
        }
    }

    public void CompleteWave()
    {
        if(currentWaveConfig == null) return;

        currentWaveNumber += 1;

        OnWaveCompleted?.Invoke(currentWaveConfig);
        EndWave();
    }
    void WaveCompletedHandler(WaveConfig config)
    {
        Debug.Log($"Completed wave! {config.waveNumber}");

        SavesManager.SaveProgress();

        WaveProgressUI.Instance.HandleWaveComplete();
        StartCoroutine(WaveCompleteRoutine(config));

    }

    public void LoseWave()
    {
        if (currentWaveConfig == null) return;

        OnWaveFailed?.Invoke(currentWaveConfig);
        EndWave();
    }

    void WaveLoseHandler(WaveConfig config)
    {
        Debug.Log($"Lose wave! {config.waveNumber}");
        SavesManager.SaveProgress();

        WaveProgressUI.Instance.HandleWaveLose();
    }

    IEnumerator WaveCompleteRoutine(WaveConfig config)
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        int lootQuantity = 5;
        int[] rewardParts = ProjectUtils.DivideIntoParts(config.GetReward(), lootQuantity);
        for (int i = 0; i < 5; i++)
        {
            LootManager.Instance.HandleSpawnLoot(config.rewardResourceType, rewardParts[i], screenCenter);

            yield return new WaitForSeconds(0.15f);
        }
    }
    public void HandleEnemyDeath(BaseUnit enemy)
    {
        ////Calls only on enemies spawned in Campaign wave
        if (GameManager.Instance.GameState != GameState.Playing) return;

        spawnedEnemies.Remove(enemy);
        currentWaveEnemiesLeft -= 1;

        WaveProgressUI.Instance.SetSliderValue(1f - (float)currentWaveEnemiesLeft / (float)currentWaveEnemiesTotal);
        LootManager.Instance.HandleSpawnLoot(ResourceName.Gold, currentWaveConfig.rewardForKill, enemy.Position);
        //Debug.Log($"HandleEnemyDead! {currentWaveConfig.waveType}. Enemies left: {currentWaveEnemies}");

        if (currentWaveEnemiesLeft == 0)
        {
            CompleteWave();
        }

    }

    public void KillAllEnemies()
    {
        foreach (BaseUnit enemy in spawnedEnemies.ToList())
        {
            if ((enemy as MonoBehaviour) == null) continue;
            enemy.Die();
        }
        spawnedEnemies.Clear();
    }

    
    void UpdateWavesText()
    {
        string waveText = $"Wave {currentWaveConfig.waveNumber}";
        WaveProgressUI.Instance.SetWaveText(waveText);
        WaveProgressUI.Instance.waveInfoUI.SetInfo(currentWaveEnemiesTotal, currentWaveConfig.GetReward());
    }
    

    public void ApplySave()
    {
        //UpdateWavesText();
    }
}
