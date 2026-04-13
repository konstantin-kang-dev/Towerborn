using System;
using UnityEngine;

[Serializable]
public class ProjectileStats
{
    public float moveSpeed; 
    public float splashRadius = 0;
    public ProjectileType projectileType;
    [ShowIfEnum("projectileType", (int)ProjectileType.Mortar)] public float mortarHeight;
    public GameObject prefab;

    public ProjectileStats Clone()
    {
        return new ProjectileStats()
        {
            moveSpeed = moveSpeed,
            splashRadius = splashRadius,
            projectileType = projectileType,
            mortarHeight = mortarHeight,
            prefab = prefab,
        };
    }
}