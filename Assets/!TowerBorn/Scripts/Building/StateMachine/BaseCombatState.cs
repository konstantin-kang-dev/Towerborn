using UnityEditor;
using UnityEngine;

public class BaseCombatState : IState
{
    public BuildingState StateType { get; private set; }

    protected readonly IBuilding _ownerBuilding;
    protected EnemyDetector _enemyDetector => _ownerBuilding.CombatWeaponController.EnemyDetector;
    protected BaseCombatState(IBuilding building, BuildingState stateType)
    {
        _ownerBuilding = building;
        StateType = stateType;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}