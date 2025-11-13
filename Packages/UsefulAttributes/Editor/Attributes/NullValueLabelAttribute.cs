using UnityEngine;

public class NullValueLabelAttribute : PropertyAttribute
{
    public readonly string label;

    public NullValueLabelAttribute(string label = "Default")
    {
        this.label = label;
    }
}