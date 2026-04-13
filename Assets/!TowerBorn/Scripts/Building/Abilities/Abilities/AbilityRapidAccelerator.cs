
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbilityRapidAccelerator : BaseAbility
{
    Buff Buff;

    float timer = 0f;
    float Cooldown => Config.GetStatValue(AbilityStatType.Cooldown);
    float AOERadius => Config.GetStatValue(AbilityStatType.AOERadius) * ProjectConstants.ATK_RANGE_MULTIPLIER;
    public override void Initialize(IBuilding building, AbilityConfig config)
    {
        base.Initialize(building, config);
        Buff = new Buff()
        {
            senderId = Config.abilityName,
            value = Config.GetStatValue(AbilityStatType.BuffValue),
            duration = Config.GetStatValue(AbilityStatType.Duration),
            type = BuffType.AtkSpeed,
        };
    }


    public override void CooldownUpdate()
    {
        base.CooldownUpdate();

        timer += Time.deltaTime;

        if (timer < Cooldown) return;

        timer -= Cooldown;
        Cast();
    }

    void Cast()
    {
        List<IBuilding> buildingsInRadius = BuildingsManager.Instance.GetBuildingsInRadius(Owner.Position, AOERadius, Owner);

        foreach (IBuilding b in buildingsInRadius)
        {
            b.StatsController.AddBuff(Buff);
        }

        //Debug.Log($"RapidAccelerator buffed buildings: {buildingsInRadius.Count}");
    }
}