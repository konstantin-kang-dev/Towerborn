using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class WavesGenerator : MonoBehaviour
{
    public static WavesGenerator Instance { get; private set; }

    [SerializeField] List<UnitConfig> units = new List<UnitConfig>();
    [SerializeField] List<UnitConfig> bosses = new List<UnitConfig>();

    public bool IsInitialized { get; private set; } = false;
    void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        IsInitialized = true;
    }

    public WaveConfig GenerateRandomWave(int waveNumber)
    {
        WaveConfig waveConfig = new WaveConfig();

        UnitConfig randomUnitConfig;

        bool isBossWave = waveNumber > 0 && waveNumber % 5 == 0;

        if (isBossWave)
        {
            int randomKey = Random.Range(0, bosses.Count);
            randomUnitConfig = bosses[randomKey].Clone();

            waveConfig.rewardResourceType = ResourceName.Gems;
            waveConfig.rewardAmount = Mathf.FloorToInt(ProjectConstants.BASE_BOSS_WAVE_REWARD_AMOUNT + (waveNumber * ProjectConstants.BASE_BOSS_WAVE_REWARD_AMOUNT * ProjectConstants.BASE_BOSS_WAVE_REWARD_AMOUNT_RATIO));

        }
        else
        {
            int randomKey = Random.Range(0, units.Count);
            randomUnitConfig = units[randomKey].Clone();

            waveConfig.rewardResourceType = ResourceName.Gold;
            waveConfig.rewardAmount = Mathf.FloorToInt(ProjectConstants.BASE_COMMON_WAVE_REWARD_AMOUNT + (waveNumber * ProjectConstants.BASE_COMMON_WAVE_REWARD_AMOUNT * ProjectConstants.BASE_COMMON_WAVE_REWARD_AMOUNT_RATIO));
        }

        randomUnitConfig.level = waveNumber;

        waveConfig.rewardForKill = Mathf.FloorToInt( ProjectConstants.BASE_ENEMY_KILL_REWARD + waveNumber * ProjectConstants.BASE_ENEMY_KILL_REWARD_WAVE_SCALE);

        waveConfig.spawnInterval = randomUnitConfig.baseSpawnInterval / (1f + waveNumber / randomUnitConfig.spawnIntervalDecreaseRatio);
        waveConfig.enemiesCount = Mathf.FloorToInt(randomUnitConfig.baseCount * (1f + waveNumber * randomUnitConfig.countIncreaseRatio));

        waveConfig.spawnDelay = 4f;
        waveConfig.waveNumber = waveNumber;
        waveConfig.enemyConfig = randomUnitConfig;
        
        return waveConfig;
    }

    public void TestGeneration(int waves)
    {
        for (int i = 1; i < waves; i++)
        {
            WaveConfig generatedConfig = GenerateRandomWave(i);

            UnitConfig enemyConfig = generatedConfig.enemyConfig;
            enemyConfig.combatStats.CacheStats();
            enemyConfig.combatStats.CalculateHpWithMultiplier(enemyConfig.level, ProjectConstants.BASE_ENEMY_HP_MULTIPLIER_PER_LEVEL);
            Debug.Log($"______________________________________________________________________");
            Debug.Log($"_________[Wave Generation]_________");
            Debug.Log($"[WaveNumber] {generatedConfig.waveNumber}");
            Debug.Log($"[EnemiesCount] {generatedConfig.enemiesCount}");
            Debug.Log($"[SpawnDelay] {generatedConfig.spawnDelay}");
            Debug.Log($"[SpawnInterval] {generatedConfig.spawnInterval}");
            Debug.Log($"[Enemy] {generatedConfig.enemyConfig.displayName}");
            Debug.Log($"[Enemy HP] {generatedConfig.enemyConfig.GetTotalHp()}");
            Debug.Log($"[RewardResourceType] {generatedConfig.rewardResourceType}");
            Debug.Log($"[RewardAmount] {generatedConfig.rewardAmount}");
            Debug.Log($"[RewardForKill] {generatedConfig.rewardForKill}");
        }
    }
}

