#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(UnityEngine.Object), true)]
[CanEditMultipleObjects]
public class ButtonAttributeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        foreach (var t in targets)
        {
            if (t == null) continue;

            MethodInfo[] methods = t.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            Dictionary<string, List<MethodInfo>> groupedMethods = new Dictionary<string, List<MethodInfo>>();
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<ButtonAttribute>();
                if (attr != null)
                {
                    string group = attr.Group ?? "__default__";
                    if (!groupedMethods.ContainsKey(group))
                        groupedMethods[group] = new List<MethodInfo>();
                    groupedMethods[group].Add(method);
                }
            }

            foreach (var group in groupedMethods.Values)
            {
                bool useHorizontal = group.Count > 1;
                if (group.Count > 1) GUILayout.BeginHorizontal();
                foreach (var method in group)
                {
                    var attr = method.GetCustomAttribute<ButtonAttribute>();
                    string label = string.IsNullOrEmpty(attr.Label) ? method.Name : attr.Label;
                    if (GUILayout.Button(label, GUILayout.Height(30)))
                    {
                        method.Invoke(t, null);
                        EditorUtility.SetDirty(t);
                    }

                    if (useHorizontal) GUILayout.Space(5);
                }
                if (group.Count > 1) GUILayout.EndHorizontal();

                GUILayout.Space(10);
            }
        }
    }
}
#endif