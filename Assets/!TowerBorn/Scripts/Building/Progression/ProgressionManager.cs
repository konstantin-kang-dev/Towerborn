using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionConfig buildingsProgressionConfig;
    public static ProgressionConfig enemiesProgressionConfig;

    string buildingsProgressionConfigPath = "Progressions/BuildingsProgression";
    string enemiesProgressionConfigPath = "Progressions/EnemiesProgression";

    private void Awake()
    {
        buildingsProgressionConfig = Resources.Load<ProgressionConfig>(buildingsProgressionConfigPath);
        enemiesProgressionConfig = Resources.Load<ProgressionConfig>(enemiesProgressionConfigPath);
    }

    public static float GetStatMultiplier(CombatStatOwnerType ownerType, CombatStatType statType, int level)
    {
        ProgressionConfig config = buildingsProgressionConfig;
        switch (ownerType)
        {
            case CombatStatOwnerType.Building:
                config = buildingsProgressionConfig;
                break;
            case CombatStatOwnerType.Enemy:
                config = enemiesProgressionConfig;
                break;
        }

        if (config == null)
        {
            Debug.LogError($"[ProgressionManager] Progression config not found for: {ownerType}");
            return 1;
        }

        ProgressionStatInfo info = config.progressionStatInfos.FirstOrDefault((x) => x.level == level);

        if (info == null) return 1;

        CombatStat stat = info.upgradesMultipliers.FirstOrDefault((x) => x.statType == statType);

        if (stat == null) return 1;

        return stat.statValue;
    }
}

[Serializable]
public class ProgressionStatInfo
{
    public int level = 1;
    public List<CombatStat> upgradesMultipliers;
}

