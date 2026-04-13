using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameGrid))]
public class GameGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameGrid script = (GameGrid)target;

        GUILayout.Space(10);

        if (GUILayout.Button("ChangeGridSize 6"))
        {
            script.ChangeGridSize(6);
        }

        if (GUILayout.Button("ChangeGridSize 10"))
        {
            script.ChangeGridSize(10);
        }
    }
}
