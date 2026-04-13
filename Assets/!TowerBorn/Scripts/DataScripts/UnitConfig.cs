using System.IO;
using UnityEditor;
using UnityEngine;

public enum UnitMovementType
{
    Ground, Air, GroundAir
}

public enum UnitAttackType
{
    Melee, Range
}

[CreateAssetMenu(menuName = "TowerBorn/UnitConfig", fileName = "UnitConfig")]
public class UnitConfig : ScriptableObject
{
    public string id;
    public string displayName;
    public string description;
    public int level;

    public int baseCount = 10;
    public float countIncreaseRatio = 0.075f;
    public float baseSpawnInterval = 2f;
    public float spawnIntervalDecreaseRatio = 50f;

    public CombatStats combatStats;
    public GameObject prefab;

    public UnitConfig Clone()
    {
        UnitConfig clone = ScriptableObject.CreateInstance<UnitConfig>();

        clone.id = id;
        clone.displayName = displayName;
        clone.description = description;
        clone.level = level;
        clone.baseCount = baseCount;
        clone.countIncreaseRatio = countIncreaseRatio;
        clone.baseSpawnInterval = baseSpawnInterval;
        clone.spawnIntervalDecreaseRatio = spawnIntervalDecreaseRatio;
        clone.combatStats = combatStats.Clone();
        clone.prefab = prefab;
        return clone;
    }

    public float GetTotalHp()
    {
        return combatStats.GetStatValue(CombatStatType.Hp);
    }

    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(id))
        {
            string filename = $"{id}";

            filename = filename.Replace(" ", "");

#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(assetPath))
            {
                string newFileName = filename + ".asset";
                string directory = Path.GetDirectoryName(assetPath);
                string newPath = Path.Combine(directory, newFileName);

                if (Path.GetFileName(assetPath) != newFileName)
                {
                    AssetDatabase.RenameAsset(assetPath, newFileName);
                    AssetDatabase.SaveAssets();
                }
            }
#endif
        }
    }
}

