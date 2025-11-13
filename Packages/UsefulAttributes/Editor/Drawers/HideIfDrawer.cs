#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections;

[CustomPropertyDrawer(typeof(HideIfAttribute))]
public class HideIfDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (ShouldHide(property))
            return 0f;

        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldHide(property))
            return;

        EditorGUI.PropertyField(position, property, label, true);
    }

    private bool ShouldHide(SerializedProperty property)
    {
        var hideIf = (HideIfAttribute)attribute;
        object target = GetTargetObject(property);

        if (target == null)
            return false;

        var type = target.GetType();
        var field = type.GetField(hideIf.condition, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var prop = type.GetProperty(hideIf.condition, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        bool conditionValue = false;

        if (field != null && field.FieldType == typeof(bool))
            conditionValue = (bool)field.GetValue(target);
        else if (prop != null && prop.PropertyType == typeof(bool))
            conditionValue = (bool)prop.GetValue(target);
        else
        {
            Debug.LogWarning($"[HideIf] No bool field or property named '{hideIf.condition}' found on {target}");
        }

        return conditionValue == hideIf.hideIfTrue;
    }

    private object GetTargetObject(SerializedProperty property)
    {
        if (property.propertyType == SerializedPropertyType.ManagedReference)
            return property.managedReferenceValue;

        object obj = property.serializedObject.targetObject;

        string normalizedPath = property.propertyPath.Replace(".Array.data[", "[");

        string[] parts = normalizedPath.Split('.');

        if (parts.Length == 1 && parts[0].Contains("["))
        {
            string part = parts[0];
            int bracket = part.IndexOf('[');
            if (bracket >= 0)
            {
                string listName = part.Substring(0, bracket);
                int index = int.Parse(part.Substring(bracket + 1, part.Length - bracket - 2));

                var listField = obj.GetType().GetField(listName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (listField == null) return null;

                if (listField.GetValue(obj) is IList list)
                {
                    if (index < 0 || index >= list.Count) return null;
                    return list[index];
                }

                return null;
            }
        }

        int stopIndex = parts.Length - 1;
        for (int i = 0; i < stopIndex; i++)
        {
            string part = parts[i];

            if (part.Contains("["))
            {
                int bracket = part.IndexOf('[');
                string listName = part.Substring(0, bracket);
                int index = int.Parse(part.Substring(bracket + 1, part.Length - bracket - 2));

                var listField = obj.GetType().GetField(listName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (listField == null) return null;

                if (listField.GetValue(obj) is IList list)
                {
                    if (index < 0 || index >= list.Count) return null;
                    obj = list[index];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var f = obj.GetType().GetField(part, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (f == null) return null;
                obj = f.GetValue(obj);
            }

            if (obj == null) return null;
        }

        return obj;
    }

}
#endif