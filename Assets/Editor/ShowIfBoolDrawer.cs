using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ShowIfBoolAttribute))]
public class ShowIfBoolDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfBoolAttribute showIfBool = (ShowIfBoolAttribute)attribute;
        SerializedProperty boolField = FindBoolProperty(property, showIfBool.boolFieldName);

        if (boolField != null && boolField.boolValue == showIfBool.expectedValue)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfBoolAttribute showIfBool = (ShowIfBoolAttribute)attribute;
        SerializedProperty boolField = FindBoolProperty(property, showIfBool.boolFieldName);

        if (boolField != null && boolField.boolValue == showIfBool.expectedValue)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return 0;
        }
    }

    private SerializedProperty FindBoolProperty(SerializedProperty property, string boolFieldName)
    {
        SerializedProperty boolField = property.serializedObject.FindProperty(boolFieldName);
        if (boolField != null && boolField.propertyType == SerializedPropertyType.Boolean)
            return boolField;

        SerializedProperty parent = property.serializedObject.FindProperty(property.propertyPath);
        while (parent != null)
        {
            boolField = parent.FindPropertyRelative(boolFieldName);
            if (boolField != null && boolField.propertyType == SerializedPropertyType.Boolean)
                return boolField;

            parent = GetParentProperty(parent);
        }

        return null;
    }

    private SerializedProperty GetParentProperty(SerializedProperty property)
    {
        string path = property.propertyPath;
        int lastDot = path.LastIndexOf('.');
        if (lastDot == -1) return null;

        string parentPath = path.Substring(0, lastDot);
        return property.serializedObject.FindProperty(parentPath);
    }
}
