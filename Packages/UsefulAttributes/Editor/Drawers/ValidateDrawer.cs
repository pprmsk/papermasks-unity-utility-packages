using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace PAPERMASK.Utilities
{
    [CustomPropertyDrawer(typeof(ValidateAttribute))]
    public class ValidateDrawer : PropertyDrawer
    {
        private static readonly Texture2D warningIcon = EditorGUIUtility.IconContent("console.erroricon.sml").image as Texture2D;
        private static readonly HashSet<string> logged = new();

        private const int PADDING = 2;
        private const int ICON_SIZE = 16;
        private const int ERROR_FONT_SIZE = 10;
        private const int FIELD_OFFSET = 18;

        private static readonly string logColorYellow = CLib.Yellow;
        private static readonly string logColorRed = CLib.Red;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var validateAttr = (ValidateAttribute)attribute;
            string error = Validate(property, validateAttr.validationType);

            Rect fieldRect = position;
            fieldRect.height = EditorGUIUtility.singleLineHeight;

            if (!string.IsNullOrEmpty(error))
            {
                Rect iconRect = new(fieldRect.x, fieldRect.y, ICON_SIZE, fieldRect.height);
                GUI.Label(iconRect, new GUIContent(warningIcon));
                fieldRect.x += FIELD_OFFSET;
                fieldRect.width -= FIELD_OFFSET;

                if (Application.isPlaying)
                {
                    Object targetObject = property.serializedObject.targetObject;

                    string objectName = targetObject.name;
                    string typeName = targetObject.GetType().Name;

                    if (targetObject is MonoBehaviour mb)
                    {
                        objectName = mb.gameObject.name;
                        typeName = "GameObject";
                    }

                    string key = targetObject.GetInstanceID() + "_" + property.propertyPath;
                    if (!logged.Contains(key))
                    {
                        logged.Add(key);
                        Debug.LogError($"<color={logColorYellow}>[{objectName}, ({typeName}) -> {property.name}]</color> <b><color={logColorRed}>{error}</color></b>");
                    }
                }
            }

            EditorGUI.PropertyField(fieldRect, property, label);

            if (!string.IsNullOrEmpty(error))
            {
                Rect errorRect = new(position.x + FIELD_OFFSET, position.y + EditorGUIUtility.singleLineHeight + PADDING,
                                     position.width - FIELD_OFFSET, EditorGUIUtility.singleLineHeight);
                GUIStyle errorStyle = new(EditorStyles.label);
                errorStyle.normal.textColor = Color.red;
                errorStyle.fontSize = ERROR_FONT_SIZE;
                GUI.Label(errorRect, error, errorStyle);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var validateAttr = (ValidateAttribute)attribute;
            string error = Validate(property, validateAttr.validationType);

            float height = EditorGUIUtility.singleLineHeight;
            if (!string.IsNullOrEmpty(error))
                height += EditorGUIUtility.singleLineHeight + PADDING;

            return height;
        }

        private string Validate(SerializedProperty property, ValidateType rule)
        {
            switch (rule)
            {
                case ValidateType.NotNull:
                    if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
                        return "Field cannot be null";
                    break;

                case ValidateType.NonEmpty:
                    if (property.propertyType == SerializedPropertyType.String && string.IsNullOrWhiteSpace(property.stringValue))
                        return "String cannot be empty";
                    break;

                case ValidateType.IsPositive:
                    if ((property.propertyType == SerializedPropertyType.Integer && property.intValue <= 0) ||
                        (property.propertyType == SerializedPropertyType.Float && property.floatValue <= 0f))
                        return "Value must be positive";
                    break;

                case ValidateType.IsNegative:
                    if ((property.propertyType == SerializedPropertyType.Integer && property.intValue < 0) ||
                        (property.propertyType == SerializedPropertyType.Float && property.floatValue < 0f))
                        return "Value must be negative";
                    break;
            }

            return null;
        }
    }
}