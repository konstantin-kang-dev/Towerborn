using System;
using UnityEditor;
using UnityEngine;

public interface IBuilding
{
    int InstanceId { get; }
    Vector3 Position { get; }
    BuildingConfig BuildingConfig { get; }
    AbilitiesController AbilitiesController { get; }
    CombatWeaponController CombatWeaponController { get; }
    BuildingAnimator BuildingAnimator { get; }
    BuildingVFXController BuildingVFXController { get; }
    BuildingTransparency BuildingTransparency { get; }
    BuildingAudioController BuildingAudioController { get; }
    BuildingStatsController StatsController { get; }
    CombatStateMachine CombatStateMachine { get; }

    bool IsInitialized { get; } 
    bool IsPlaced { get; } 
    void DestroyBuilding();
}