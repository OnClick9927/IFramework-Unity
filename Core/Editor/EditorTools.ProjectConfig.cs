/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEditorInternal;
using UnityEditor.Callbacks;
using System.Text.RegularExpressions;

namespace IFramework
{

    public static partial class EditorTools
    {

        public static class ProjectConfig
        {
            //用正则来匹配对应的文件名和行号
            static Regex reg = new Regex("at[\u4e00-\u9fa5 \f\r\t\na-zA-Z0-9\\.\\-/]*:[0-9]+");
            private static string GetStackTrace()
            {
                var window = EditorWindowTool.Find("UnityEditor.ConsoleWindow");
                if (window == null || window != EditorWindow.focusedWindow) return null;
                var consoleWindowType = window.GetType();
                var activeTextFieldInfo = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                return (string)activeTextFieldInfo.GetValue(window);
            }
            private static bool Contains(string source)
            {
                for (var i = 0; i < Info.logIgnoreFiles.Count; i++)
                {
                    if (source.Contains(Info.logIgnoreFiles[i]))
                    {
                        return true;
                    }
                }
                return false;
            }
            [OnOpenAsset(0)]
            static bool ReposLog(int instanceID, int line)
            {
                var trackInfo = GetStackTrace();
                if (string.IsNullOrEmpty(trackInfo) || !Contains(trackInfo)) return false;
                MatchCollection collection = reg.Matches(trackInfo);
                foreach (Match item in collection)
                {
                    string value = item.Value;
                    value = value.Substring(3);
                    string[] values = value.Split(':');
                    string path = values[0];
                    string _line = values[1];
                    if (!Contains(path))
                    {
                        InternalEditorUtility.OpenFileAtLineExternal(path, int.Parse(_line));
                        return true;
                    }
                }
                return false;
            }
            public static string NameSpace { get { return Info.NameSpace; } }
            public static string UserName { get { return Info.UserName; } }
            public static string Version { get { return Info.Version; } }
            public static bool enable { get { return Info.enable; } }
            public static bool enable_L { get { return Info.enable_L; } }
            public static bool enable_W { get { return Info.enable_W; } }
            public static bool enable_E { get { return Info.enable_E; } }
            public static bool dockWindow { get { return Info.dockWindow; } }
            public static string projectPath { get { return Info.projectPath; } }

            public const string configName = "ProjectConfig";
            [Serializable]
            public class ProjectConfigInfo
            {
                public bool enable = true;
                public bool enable_L = true;
                public bool enable_W = true;
                public bool enable_E = true;
                public bool dockWindow = true;
                public List<string> logIgnoreFiles = new List<string>();
                public string projectPath = "Assets/Project";
                public string Version { get { return PlayerSettings.bundleVersion; } set { PlayerSettings.bundleVersion = value; } }
                public string NameSpace = "IFramework";
                public string UserName { get; private set; }

                public static ProjectConfigInfo Load()
                {
                    var __info = EditorTools.GetFromPrefs<ProjectConfigInfo>(configName, false);
                    if (__info == null) __info = new ProjectConfigInfo();
                    var type = typeof(UnityEditor.Connect.UnityOAuth).Assembly.GetType("UnityEditor.Connect.UnityConnect");
                    var m = type.GetMethod("GetUserInfo");
                    var instance = type.GetProperty("instance");
                    var userInfo = m.Invoke(instance.GetValue(null), null);
                    var _type = userInfo.GetType();
                    var p = _type.GetProperty("displayName");
                    __info.UserName = (string)p.GetValue(userInfo);
                    if (!__info.logIgnoreFiles.Contains(default_path))
                    {
                        __info.logIgnoreFiles.Add(default_path);
                    }
                    __info.Save();
                    return __info;
                }
                public void Save()
                {
                    EditorTools.SaveToPrefs<ProjectConfigInfo>(this, configName, false);
                }
            }

            private static string default_path { get { return UnityLogger.GetLogFilePath(); } }
            private static ProjectConfigInfo __info;
            public static ProjectConfigInfo Info
            {
                get
                {
                    if (__info == null)
                    {
                        __info = ProjectConfigInfo.Load();
                    }
                    return __info;
                }
            }
            public static void Save()
            {
                __info.Save();
            }




        }
    }
}
