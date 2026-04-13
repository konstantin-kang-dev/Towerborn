
using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingConfig))]
public class BuildingConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BuildingConfig script = (BuildingConfig)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate ID", GUILayout.Height(30)))
        {
            script.GenerateId();
        }

        EditorGUILayout.HelpBox($"Current ID: {script.Id}", MessageType.Info);

        GUILayout.Space(10);

        if (GUILayout.Button("Rename Asset", GUILayout.Height(30)))
        {
            script.RenameConfigAsset();
        }

        EditorGUILayout.HelpBox($"Asset name formula: [displayName_id.asset]", MessageType.Info);
    }
}