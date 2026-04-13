using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameIDSystem
{
    [CreateAssetMenu(fileName = "IDGeneratorData", menuName = "Game/ID Generator Data")]
    public class IDGeneratorData : ScriptableObject
    {
        public static string configPath = "Assets/Resources/IDGenerator/";

        [System.Serializable]
        public class CategoryCounter
        {
            public string category;
            public int currentCount;

            public CategoryCounter(string cat)
            {
                category = cat;
                currentCount = 0;
            }
        }

        [SerializeField] private List<CategoryCounter> counters = new List<CategoryCounter>();

        private static IDGeneratorData instance;

        private static IDGeneratorData Instance
        {
            get
            {
                if (instance == null)
                {
#if UNITY_EDITOR
                    string[] guids = AssetDatabase.FindAssets("t:IDGeneratorData");
                    if (guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        instance = AssetDatabase.LoadAssetAtPath<IDGeneratorData>(path);
                    }

                    if (instance == null)
                    {
                        instance = CreateInstance<IDGeneratorData>();
                        AssetDatabase.CreateAsset(instance, configPath + "IDGeneratorData.asset");
                        AssetDatabase.SaveAssets();
                    }
#endif
                }
                return instance;
            }
        }

        public static string GenerateID(string category)
        {
            return Instance.GenerateNewID(category);
        }

        private string GenerateNewID(string category)
        {
            CategoryCounter counter = counters.Find(c => c.category == category);

            if (counter == null)
            {
                counter = new CategoryCounter(category);
                counters.Add(counter);
            }

            counter.currentCount++;

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif

            return $"{category}_{counter.currentCount:D5}";
        }

        public static int GetCount(string category)
        {
            var counter = Instance.counters.Find(c => c.category == category);
            return counter?.currentCount ?? 0;
        }
    }

#if UNITY_EDITOR

    public class IDGeneratorWindow : EditorWindow
    {
        private string category = "item";
        private string lastGeneratedID = "";

        [MenuItem("Tools/ID Generator")]
        public static void ShowWindow()
        {
            GetWindow<IDGeneratorWindow>("ID Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("ID Generator", EditorStyles.boldLabel);
            GUILayout.Space(10);

            category = EditorGUILayout.TextField("Category:", category);

            if (GUILayout.Button("Generate ID", GUILayout.Height(30)))
            {
                if (!string.IsNullOrEmpty(category))
                {
                    lastGeneratedID = IDGeneratorData.GenerateID(category);
                    EditorGUIUtility.systemCopyBuffer = lastGeneratedID;
                    Debug.Log($"Generated: {lastGeneratedID} (copied to clipboard)");
                }
            }

            if (!string.IsNullOrEmpty(lastGeneratedID))
            {
                GUILayout.Space(10);
                EditorGUILayout.HelpBox($"Generated: {lastGeneratedID}", MessageType.Info);
            }

            GUILayout.Space(20);
            GUILayout.Label("Current Counters:", EditorStyles.boldLabel);

            var data = AssetDatabase.LoadAssetAtPath<IDGeneratorData>(IDGeneratorData.configPath + "IDGeneratorData.asset");
            if (data != null)
            {
                GUI.enabled = false;
                SerializedObject so = new SerializedObject(data);
                EditorGUILayout.PropertyField(so.FindProperty("counters"), true);
                GUI.enabled = true;
            }
        }
    }
#endif
}