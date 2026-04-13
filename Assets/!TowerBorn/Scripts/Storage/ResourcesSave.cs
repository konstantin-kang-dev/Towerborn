using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ResourcesSave
{
    public Dictionary<ResourceName, int> resourcesData = new Dictionary<ResourceName, int>();

    public ResourcesSave() { }
    public void UpdateResourceSave(ResourceName resourceName, int value)
    {
        if (resourcesData.ContainsKey(resourceName))
        {
            resourcesData[resourceName] = value;
        }
        else
        {
            resourcesData.Add(resourceName, value);
        }
    }
}