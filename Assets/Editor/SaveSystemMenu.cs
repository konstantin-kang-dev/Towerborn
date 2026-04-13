// SaveSettingsEditor.cs
// Поместите этот файл в папку Editor
using UnityEditor;
using UnityEngine;

public class SaveSettingsMenu : EditorWindow
{
    private SaveSettings _settings;

    [MenuItem("Tools/Save System/Settings")]
    public static void ShowWindow()
    {
        GetWindow<SaveSettingsMenu>("Save System Settings");
    }

    [MenuItem("Tools/Save System/Toggle Save System", false, 0)]
    public static void ToggleSaveSystem()
    {
        SaveSettings settings = LoadSettings();
        if (settings != null)
        {
            SerializedObject serializedObject = new SerializedObject(settings);
            SerializedProperty enabledProperty = serializedObject.FindProperty("saveSystemEnabled");
            enabledProperty.boolValue = !enabledProperty.boolValue;
            serializedObject.ApplyModifiedProperties();

            Debug.Log("Save System is now " + (enabledProperty.boolValue ? "ENABLED" : "DISABLED"));
        }
    }

    [MenuItem("Tools/Save System/Toggle Save System", true)]
    private static bool ToggleSaveSystemValidate()
    {
        SaveSettings settings = LoadSettings();
        if (settings != null)
        {
            Menu.SetChecked("Tools/Save System/Toggle Save System", settings.SaveSystemEnabled);
        }
        return true;
    }

    private static SaveSettings LoadSettings()
    {
        string[] guids = AssetDatabase.FindAssets("t:SaveSettings");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<SaveSettings>(path);
        }

        SaveSettings settings = ScriptableObject.CreateInstance<SaveSettings>();
        AssetDatabase.CreateAsset(settings, "Assets/Resources/SaveSettings.asset");
        AssetDatabase.SaveAssets();
        return settings;
    }

    private void OnGUI()
    {
        if (_settings == null)
        {
            _settings = LoadSettings();
        }

        EditorGUI.BeginChangeCheck();

        SerializedObject serializedObject = new SerializedObject(_settings);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("saveSystemEnabled"), new GUIContent("Enable Save System"));

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}