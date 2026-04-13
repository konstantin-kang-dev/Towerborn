using UnityEngine;

public class ShowIfBoolAttribute : PropertyAttribute
{
    public string boolFieldName;
    public bool expectedValue;

    public ShowIfBoolAttribute(string boolFieldName, bool expectedValue = true)
    {
        this.boolFieldName = boolFieldName;
        this.expectedValue = expectedValue;
    }
}
