using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RichHeaderAttribute))]
public class RichHeaderDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        RichHeaderAttribute header = (RichHeaderAttribute)attribute;
        GUIStyle style = new(EditorStyles.boldLabel);
        style.richText = true;
        EditorGUI.LabelField(position, header.text, style);
    }

    public override float GetHeight()
    {
        return EditorGUIUtility.singleLineHeight + 2;
    }
}