
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResourcesTester))]
public class ResourcesTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ResourcesTester script = (ResourcesTester)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Add Gold 100"))
        {
            script.AddResource(ResourceName.Gold, 100);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Add Gold 1000"))
        {
            script.AddResource(ResourceName.Gold, 1000);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Add Gems 100"))
        {
            script.AddResource(ResourceName.Gems, 100);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Add Gems 1000"))
        {
            script.AddResource(ResourceName.Gems, 1000);
        }
    }
}