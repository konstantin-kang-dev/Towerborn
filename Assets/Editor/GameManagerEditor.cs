using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager script = (GameManager)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Game speed 1x"))
        {
            GameManager.Instance.SetTimescale(1f);
        }
        if (GUILayout.Button("Game speed 2x"))
        {
            GameManager.Instance.SetTimescale(2f);
        }
        if (GUILayout.Button("Game speed 5x"))
        {
            GameManager.Instance.SetTimescale(5f);
        }
        if (GUILayout.Button("EndGame"))
        {
            GameManager.Instance.EndGame();
        }
    }
}
