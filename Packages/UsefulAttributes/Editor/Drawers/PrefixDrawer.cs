using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PrefixAttribute))]
internal class PrefixDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (PrefixAttribute)attribute;

        var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
        var fieldRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height);

        string prefix = "[" + attr.prefix + "]";
        string main = " " + label.text;

        float prefixWidth = GUI.skin.label.CalcSize(new GUIContent(prefix)).x;

        var prefixRect = new Rect(labelRect.x, labelRect.y, prefixWidth, labelRect.height);
        var mainRect = new Rect(labelRect.x + prefixWidth, labelRect.y, labelRect.width - prefixWidth, labelRect.height);

        var oldColor = GUI.color;
        GUI.color = attr.color;
        EditorGUI.LabelField(prefixRect, prefix);
        GUI.color = oldColor;

        EditorGUI.LabelField(mainRect, main);

        EditorGUI.PropertyField(fieldRect, property, GUIContent.none, true);
    }
}