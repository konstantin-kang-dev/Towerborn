using System.Collections.Generic;
using UnityEngine;

public class EnemiesDB : MonoBehaviour
{
    public static EnemiesDB Instance { get; private set; }
    string configsPath = "EnemiesConfigs/";
    Dictionary<string, UnitConfig> enemyConfigs = new Dictionary<string, UnitConfig>();
    public Dictionary<string, UnitConfig> Configs => enemyConfigs;

    public bool isInitialized { get; private set; } = false;
    void Awake()
    {
        Instance = this;
        LoadConfigs();
    }

    public void Init()
    {
        isInitialized = true;
    }

    void LoadConfigs()
    {
        UnitConfig[] configs = Resources.LoadAll<UnitConfig>(configsPath);

        foreach (UnitConfig enemyConfig in configs)
        {
            UnitConfig copy = enemyConfig.Clone();
            enemyConfigs.Add(copy.id, copy); 
        }
    }

    void Update()
    {
        
    }
}
