using UnityEditor;
using UnityEngine;

public static class BuildingStatCalculator
{
    private static float baseAttackInterval => ProjectConstants.BASE_ATK_INTERVAL;
    private static float baseAttackSpeed => ProjectConstants.BASE_ATK_SPEED;

    public static float AttackSpeedToInterval(float attackSpeed)
    {
        attackSpeed = Mathf.Abs(attackSpeed);
        attackSpeed = Mathf.Max(0.01f, attackSpeed);

        return baseAttackInterval * (baseAttackSpeed / attackSpeed);
    }

    public static float IntervalToAttackSpeed(float interval)
    {
        interval = Mathf.Abs(interval);
        interval = Mathf.Max(0.01f, interval);

        return baseAttackSpeed * (baseAttackInterval / interval);
    }
}