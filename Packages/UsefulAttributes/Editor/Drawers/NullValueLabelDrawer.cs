using UnityEditor;
using UnityEngine;

namespace PAPERMASK.Utilities
{
    [CustomPropertyDrawer(typeof(NullValueLabelAttribute))]
    public class NullValueLabelDrawer : PropertyDrawer
    {
        private readonly string nullColor = CLib.DarkGray;
        private const int PADDING = 2;
        private const int WIDTH_OFFSET = 20;

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
                GUIStyle style = new GUIStyle(EditorStyles.objectField);
                style.richText = true;

                property.objectReferenceValue = EditorGUI.ObjectField(position, label, null, fieldType, true);

                Rect textRect = position;
                textRect.x += EditorGUIUtility.labelWidth + PADDING;
                textRect.width -= EditorGUIUtility.labelWidth + WIDTH_OFFSET;
                GUI.Label(textRect, fieldLabel, style);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

            EditorGUI.EndProperty();
        }
    }
}