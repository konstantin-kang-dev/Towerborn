using System;
using UnityEditor;
using UnityEngine;

public enum DamageableState
{
    Vulnerable, Invincible
}

public interface IDamageable
{
    Vector3 Position { get; }
    Collider Collider { get; }
    EntityUI EntityUI { get; }
    DamageableState DamageableState { get; }
    UnitMovementType MovementType { get; }

    event System.Action<BaseUnit> OnDestroyed;
    event System.Action<float> OnHpChanged;
    bool IsDead { get; }

    void TakeDamage(float damage);
    bool IsDeadlyDamage(float damage);
    bool IsFullHp();
    void Die();
}