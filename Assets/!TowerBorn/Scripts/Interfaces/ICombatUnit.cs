using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public interface ICombatUnit
{
    NavMeshAgent Agent { get; }
    EnemyDetector EnemyDetector { get; }
    IDamageable Target { get; }
    float AtkTimer { get; }
    void HandleAttack();

    void MoveToTarget();
}