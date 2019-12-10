/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-17
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditor;
namespace IFramework
{
    class RepositionLog
	{
        public static string StoName { get { return "LogSetting"; } }
        private static LogSetting info;
        private static LogSetting Info {
            get {
                if (info==null)
                {
                    info = Resources.Load<LogSetting>(StoName);
                }
                return info;
            }
        }
        private static bool Ignore(string Path)
        {
            for (int i = 0; i < Info.Infos.Count; i++)
            {
                if (Path == Info.Infos[i].Path) return true;
            }
            return false;
        }

        [OnOpenAsset(-1)]
        static bool OnOpenAsset(int instanceID, int line)
        {
            string objPath=AssetDatabase.GetAssetPath(instanceID);
            if (!objPath.EndsWith(".cs")) return false;
            if (!Ignore(objPath) && objPath.Contains("Assets"))
            {
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(objPath.ToAbsPath(), line);
                return true;
            }
            string stack_trace = GetLogString();
            if (!string.IsNullOrEmpty(stack_trace))
            {
                Match matches = Regex.Match(stack_trace, @"\(at(.+)\)", RegexOptions.IgnoreCase);
                string pathline = string.Empty;
                if (!matches.Success) return false;
                while (!string.IsNullOrEmpty(matches.Groups[0].Value))
                {
                    pathline = matches.Groups[1].Value;
                    if (pathline.Contains(":"))
                    {
                        pathline = pathline.Trim();
                        int split_index = pathline.LastIndexOf(":");
                        string path = pathline.Substring(0, split_index);
                        if (System.IO.File.Exists(path)/* && path.Contains("Assets")*/ && !Ignore(path))
                        {
                            line = Convert.ToInt32(pathline.Substring(split_index + 1));
                            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path.ToAbsPath(), line);
                            return true;
                        }
                    }
                    matches = matches.NextMatch();
                }
            }
            return false;
        }
        static RepositionLog()
        {
            // 找到UnityEditor.EditorWindow的assembly
             assembly_unity_editor = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.EditorWindow));
            if (assembly_unity_editor == null) Debug.Log("Log Init Err");

            // 找到类UnityEditor.ConsoleWindow
            type_console_window = assembly_unity_editor.GetType("UnityEditor.ConsoleWindow");
            if (type_console_window == null) Debug.Log("Log Init Err");
            // 找到UnityEditor.ConsoleWindow中的成员ms_ConsoleWindow
             field_console_window = type_console_window.GetField("ms_ConsoleWindow", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            if (field_console_window == null) Debug.Log("Log Init Err");
            
        }
        static FieldInfo field_console_window;
        static Assembly assembly_unity_editor;
        static Type type_console_window;
        static string GetLogString()
        {
            // 获取ms_ConsoleWindow的值
           object instance_console_window = field_console_window.GetValue(null);
            if (instance_console_window == null) return null;
            // 如果console窗口时焦点窗口的话，获取stacktrace
            if ((object)UnityEditor.EditorWindow.focusedWindow == instance_console_window)
            {
                // 通过assembly获取类ListViewState
                var type_list_view_state = assembly_unity_editor.GetType("UnityEditor.ListViewState");
                if (type_list_view_state == null) return null;

                // 找到类UnityEditor.ConsoleWindow中的成员m_ListView
                var field_list_view = type_console_window.GetField("m_ListView", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field_list_view == null) return null;

                // 获取m_ListView的值
                var value_list_view = field_list_view.GetValue(instance_console_window);
                if (value_list_view == null) return null;

                // 找到类UnityEditor.ConsoleWindow中的成员m_ActiveText
                var field_active_text = type_console_window.GetField("m_ActiveText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field_active_text == null) return null;

                // 获得m_ActiveText的值，就是我们需要的stacktrace
                string value_active_text = field_active_text.GetValue(instance_console_window).ToString();
                return value_active_text;
            }

            return null;
        }
    }
}
