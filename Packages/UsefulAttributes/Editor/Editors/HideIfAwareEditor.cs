#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PAPERMASK.Utilities
{
    [CustomEditor(typeof(MonoBehaviour), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class HideIfAwareEditorMono : HideIfAwareEditor { }

    [CustomEditor(typeof(ScriptableObject), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class HideIfAwareEditorSO : HideIfAwareEditor { }

    public class HideIfAwareEditor : Editor
    {
        private readonly Dictionary<string, ReorderableList> _lists = new();

        private void OnEnable()
        {
            var type = serializedObject.targetObject.GetType();

            while (type != null && type != typeof(MonoBehaviour) && type != typeof(ScriptableObject))
            {
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (!typeof(IList).IsAssignableFrom(field.FieldType)) { continue; }

                    var elementType = GetListElementType(field.FieldType);
                    if (elementType == null) { continue; }

                    bool hasHideIf = elementType
                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Any(f => f.GetCustomAttribute<HideIfAttribute>() != null);

                    if (!hasHideIf) { continue; }

                    var listProp = serializedObject.FindProperty(field.Name);
                    if (listProp == null) { continue; }

                    if (!_lists.ContainsKey(field.Name))
                        _lists[field.Name] = BuildList(listProp);
                }

                type = type.BaseType;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));

            var iterator = serializedObject.GetIterator();
            iterator.NextVisible(true);

            while (iterator.NextVisible(false))
            {
                if (_lists.TryGetValue(iterator.name, out var list))
                {
                    list.DoLayoutList();
                    continue;
                }

                if (ShouldHideProperty(iterator)) { continue; }

                EditorGUILayout.PropertyField(iterator, includeChildren: true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool ShouldHideProperty(SerializedProperty property)
        {
            var targetObj = serializedObject.targetObject;

            FieldInfo field = null;
            var type = targetObj.GetType();
            while (type != null && field == null)
            {
                field = type.GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                type = type.BaseType;
            }

            if (field == null) { return false; }

            var hideIf = field.GetCustomAttribute<HideIfAttribute>();
            if (hideIf == null) { return false; }

            return EvaluateHideIf(hideIf, targetObj);
        }

        private bool ShouldHideElement(SerializedProperty element)
        {
            object target = element.managedReferenceValue ?? GetTargetFromPath(element);
            if (target == null) { return false; }

            foreach (var field in target.GetType()
                         .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var hideIf = field.GetCustomAttribute<HideIfAttribute>();
                if (hideIf == null) { continue; }

                if (EvaluateHideIf(hideIf, target)) { return true; }
            }

            return false;
        }

        private static bool EvaluateHideIf(HideIfAttribute hideIf, object target)
        {
            var conditionField = target.GetType()
                .GetField(hideIf.condition, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (conditionField == null) { return false; }

            object valueObj = conditionField.GetValue(target);
            bool conditionValue = false;

            if (conditionField.FieldType == typeof(bool))
            {
                conditionValue = (bool)valueObj;
            }
            else if (hideIf.hasCompareValue)
            {
                if (valueObj is IList list)
                {
                    conditionValue = hideIf.compareValue is int intVal
                        ? list.Count == intVal
                        : list.Count > 0;
                }
                else
                {
                    conditionValue = valueObj != null && valueObj.Equals(hideIf.compareValue);
                }
            }

            return conditionValue == hideIf.hideIfTrue;
        }

        private ReorderableList BuildList(SerializedProperty listProp)
        {
            string propPath = listProp.propertyPath;

            var list = new ReorderableList(
                serializedObject, listProp,
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true
            );

            list.drawHeaderCallback = rect =>
                EditorGUI.LabelField(rect, listProp.displayName);

            list.elementHeightCallback = index =>
            {
                var prop = serializedObject.FindProperty(propPath);
                var element = prop.GetArrayElementAtIndex(index);
                if (ShouldHideElement(element)) { return 0f; }
                return EditorGUI.GetPropertyHeight(element, true) + 2f;
            };

            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var prop = serializedObject.FindProperty(propPath);
                var element = prop.GetArrayElementAtIndex(index);
                if (ShouldHideElement(element)) { return; }
                rect.y += 1f;
                rect.height = EditorGUI.GetPropertyHeight(element, true);
                EditorGUI.PropertyField(rect, element, true);
            };

            return list;
        }

        private static object GetTargetFromPath(SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace(".Array.data[", "[");

            foreach (var part in path.Split('.'))
            {
                if (obj == null) { return null; }

                if (part.Contains("["))
                {
                    int bracket = part.IndexOf('[');
                    string name = part[..bracket];
                    int index = int.Parse(part[(bracket + 1)..^1]);

                    var f = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (f?.GetValue(obj) is IList list)
                    {
                        obj = index < list.Count ? list[index] : null;
                    }
                    else { return null; }
                }
                else
                {
                    var f = obj.GetType().GetField(part,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    obj = f?.GetValue(obj);
                }
            }

            return obj;
        }

        private static Type GetListElementType(Type listType)
        {
            return listType switch
            {
                Type t when t.IsArray => t.GetElementType(),
                Type t when t.IsGenericType => t.GetGenericArguments().FirstOrDefault(),
                _ => null
            };
        }
    }
}
#endif