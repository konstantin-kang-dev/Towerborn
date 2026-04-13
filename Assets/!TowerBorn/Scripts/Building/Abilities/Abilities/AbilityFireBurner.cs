using UnityEditor;
using UnityEngine;

public class AbilityFireBurner : BaseAbility
{
    Debuff Debuff;

    float TotalDamage => ProjectUtils.CalculatePercent(Owner.StatsController.TotalDamage, Config.GetStatValue(AbilityStatType.Damage));
    public override void Initialize(IBuilding building, AbilityConfig config)
    {
        base.Initialize(building, config);
        Debuff = new Debuff()
        {
            senderId = Config.abilityName + Owner.InstanceId,
            value = TotalDamage,
            duration = Config.GetStatValue(AbilityStatType.Duration),
            type = DebuffType.PeriodicDamage,
        };
    }

    public override void OnHit(AttackData attackData)
    {
        base.OnHit(attackData);

        attackData.target.AddDebuff(Debuff);
    }
}