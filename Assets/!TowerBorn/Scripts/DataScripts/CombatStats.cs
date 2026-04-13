
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Zenject;
using static MirzaBeig.ParticleSystems.Demos.DemoManager;
using static UnityEngine.UI.Image;

[Serializable]
public enum CombatStatType
{
    Hp, Damage, AtkRange, AtkInterval, MoveSpeed
}

[Serializable]
public enum CombatStatOwnerType
{
    Building, Enemy
}

[Serializable] 
public class CombatStat
{
    public CombatStatType statType;
    public float statValue = 0f;

    public CombatStat Clone()
    {
        return new CombatStat()
        {
            statType = statType,
            statValue = statValue,
        };
    }
}

[Serializable]
public class CombatStats
{
    public List<CombatStat> statsList = new List<CombatStat>() 
    {
        new CombatStat(){statType = CombatStatType.Damage},
        new CombatStat(){statType = CombatStatType.AtkRange},
        new CombatStat(){statType = CombatStatType.AtkInterval},
    };

    public Dictionary<CombatStatType, CombatStat> cachedStatsList = new Dictionary<CombatStatType, CombatStat> ();

    public UnitMovementType movementType;

    public ProjectileStats projectileStats;

    [SerializeField] private List<AbilityConfig> abilities;
    public List<AbilityConfig> Abilities => abilities;
    public CombatStats Clone()
    {
        return new CombatStats()
        {
            statsList = statsList.Select(item => item.Clone()).ToList(),
            movementType = movementType,
            projectileStats = projectileStats != null ? projectileStats.Clone() : null,
            abilities = this.abilities.Select((x) => x.Clone()).ToList(),
        };
    }
    public void CacheStats()
    {
        foreach (var stat in statsList)
        {
            if (cachedStatsList.ContainsKey(stat.statType)) continue;

            cachedStatsList.Add(stat.statType, stat);
        }
    }
    public void CalculateStats(CombatStatOwnerType ownerType, int level)
    {
        foreach (var stat in statsList)
        {
            stat.statValue *= ProgressionManager.GetStatMultiplier(ownerType, stat.statType, level);

            //Debug.Log($"Calculated stat: {stat.statType} value: {stat.statValue}");
        }

    }
    public void CalculateHpWithMultiplier(int level, float multiplier)
    {
        if (!cachedStatsList.ContainsKey(CombatStatType.Hp)) return;

        CombatStat hpStat = cachedStatsList[CombatStatType.Hp];

        hpStat.statValue *= 1 + (level - 1) * multiplier;
    }

    public float GetStatValue(CombatStatType type)
    {
        if(cachedStatsList.Count == 0) CacheStats();

        if (!cachedStatsList.ContainsKey(type)) return 0f;

        return cachedStatsList[type].statValue;
    }
}