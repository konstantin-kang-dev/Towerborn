using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    public bool IsInitialized { get; private set; } = false;

    [SerializeField] List<Transform> _enemiesDestinations = new List<Transform>();

    [SerializeField] Transform spawnPoint;
    [SerializeField] int poolSize = 100;

    public int spawnedCount = 0;

    private Dictionary<UnitConfig, Queue<BaseUnit>> unitPools = new Dictionary<UnitConfig, Queue<BaseUnit>>();

    private List<BaseUnit> activeUnits = new List<BaseUnit>();

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        IsInitialized = true;
    }

    private void CreatePool(UnitConfig config)
    {
        if (unitPools.ContainsKey(config))
            return;

        Queue<BaseUnit> pool = new Queue<BaseUnit>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemyGO = Instantiate(config.prefab, transform);
            enemyGO.SetActive(false);

            BaseUnit unit = enemyGO.GetComponent<BaseUnit>();
            pool.Enqueue(unit);
        }

        unitPools[config] = pool;
    }

    public BaseUnit SpawnEnemy(UnitConfig config)
    {
        if (!unitPools.ContainsKey(config))
        {
            CreatePool(config);
        }

        Queue<BaseUnit> pool = unitPools[config];
        BaseUnit unit;

        if (pool.Count > 0)
        {
            unit = pool.Dequeue();
        }
        else
        {
            Debug.LogWarning($"Pool for {config.name} is empty, creating new unit");
            GameObject enemyGO = Instantiate(config.prefab, transform);
            unit = enemyGO.GetComponent<BaseUnit>();
        }

        Vector3 spawnPos = spawnPoint.position;
        Vector3 randomOffset = new Vector3(0, 0, Random.Range(-0.5f, 0.5f));
        spawnPos += randomOffset;

        unit.transform.position = spawnPos;
        unit.transform.rotation = spawnPoint.rotation;
        unit.gameObject.SetActive(true);

        unit.Init(config);
        unit.SetDestination(0, _enemiesDestinations[0].position);

        activeUnits.Add(unit);
        spawnedCount++;

        return unit;
    }

    public Transform GetDestination(int currentDestinationIndex)
    {
        if(_enemiesDestinations.Count - 1 < currentDestinationIndex) return null;

        return _enemiesDestinations[currentDestinationIndex];
    }

    public void ReturnToPool(BaseUnit unit, UnitConfig config)
    {
        if (unit == null || !unitPools.ContainsKey(config))
            return;

        activeUnits.Remove(unit);

        unit.gameObject.SetActive(false);
        unit.ResetUnit();
        unitPools[config].Enqueue(unit);

        spawnedCount--;
    }

    public List<BaseUnit> GetActiveUnits()
    {
        return activeUnits;
    }

    public void ClearAllUnits()
    {
        foreach (var unit in activeUnits.ToArray())
        {
            if (unit != null)
            {
                ReturnToPool(unit, unit.Config);
            }
        }
        activeUnits.Clear();
        spawnedCount = 0;
    }
}