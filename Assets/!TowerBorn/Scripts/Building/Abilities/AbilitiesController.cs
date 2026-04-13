using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;
using static UnityEngine.UI.GridLayoutGroup;

public class AbilitiesController
{
    IBuilding _ownerBuilding;
    Transform _abilitiesParent;
    public List<IAbility> Abilities { get; private set; } = new();

    public bool IsInitialized { get; private set; } = false;
    public void Init(IBuilding building, Transform abilitiesParent)
    {
        _ownerBuilding = building;
        _abilitiesParent = abilitiesParent;
        List<AbilityConfig> configs = _ownerBuilding.BuildingConfig.CombatStats.Abilities;

        foreach (var abilityConfig in configs)
        {
            Debug.Log($"Initialize PA {abilityConfig.abilityName}");
            GameObject abilityGO = GameObject.Instantiate(abilityConfig.prefab, _abilitiesParent);
            abilityGO.transform.position = _abilitiesParent.position;
            abilityGO.transform.rotation = Quaternion.identity;

            IAbility ability = abilityGO.GetComponent<IAbility>();
            ability.Initialize(building, abilityConfig);
            Abilities.Add(ability);
        }

        _ownerBuilding.CombatWeaponController.OnAttack += HandleAbilitiesOnAttack;

        IsInitialized = true;
    }

    public void UpdateTick()
    {
        if (!IsInitialized) return;

        HandleAbilitiesCooldownUpdate();
    }

    public void HandleAbilitiesOnAttack(AttackData attackData)
    {
        foreach (var ability in Abilities)
        {
            ability.OnAttack(attackData);
        }
    }

    public void HandlePassiveAbilitiesOnHit(AttackData attackData)
    {
        foreach (var ability in Abilities)
        {
            ability.OnHit(attackData);
        }
    }
    public void HandlePassiveAbilitiesOnKill(AttackData attackData)
    {
        foreach (var ability in Abilities)
        {
            ability.OnKill(attackData);
        }
    }
    void HandleAbilitiesCooldownUpdate()
    {
        foreach (var ability in Abilities)
        {
            ability.CooldownUpdate();
        }
    }
}