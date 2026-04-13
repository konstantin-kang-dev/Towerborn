using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof(Animator))]
public class BuildingAnimator : MonoBehaviour
{
    IBuilding _ownerBuilding;
    public Animator Animator {  get; private set; }

    public event Action OnAction;

    void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void Init(IBuilding building)
    {
        _ownerBuilding = building;
    }

    public void PlayAttack()
    {
        string attackAnimName = _ownerBuilding.CombatWeaponController.CurrentSpawnPoint.AttackAnimationName;

        Animator.Play(attackAnimName);
    }

    public void SetAttackSpeed(float value)
    {
        Animator.SetFloat("AttackSpeed", value);
    }

    public void TriggerSpawnVfx()
    {
        _ownerBuilding.BuildingVFXController.PlaySpawnVFX();
    }
}
