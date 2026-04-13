using UnityEditor;
using UnityEngine;

public interface IAbility
{
    void Initialize(IBuilding building, AbilityConfig config);
    void OnAttack(AttackData attackData);
    void OnHit(AttackData attackData);
    void OnKill(AttackData attackData);
    void CooldownUpdate();
}