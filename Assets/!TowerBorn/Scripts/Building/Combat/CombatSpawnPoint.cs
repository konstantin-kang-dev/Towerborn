using System;
using UnityEditor;
using UnityEngine;


[Serializable]
public class CombatSpawnPoint
{
    public Transform SpawnPoint;
    public ParticleSystem Vfx;
    public string AttackAnimationName;
}