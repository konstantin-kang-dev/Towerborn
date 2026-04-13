using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ShowIfEnumAttribute))]
public class ShowIfEnumDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfEnumAttribute showIfEnum = (ShowIfEnumAttribute)attribute;
        SerializedProperty enumField = FindEnumProperty(property, showIfEnum.enumFieldName);

        if (enumField != null && enumField.enumValueIndex == showIfEnum.enumValue)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfEnumAttribute showIfEnum = (ShowIfEnumAttribute)attribute;
        SerializedProperty enumField = FindEnumProperty(property, showIfEnum.enumFieldName);

        if (enumField != null && enumField.enumValueIndex == showIfEnum.enumValue)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return 0;
        }
    }

    private SerializedProperty FindEnumProperty(SerializedProperty property, string enumFieldName)
    {
        SerializedProperty enumField = property.serializedObject.FindProperty(enumFieldName);
        if (enumField != null && enumField.propertyType == SerializedPropertyType.Enum)
            return enumField;

        SerializedProperty parent = property.serializedObject.FindProperty(property.propertyPath);
        while (parent != null)
        {
            enumField = parent.FindPropertyRelative(enumFieldName);
            if (enumField != null && enumField.propertyType == SerializedPropertyType.Enum)
                return enumField;

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
