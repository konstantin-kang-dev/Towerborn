using GameIDSystem;
using System;
using System.IO;
using TowerBorn.SaveSystem;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.STP;
[Serializable]
public enum BuildingTier
{
    Tier1, Tier2, Tier3, Tier4, Tier5,
}

[Serializable]
public enum BuildingType
{
    Combat, NonCombat
}

[Serializable]
public class BuildingDefenseData
{
    public float atkRadius = 3f;
    public float atkInterval = 1f;
    public float atkDamage = 25f;

    public bool isSlowing = false;
    [ShowIfBool("isSlowing", true)] public float slowingPower = 0f;

    public int damageIncreaseStep = 5;

    public BuildingDefenseData Clone()
    {
        return new BuildingDefenseData
        {
            atkRadius = this.atkRadius,
            atkInterval = this.atkInterval,
            atkDamage = this.atkDamage,
            isSlowing = this.isSlowing,
            slowingPower = this.slowingPower,
            damageIncreaseStep = this.damageIncreaseStep,
        };
    }
}

[Serializable]
[CreateAssetMenu(menuName = "TowerBorn/BuildingConfig", fileName = "BuildingConfig")]
public class BuildingConfig : ScriptableObject
{
    [SerializeField] string id;
    public string Id => id;

    [SerializeField] BuildingTier buildingTier = BuildingTier.Tier1;
    public BuildingTier BuildingTier => buildingTier;

    [SerializeField] BuildingType buildingType = BuildingType.Combat;
    public BuildingType BuildingType => buildingType;

    [SerializeField] int level = 1;

    public int Level => level;

    [SerializeField] int buildPrice = 100;
    public int BuildPrice => buildPrice;

    [SerializeField] int unlockPrice = 10;
    public int UnlockPrice => unlockPrice;

    [SerializeField] string displayName;
    public string DisplayName => displayName;

    [SerializeField] string descriptionText;
    public string DescriptionText => descriptionText;

    public int kills = 0;

    [SerializeField] Sprite cardSprite;
    public Sprite CardSprite => cardSprite;

    [SerializeField] GameObject buildingPrefab;
    public GameObject BuildingPrefab => buildingPrefab;

    [SerializeField]
    CombatStats combatStats;
    public CombatStats CombatStats => combatStats;

    public BuildingConfig Clone()
    {
        BuildingConfig clone = ScriptableObject.CreateInstance<BuildingConfig>();

        clone.id = this.id;
        clone.buildingTier = this.buildingTier;
        clone.buildingType = this.buildingType;
        clone.level = this.level;
        clone.buildPrice = this.buildPrice;
        clone.unlockPrice = this.unlockPrice;
        clone.displayName = this.displayName;
        clone.descriptionText = this.descriptionText;
        clone.kills = this.kills;

        clone.cardSprite = this.cardSprite;
        clone.buildingPrefab = this.buildingPrefab;

        if (this.combatStats != null)
            clone.combatStats = this.combatStats.Clone();

        return clone;
    }

    public void Upgrade()
    {
        level += 1;
    }

#if UNITY_EDITOR
    private void RenameAssetSafe()
    {
        if (this == null) return;

        string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
        if (string.IsNullOrEmpty(assetPath)) return;

        string cleanDisplayName = displayName.Replace(" ", "");
        string newFileName = $"{cleanDisplayName}_{id}.asset";
        if (Path.GetFileName(assetPath) == newFileName) return;

        string directory = Path.GetDirectoryName(assetPath);
        string newPath = Path.Combine(directory, newFileName);

        if (File.Exists(newPath))
        {
            Debug.LogWarning($"File already exists: {newPath}");
            return;
        }

        UnityEditor.AssetDatabase.SaveAssets();

        string errorMessage = UnityEditor.AssetDatabase.RenameAsset(assetPath, Path.GetFileNameWithoutExtension(newFileName));

        if (!string.IsNullOrEmpty(errorMessage))
        {
            Debug.LogError($"Failed to rename asset: {errorMessage}");
        }
        else
        {
            Debug.Log($"Asset renamed to: {newFileName}");
        }
    }

    public void GenerateId()
    {
        id = IDGeneratorData.GenerateID("Building");
        RenameConfigAsset();
    }
    public void RenameConfigAsset()
    {
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();

        UnityEditor.EditorApplication.delayCall += () => RenameAssetSafe();
    }
#endif
}

