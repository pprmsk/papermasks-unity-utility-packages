using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NullValueLabelAttribute))]
public class NullValueLabelDrawer : PropertyDrawer
{
    string nullColor = CLib.DarkGray;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (NullValueLabelAttribute)attribute;

        if (property.propertyType != SerializedPropertyType.ObjectReference)
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        Object assigned = property.objectReferenceValue;
        var fieldType = fieldInfo.FieldType;

        if (assigned == null)
        {
            GUIContent fieldLabel = new GUIContent($"<color={nullColor}>NULL = {attr.label}</color>");
            GUIStyle style = new GUIStyle(EditorStyles.objectField)
            {
                richText = true
            };

            property.objectReferenceValue = EditorGUI.ObjectField(position, label, null, fieldType, true);

            Rect textRect = position;
            textRect.x += EditorGUIUtility.labelWidth + 2;
            textRect.width -= EditorGUIUtility.labelWidth + 20;
            GUI.Label(textRect, fieldLabel, style);
        }
        else
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        EditorGUI.EndProperty();
    }
}