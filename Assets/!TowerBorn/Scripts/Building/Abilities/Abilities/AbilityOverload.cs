using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbilityOverload: BaseAbility
{
    Buff UnitBuff {  get; set; }

    float boost = 0f;
    float boostIncrement => Config.GetStatValue(AbilityStatType.BuffValue);
    int stacks = 0;

    BaseUnit currentTarget;
    public override void Initialize(IBuilding building, AbilityConfig config)
    {
        base.Initialize(building, config);

        UnitBuff = new Buff()
        {
            senderId = Config.abilityName,
            value = boost,
            duration = Config.GetStatValue(AbilityStatType.Duration),
            type = BuffType.AtkSpeed,
        };
    }

    public override void OnAttack(AttackData attackData)
    {
        base.OnAttack(attackData);
        if (currentTarget != attackData.target)
        {
            currentTarget = attackData.target;
            stacks = 0;
            boost = 0;
        }

        if (stacks < Config.GetStatValue(AbilityStatType.MaxStacks))
        {
            stacks += 1;
            boost = stacks * boostIncrement;
        }

        UpdateUnitBuff();
    }

    void UpdateUnitBuff()
    {
        UnitBuff.value = boost;
        Owner.StatsController.AddBuff(UnitBuff);
    }

}