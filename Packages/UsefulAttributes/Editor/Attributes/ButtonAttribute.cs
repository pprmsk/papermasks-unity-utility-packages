using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Method)]
public class ButtonAttribute : PropertyAttribute
{
    public string label;
    public string group;
    public string foldout;

    public ButtonAttribute(string methodName = null, string group = null, string foldout = null)
    {
        label = methodName;
        this.group = group;
        this.foldout = foldout;
    }
}