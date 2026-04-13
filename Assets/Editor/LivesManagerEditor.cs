using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LivesManager))]
public class LivesManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LivesManager script = (LivesManager)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Set 9999 lives"))
        {
            script.SetLives(9999);
        }
    }
}