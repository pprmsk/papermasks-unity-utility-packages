/*
 * Scene Pilot - Scene Switching Utility for Unity3D
 * ------------------------------------------------
 * Automatically detects and organizes scenes under Assets/Scenes,
 * creating an intuitive editor window for quick navigation and scene management.
 *
 * Author: PAPERMASK
 * Version: 1.0.1
 */

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace PAPERMASK.Utilities
{
    public class ScenePilot : EditorWindow
    {
        private const string VERSION = "1.0.1";

        private Dictionary<string, List<string>> scenesByFolder;
        private string[] folderNames;
        private string activeFolder;

        private const float REFRESH_OFFSET = 4f;

        [MenuItem("Window/PAPERMASK Utility/Scene Pilot")]
        public static void ShowWindow()
        {
            GetWindow<ScenePilot>("Scene Pilot");
        }

        private void OnEnable()
        {
            RefreshScenes();
        }

        private void OnGUI()
        {
            Repaint();

            if (scenesByFolder == null || scenesByFolder.Count == 0)
            {
                EditorGUILayout.HelpBox("No scenes found in Assets/Scenes", MessageType.Info);

                if (GUILayout.Button("Refresh"))
                {
                    RefreshScenes();
                }

                return;
            }

            GUILayout.Space(5);
            DrawFolderTabs();

            GUILayout.Space(10);
            DrawSceneButtons();

            GUILayout.FlexibleSpace();
            GUILayout.Label($"Scene Pilot v{VERSION}", EditorStyles.miniLabel);
        }

        private void DrawFolderTabs()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            float totalWidth = 0f;
            foreach (var folder in folderNames)
            {
                string folderName = Path.GetFileName(folder).Replace("Assets", "");
                GUIContent content = new(folderName);
                Vector2 size = EditorStyles.toolbarButton.CalcSize(content);
                totalWidth += size.x + REFRESH_OFFSET;
            }

            var sortedFolders = folderNames?.OrderBy(f => f).ToArray() ?? Array.Empty<string>();
            bool useDropdown = totalWidth > position.width;

            if (useDropdown) 
            {
                DrawDropdownTabs(sortedFolders);
            } 
            else
            {
                DrawButtonTabs(sortedFolders);
            }

            if (GUILayout.Button("↻", EditorStyles.toolbarButton, GUILayout.Width(25)))
            {
                RefreshScenes();
            }

            GUILayout.EndHorizontal();
        }

        private void DrawButtonTabs(string[] sortedFolders)
        {
            foreach (var folder in sortedFolders)
            {
                bool isActive = folder == activeFolder;

                GUIStyle style = new(EditorStyles.toolbarButton)
                {
                    fontStyle = isActive ? FontStyle.Bold : FontStyle.Normal
                };

                string folderName = Path.GetFileName(folder).Replace("Assets", "");

                if (GUILayout.Toggle(isActive, folderName, style, GUILayout.MinWidth(70)))
                {
                    activeFolder = folder;
                }
            }

            GUILayout.FlexibleSpace();
        }

        private void DrawDropdownTabs(string[] sortedFolders)
        {
            int index = Array.IndexOf(sortedFolders, activeFolder);

            int newIndex = EditorGUILayout.Popup(
                index,
                sortedFolders.Select(Path.GetFileName).ToArray(),
                EditorStyles.popup,
                GUILayout.ExpandWidth(true)
            );

            if (newIndex != index && newIndex >= 0)
            {
                activeFolder = sortedFolders[newIndex];
            }
        }

        private void DrawSceneButtons()
        {
            if (string.IsNullOrEmpty(activeFolder)) return;
            if (!scenesByFolder.ContainsKey(activeFolder)) return;

            foreach (var scenePath in scenesByFolder[activeFolder])
            {
                string name = Path.GetFileNameWithoutExtension(scenePath);

                if (GUILayout.Button(name, GUILayout.Height(25)))
                {
                    OpenScene(scenePath);
                }
            }
        }

        private void RefreshScenes()
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
            if (guids == null || guids.Length == 0)
            {
                scenesByFolder = new Dictionary<string, List<string>>();
                folderNames = Array.Empty<string>();
                activeFolder = null;
                return;
            }

            var paths = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();

            scenesByFolder = paths
                .GroupBy(path => GetFolderName(path))
                .ToDictionary(g => g.Key, g => g.OrderBy(p => p).ToList());

            folderNames = scenesByFolder.Keys.OrderBy(n => n).ToArray();
            activeFolder ??= folderNames.FirstOrDefault();
        }

        private static string GetFolderName(string scenePath)
        {
            string dir = Path.GetDirectoryName(scenePath).Replace("\\", "/");

            int index = dir.IndexOf("Assets/Scenes/");
            string subPath = index >= 0 ? dir[(index + "Assets/Scenes/".Length)..] : dir;

            return string.IsNullOrEmpty(subPath) ? "Root" : subPath;
        }

        private void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }
    }
}