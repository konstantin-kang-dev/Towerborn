using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(WavesManagerTester))]
public class WavesManagerTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WavesManagerTester script = (WavesManagerTester)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Skip 1 Wave"))
        {
            script.SkipWaves(1);
        }

        GUILayout.Space(10);
        
        if (GUILayout.Button("Skip 5 Waves"))
        {
            script.SkipWaves(5);
        }

        GUILayout.Space(10);
        
        if (GUILayout.Button("Skip 10 Waves"))
        {
            script.SkipWaves(10);
        }


        
        GUILayout.Space(30);
        
        if (GUILayout.Button("Set 5 Wave"))
        {
            script.SetWave(5);
        }

        GUILayout.Space(10);
        
        if (GUILayout.Button("Set 10 Wave"))
        {
            script.SetWave(10);
        }

        GUILayout.Space(10);
        
        if (GUILayout.Button("Set 50 Wave"))
        {
            script.SetWave(50);
        }

        GUILayout.Space(10);
        
        if (GUILayout.Button("Set 100 Wave"))
        {
            script.SetWave(100);
        }

        GUILayout.Space(10);
        
        if (GUILayout.Button("Set 200 Wave"))
        {
            script.SetWave(200);
        }

    }
}

