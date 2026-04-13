using UnityEditor;using UnityEngine;
using Zenject;

public class BuildingStatsController
{
    [Inject] public BuffsController BuffsController {  get; private set; }

    public CombatStats BaseCombatStats { get; private set; }
    public CombatStats CalculatedCombatStats { get; private set; }

    public float BaseAtkInterval => BaseCombatStats.GetStatValue(CombatStatType.AtkInterval);
    public float Damage => CalculatedCombatStats.GetStatValue(CombatStatType.Damage);
    public float TotalDamage => Damage + (Damage * BuffsController.CalculateBuffValue(BuffType.Damage) / 100f);

    public float TotalAtkInterval => BuildingStatCalculator.AttackSpeedToInterval(TotalAtkSpeed);
    public float AtkSpeed => BuildingStatCalculator.IntervalToAttackSpeed(CalculatedCombatStats.GetStatValue(CombatStatType.AtkInterval));
    public float TotalAtkSpeed => AtkSpeed + (AtkSpeed * BuffsController.CalculateBuffValue(BuffType.AtkSpeed) / 100f);
    public float TotalAtkRange => AtkRange;
    public float AtkRange => CalculatedCombatStats.GetStatValue(CombatStatType.AtkRange);

    public bool IsInitialized { get; private set; } = false;
    public void Init(CombatStats baseStats)
    {
        BaseCombatStats = baseStats.Clone();
        BaseCombatStats.CacheStats();

        BuffsController.Init();

        IsInitialized = true;
    }

    public void CalculateStats(int buildingLevel)
    {
        CalculatedCombatStats = BaseCombatStats.Clone();
        CalculatedCombatStats.CacheStats();
        CalculatedCombatStats.CalculateStats(CombatStatOwnerType.Building, buildingLevel);
    }

    public void AddBuff(Buff buff)
    {
        buff = buff.Clone();
        BuffsController.AddBuff(buff);
    }
    public void RemoveBuff(Buff buff)
    {
        BuffsController.RemoveBuff(buff);
    }

    public void UpdateBuffsDuration()
    {
        BuffsController.UpdateTimer();
    }
    public void UpdateTick()
    {
        if(!IsInitialized) return;

        UpdateBuffsDuration();
    }
}