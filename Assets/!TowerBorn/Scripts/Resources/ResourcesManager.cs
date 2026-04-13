using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerBorn.SaveSystem;

[Serializable]
public enum ResourceName
{
    Gold, Gems
}
[Serializable]
public class Resource
{
    public ResourceName resourceName;
    public int amount = 0;
}
public class ResourcesManager
{

    static Dictionary<ResourceName, Resource> resources = new Dictionary<ResourceName, Resource>() {
        {ResourceName.Gold, new Resource() { resourceName = ResourceName.Gold} },
        {ResourceName.Gems, new Resource() { resourceName = ResourceName.Gems} },
    };

    public static event Action<Resource> OnResourceUpdated;
    public static event Action<Resource> OnResourceAdded;
    public static event Action<Resource> OnResourceSpend;

    public static bool CheckResourceForEnough(ResourceName resourceName, int required)
    {
        return resources[resourceName].amount >= required;
    }
    public static void AddResource(ResourceName resourceName, int quantity)
    {
        //if (GameManager.Instance.GameState != GameState.Playing) return;

        //Debug.Log($"Added resource: {resourceName} {quantity}");
        resources[resourceName].amount += quantity;
        SavesManager.ResourcesSave.UpdateResourceSave(resourceName, resources[resourceName].amount);

        OnResourceAdded?.Invoke(resources[resourceName]);
        OnResourceUpdated?.Invoke(resources[resourceName]);
    }
    public static void SpendResource(ResourceName resourceName, int quantity)
    {
        if (GameManager.Instance.GameState != GameState.Playing) return;

        Debug.Log($"Spend resource: {resourceName} {quantity}");
        resources[resourceName].amount -= quantity;
        SavesManager.ResourcesSave.UpdateResourceSave(resourceName, resources[resourceName].amount);

        OnResourceSpend?.Invoke(resources[resourceName]);
        OnResourceUpdated?.Invoke(resources[resourceName]);
    }

    public static void SetResource(ResourceName resourceName, int quantity)
    {
        resources[resourceName].amount = quantity;

        OnResourceUpdated?.Invoke(resources[resourceName]);
    }

    public static Resource GetResource(ResourceName resourceName)
    {
        return resources[resourceName];
    }

    public static void ApplySave()
    {
        foreach (KeyValuePair<ResourceName, int> resource in SavesManager.ResourcesSave.resourcesData)
        {
            switch (resource.Key)
            {
                case ResourceName.Gold:
                    break;
                case ResourceName.Gems:
                    SetResource(resource.Key, resource.Value);
                    break;
                default:
                    break;
            }
        }

        if (SavesManager.ProgressSave.isFirstLaunch)
        {
            AddResource(ResourceName.Gems, 0);
        }
    }
}