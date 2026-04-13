using UnityEditor;
using UnityEngine;

public class AbilityFreezingShells : BaseAbility
{
    Debuff Debuff;

    public override void Initialize(IBuilding building, AbilityConfig config)
    {
        base.Initialize(building, config);
        Debuff = new Debuff()
        {
            senderId = Config.abilityName,
            value = Config.GetStatValue(AbilityStatType.DebuffValue),
            duration = Config.GetStatValue(AbilityStatType.Duration),
            type = DebuffType.Slowing,
        };
    }

    public override void OnHit(AttackData attackData)
    {
        base.OnHit(attackData);

        attackData.target.AddDebuff(Debuff);
    }
}