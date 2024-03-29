﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

namespace IFramework
{
    [InitializeOnLoad]
    partial class EditorTools
    {
        static EditorTools()
        {
            ScriptEnvCheck();
            var directorys = new List<string>()
            {
                EditorTools.projectMemoryPath,
                EditorTools.projectMemoryPath_unique,
                EditorTools.projectPath,
                EditorTools.projectConfigPath,
                EditorTools.projectScriptPath,
            };
            foreach (var path in directorys)
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }


        public const string projectMemoryPath = "Assets/Editor";
        public const string projectMemoryPath_unique = "Assets/Editor/Unique";

        public static string projectPath => ProjectConfig.projectPath;
        public static string projectConfigPath { get { return projectPath.CombinePath("Configs"); } }
        public static string projectScriptPath { get { return projectPath.CombinePath("Scripts"); } }
        private static void ScriptEnvCheck()
        {
#if UNITY_2018_1_OR_NEWER
            PlayerSettings.allowUnsafeCode = true;
#else
            string  path = UnityEngine.Application.dataPath.CombinePath("mcs.rsp");
            string content = "-unsafe";
            if (File.Exists(path) && path.ReadText(System.Text.Encoding.Default) == content) return;
                path.WriteText(content, System.Text.Encoding.Default); 
            AssetDatabase.Refresh();
            EditorTools.Quit();
#endif
        }


        public static void SaveToPrefs<T>(T value, string key, bool unique = true)
        {
            Prefs.SetObject(value.GetType(),key, value, unique);
        }
        public static T GetFromPrefs<T>(string key, bool unique = true)
        {
            return Prefs.GetObject<T>(typeof(T),key, unique);
        }
        public static void OpenFolder(string folder)
        {
            EditorUtility.OpenWithDefaultApp(folder);
        }
        [MenuItem("Tools/IFramework/UpdateCS")]
        public static void UpdateCS()
        {

            var path = AssetDatabase.FindAssets("t:script Launcher")
                      .ToList()
                      .ConvertAll(x => AssetDatabase.GUIDToAssetPath(x))
                      .Find(x =>
                      {
                          var script = AssetDatabase.LoadAssetAtPath<MonoScript>(x);
                          if (script == null) return false;
                          var cls = AssetDatabase.LoadAssetAtPath<MonoScript>(x).GetClass();
                          return cls == typeof(Launcher);
                      });
            if (string.IsNullOrEmpty(path)) return;
            path = path.CombinePath("../.CS/UpdateCS.bat").ToAbsPath();
            var startInfo = new ProcessStartInfo(path);
            startInfo.WorkingDirectory = Path.GetDirectoryName(path);
            Process.Start(startInfo);
        }
    }
}
