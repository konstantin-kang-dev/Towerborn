
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "TowerBorn/WaveConfig", fileName = "WaveConfig")]
public class WaveConfig : ScriptableObject
{
    public string id;
    public int waveNumber;

    public int enemiesCount;
    public float spawnDelay;
    public float spawnInterval;
    public UnitConfig enemyConfig;

    public ResourceName rewardResourceType = ResourceName.Gold;
    public int rewardAmount = 0;
    public int rewardForKill = 0;

    public int GetTotalEnemies()
    {
        return enemiesCount;
    }

    public int GetReward()
    {
        return Mathf.RoundToInt(rewardAmount);
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
