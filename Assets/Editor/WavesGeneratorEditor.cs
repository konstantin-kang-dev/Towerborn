
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WavesGenerator))]
public class WavesGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WavesGenerator script = (WavesGenerator)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Start Waves Generation Test (50)"))
        {
            script.TestGeneration(50);
        }
        GUILayout.Space(10);

        if (GUILayout.Button("Start Waves Generation Test (100)"))
        {
            script.TestGeneration(100);
        }

    }
}