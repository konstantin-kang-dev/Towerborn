using UnityEditor;
using UnityEngine;

public interface ICombatBuilding
{
    BuildingState CombatState { get; }
    BaseUnit Target { get; }
    float AtkTimer { get; }
    void HandleAttack();
    void Attack();
}