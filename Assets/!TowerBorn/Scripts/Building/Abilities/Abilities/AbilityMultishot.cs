
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AbilityMultishot : BaseAbility
{
    public int TargetsCount => (int)Config.GetStatValue(AbilityStatType.BuffValue);
    public override void Initialize(IBuilding building, AbilityConfig config)
    {
        base.Initialize(building, config);
    }

    public override void OnAttack(AttackData attackData)
    {
        base.OnAttack(attackData);

        List<BaseUnit> extraUnits = Owner.CombatWeaponController.EnemyDetector.Targets.ToList();
        extraUnits.Remove(attackData.target);

        extraUnits = extraUnits.GetRange(0, TargetsCount <= extraUnits.Count ? TargetsCount : extraUnits.Count);

        foreach (BaseUnit unit in extraUnits)
        {
            if (unit == null) continue;

            AttackData extraAttackData = new AttackData()
            {
                sender = Owner,
                damage = Owner.StatsController.TotalDamage,
                target = unit,
            };

            ProjectileStats projectileStats = Owner.StatsController.CalculatedCombatStats.projectileStats;

            ProjectileInfo projectileInfo = new ProjectileInfo()
            {
                config = projectileStats,
                damage = Owner.StatsController.TotalDamage,
                sender = Owner,
                target = unit,
            };

            GameObject projectileGO = Instantiate(projectileStats.prefab, Owner.CombatWeaponController.CurrentSpawnPointTransform.position, Quaternion.identity);
            Projectile projectileComp = projectileGO.GetComponent<Projectile>();
            projectileComp.Init(attackData, projectileInfo);
        }
    }
}