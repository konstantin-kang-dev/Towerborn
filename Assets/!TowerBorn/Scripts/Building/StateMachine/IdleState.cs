using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IdleState : BaseCombatState
{
    public IdleState(IBuilding building, BuildingState stateType)
        : base(building, stateType) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        BaseUnit target = _enemyDetector.CurrentTarget;

        if ((target as MonoBehaviour) != null && target.isActiveAndEnabled)
        {
            _ownerBuilding.CombatWeaponController.UpdateTarget(target);

            _ownerBuilding.CombatStateMachine.ChangeState(BuildingState.Attack);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}