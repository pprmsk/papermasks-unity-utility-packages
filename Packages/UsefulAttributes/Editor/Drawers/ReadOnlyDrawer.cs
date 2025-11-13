using UnityEditor;
using UnityEngine;

namespace PAPERMASK.Utilities
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    internal class ReadOnlyDrawer : PropertyDrawer
    {
        Color readOnlyColor = ColorUtility.TryParseHtmlString(CLib.LightPink, out var color) ? color : Color.magenta;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Color originalColor = GUI.color;
            Color originalLabelColor = GUI.contentColor;

            GUI.backgroundColor = readOnlyColor;
            GUI.contentColor = readOnlyColor;

            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;

            GUI.backgroundColor = originalColor;
            GUI.contentColor = originalLabelColor;
        }
    }
}