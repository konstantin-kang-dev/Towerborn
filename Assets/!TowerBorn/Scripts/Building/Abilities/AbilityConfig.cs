using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

[Serializable]
public enum AbilityType
{
    None,
    Overload,
    FreezingShells,
    Multishot,
    FireBurner,
    IceBurner,
    RapidAccelerator,
}

[Serializable]
[Tooltip("All values are in percents of base values")]
public enum AbilityStatType
{
    Damage = 0,

    Cooldown = 2,
    Duration = 3,
    BuffValue = 4,
    DebuffValue = 5,
    AOERadius = 6,
    RequiredAttacks = 7,
    MaxStacks = 8,
    RollChance = 9,
    GoldGiven = 10,
}
[Serializable]
public class AbilityStat
{
    public AbilityStatType type = AbilityStatType.Damage;
    public float value = 0f;

    public AbilityStat Clone()
    {
        return new AbilityStat()
        {
            type = type,
            value = value
        };
    }
}
[CreateAssetMenu(fileName = "AbilityConfig", menuName = "TowerBorn/Abilities/AbilityConfig")]
public class AbilityConfig : ScriptableObject
{
    public string abilityName;
    public string displayName;

    [TextArea(3,10)]
    [Tooltip("Используйте {Damage}, {Cooldown}, {BuffValue} и тд как плейсхолдеры")]
    public string description;
    public string FormattedDescription => GetFormattedDescription();

    public Sprite icon;
    public AbilityType abilityType;
    public GameObject prefab;

    public List<AbilityStat> statList;

    public AbilityConfig Clone()
    {
        AbilityConfig config = ScriptableObject.CreateInstance<AbilityConfig>();

        config.abilityName = abilityName;
        config.displayName = displayName;
        config.description = description;
        config.icon = icon;
        config.abilityType = abilityType;
        config.prefab = prefab;
        config.statList = statList.Select(x => x.Clone()).ToList();

        return config;
    }
    public float GetStatValue(AbilityStatType statType)
    {
        AbilityStat result = statList.FirstOrDefault((x) => x.type == statType);
        return result != null ? result.value : 0f;
    }
    public string GetFormattedDescription()
    {
        string result = description;
        foreach (AbilityStatType statType in System.Enum.GetValues(typeof(AbilityStatType)))
        {
            string statText = "{" + statType.ToString() + "}";
            result = result.Replace(statText, GetStatValue(statType).ToString());
        }

        //result = result.Replace("{radius}", radius.ToString("F1")); // одна цифра после запятой

        return result;
    }
    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(abilityType.ToString()))
        {
            abilityName = $"Ability_{abilityType.ToString()}";

            abilityName = abilityName.Replace(" ", "");

#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(assetPath))
            {
                string newFileName = abilityName + ".asset";
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