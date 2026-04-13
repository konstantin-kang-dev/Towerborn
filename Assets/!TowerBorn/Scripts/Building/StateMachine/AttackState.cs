using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackState : BaseCombatState
{
    public AttackState(IBuilding building, BuildingState stateType)
        : base(building, stateType) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        BaseUnit target = _ownerBuilding.CombatWeaponController.Target;

        bool isNullTarget = (target as MonoBehaviour) == null || !target.isActiveAndEnabled;
        bool isTargetOutOfRange = !_ownerBuilding.CombatWeaponController.IsTargetInRange();

        if (isNullTarget || isTargetOutOfRange)
        {
            _ownerBuilding.CombatStateMachine.ChangeState(BuildingState.Idle);
            return;
        }

        _ownerBuilding.CombatWeaponController.RotateTowardsTarget();
        _ownerBuilding.CombatWeaponController.HandleAttack();
    }

    public override void Exit()
    {
        base.Exit();
    }
}