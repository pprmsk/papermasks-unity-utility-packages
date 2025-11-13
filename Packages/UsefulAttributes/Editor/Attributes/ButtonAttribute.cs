using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Method)]
public class ButtonAttribute : PropertyAttribute
{
    public string Label;
    public string Group;
    public string Foldout;

    public ButtonAttribute(string methodName = null, string group = null, string foldout = null)
    {
        Label = methodName;
        Group = group;
        Foldout = foldout;
    }
}