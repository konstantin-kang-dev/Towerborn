using UnityEngine;

public class ShowIfEnumAttribute : PropertyAttribute
{
    public string enumFieldName;
    public int enumValue;

    public ShowIfEnumAttribute(string enumFieldName, int enumValue)
    {
        this.enumFieldName = enumFieldName;
        this.enumValue = enumValue;
    }
}
