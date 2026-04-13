using UnityEditor;
using UnityEngine;

public abstract class BaseAbility : MonoBehaviour, IAbility
{
    protected IBuilding Owner { get; private set; }
    protected AbilityConfig Config { get; private set; }

    public virtual void Initialize(IBuilding building, AbilityConfig config)
    {
        Owner = building;
        Config = config;
    }

    public virtual void OnAttack(AttackData attackData) { }
    public virtual void OnHit(AttackData attackData) { }
    public virtual void OnKill(AttackData attackData) { }
    public virtual void CooldownUpdate() { }
    public virtual void OnWaveStart() { }

}