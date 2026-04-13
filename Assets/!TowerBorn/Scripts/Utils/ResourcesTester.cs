
using UnityEngine;

public class ResourcesTester : MonoBehaviour
{
    public void AddResource(ResourceName name, int value)
    {
        ResourcesManager.AddResource(name, value);
    }
}
