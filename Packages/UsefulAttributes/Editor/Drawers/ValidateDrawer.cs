using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(ValidateAttribute))]
public class ValidateDrawer : PropertyDrawer
{
    private static readonly Texture2D warningIcon = EditorGUIUtility.IconContent("console.erroricon.sml").image as Texture2D;
    private static readonly HashSet<string> logged = new();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var validateAttr = (ValidateAttribute)attribute;
        string error = Validate(property, validateAttr.validationType);

        Rect fieldRect = position;
        fieldRect.height = EditorGUIUtility.singleLineHeight;

        if (!string.IsNullOrEmpty(error))
        {
            Rect iconRect = new Rect(fieldRect.x, fieldRect.y, 16, fieldRect.height);
            GUI.Label(iconRect, new GUIContent(warningIcon));
            fieldRect.x += 18;
            fieldRect.width -= 18;

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
                    Debug.LogError($"<color=yellow>[{objectName}, ({typeName}) -> {property.name}]</color> <b><color=red>{error}</color></b>");
                }
            }
        }

        EditorGUI.PropertyField(fieldRect, property, label);

        if (!string.IsNullOrEmpty(error))
        {
            Rect errorRect = new(position.x + 18, position.y + EditorGUIUtility.singleLineHeight + 2,
                                      position.width - 18, EditorGUIUtility.singleLineHeight);
            GUIStyle errorStyle = new(EditorStyles.label);
            errorStyle.normal.textColor = Color.red;
            errorStyle.fontSize = 10;
            GUI.Label(errorRect, error, errorStyle);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var validateAttr = (ValidateAttribute)attribute;
        string error = Validate(property, validateAttr.validationType);

        float height = EditorGUIUtility.singleLineHeight;
        if (!string.IsNullOrEmpty(error))
            height += EditorGUIUtility.singleLineHeight + 2;

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

            case ValidateType.Positive:
                if ((property.propertyType == SerializedPropertyType.Integer && property.intValue <= 0) ||
                    (property.propertyType == SerializedPropertyType.Float && property.floatValue <= 0f))
                    return "Value must be positive";
                break;

            case ValidateType.Negative:
                if ((property.propertyType == SerializedPropertyType.Integer && property.intValue < 0) ||
                    (property.propertyType == SerializedPropertyType.Float && property.floatValue < 0f))
                    return "Value must be negative";
                break;
        }

        return null;
    }
}