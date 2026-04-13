using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TierWeight
{
    public BuildingTier tier;
    [Range(0f, 100f)]
    public float weight = 10f;

    public TierWeight(BuildingTier tier, float weight)
    {
        this.tier = tier;
        this.weight = weight;
    }
}

public class DeckGenerator
{
    [Header("Настройки шансов выпадения")]
    [SerializeField]
    private static List<TierWeight> tierWeights = new List<TierWeight>()
    {
        new TierWeight(BuildingTier.Tier1, 70f),
        new TierWeight(BuildingTier.Tier2, 25f),
        new TierWeight(BuildingTier.Tier3, 10f),
        new TierWeight(BuildingTier.Tier4, 5f),
        new TierWeight(BuildingTier.Tier5, 2f),
    };

    public static BuildingTier GetRandomTier()
    {
        if (tierWeights.Count == 0)
        {
            Debug.LogWarning("TierWeights пуст! Возвращаем Tier1");
            return BuildingTier.Tier1;
        }

        float totalWeight = CalculateTotalWeight();
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var tierWeight in tierWeights)
        {
            currentWeight += tierWeight.weight;
            if (randomValue <= currentWeight)
            {
                return tierWeight.tier;
            }
        }

        return tierWeights[0].tier;
    }

    private static float CalculateTotalWeight()
    {
        float total = 0f;
        foreach (var tierWeight in tierWeights)
        {
            total += tierWeight.weight;
        }
        return total;
    }

    public static float GetTierChancePercentage(BuildingTier tier)
    {
        float totalWeight = CalculateTotalWeight();

        foreach (var tierWeight in tierWeights)
        {
            if (tierWeight.tier == tier)
            {
                return (tierWeight.weight / totalWeight) * 100f;
            }
        }

        return 0f;
    }

    public void SetTierWeights(List<TierWeight> newWeights)
    {
        tierWeights = new List<TierWeight>(newWeights);
        LogCurrentChances();
    }

    public void UpdateTierWeight(BuildingTier tier, float newWeight)
    {
        foreach (var tierWeight in tierWeights)
        {
            if (tierWeight.tier == tier)
            {
                tierWeight.weight = Mathf.Max(0f, newWeight);
                LogCurrentChances();
                return;
            }
        }

        tierWeights.Add(new TierWeight(tier, Mathf.Max(0f, newWeight)));
        LogCurrentChances();
    }

    [ContextMenu("Показать текущие шансы")]
    public void LogCurrentChances()
    {
        float totalWeight = CalculateTotalWeight();
        Debug.Log("=== Текущие шансы выпадения ===");

        foreach (var tierWeight in tierWeights)
        {
            float percentage = (tierWeight.weight / totalWeight) * 100f;
            Debug.Log($"{tierWeight.tier}: {percentage:F1}% (вес: {tierWeight.weight})");
        }

        Debug.Log($"Общий вес: {totalWeight}");
    }
}