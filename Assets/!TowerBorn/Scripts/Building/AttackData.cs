using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class AttackData
{
    public float damage;
    public IBuilding sender;
    public BaseUnit target;
}