// Скрипт для автоматического создания спрайтов цифр
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

public class FontToSpriteGenerator : EditorWindow
{
    public Font font;
    public int fontSize = 64;
    public Color textColor = Color.white;
    public string savePath = "Assets/Sprites/Numbers/";

    [MenuItem("Tools/Generate Number Sprites")]
    static void ShowWindow()
    {
        GetWindow<FontToSpriteGenerator>("Font to Sprite");
    }

    void OnGUI()
    {
        font = (Font)EditorGUILayout.ObjectField("Font", font, typeof(Font), false);
        fontSize = EditorGUILayout.IntField("Font Size", fontSize);
        textColor = EditorGUILayout.ColorField("Text Color", textColor);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Generate Sprites"))
        {
            GenerateNumberSprites();
        }
    }

    void GenerateNumberSprites()
    {
        if (!System.IO.Directory.Exists(savePath))
            System.IO.Directory.CreateDirectory(savePath);

        for (int i = 0; i <= 9; i++)
        {
            CreateSpriteForDigit(i);
        }

        AssetDatabase.Refresh();
        Debug.Log("Спрайты цифр созданы!");
    }

    void CreateSpriteForDigit(int digit)
    {
        GameObject tempObj = new GameObject("TempDigit");
        Canvas canvas = tempObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        GameObject textObj = new GameObject("Digit");
        textObj.transform.SetParent(tempObj.transform);

        TextMeshProUGUI textMesh = textObj.AddComponent<TextMeshProUGUI>();
        textMesh.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        textMesh.text = digit.ToString();
        textMesh.fontSize = fontSize;
        textMesh.color = textColor;
        textMesh.alignment = TextAlignmentOptions.Center;

        RenderTexture renderTexture = new RenderTexture(128, 128, 24);
        Camera renderCamera = new GameObject("RenderCamera").AddComponent<Camera>();
        renderCamera.targetTexture = renderTexture;
        renderCamera.backgroundColor = Color.clear;
        renderCamera.clearFlags = CameraClearFlags.SolidColor;

        renderCamera.transform.position = new Vector3(0, 0, -10);
        renderCamera.orthographic = true;
        renderCamera.orthographicSize = 2;

        renderCamera.Render();

        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(128, 128, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, 128, 128), 0, 0);
        texture.Apply();

        byte[] pngData = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(savePath + $"digit_{digit}.png", pngData);

        RenderTexture.active = null;
        DestroyImmediate(tempObj);
        DestroyImmediate(renderCamera.gameObject);
        DestroyImmediate(renderTexture);
        DestroyImmediate(texture);
    }
}
#endif