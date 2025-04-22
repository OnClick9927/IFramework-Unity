/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.51
 *UnityVersion:   2018.4.24f1
 *Date:           2020-09-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;

#pragma warning disable
namespace IFramework
{
    partial class RootWindow
    {
        class ProjectConfigTab : UserOptionTab
        {
            static class Contents
            {
                public static GUIContent Name = new GUIContent("UserName", "Project Author's Name");
                public static GUIContent Version = new GUIContent("Version", "Version of Project");
                public static GUIContent Namespace = new GUIContent("NameSpace", "Script's Namespace");
                public static string logset = "LogSetting in Editor mode";
                public static string enable = "Enable";
                public static string logenable = "Log Enable";
                public static string framelogenable = "Frame Log Enable";

                public static string dockWindow = "Dock EditorWindow";


                public static string wenable = "Warning Enable";
                public static string errenable = "Error Enable";
                public static string aenable = "Assert Enable";

                public static string projectPath = "Project Path";
            }

            public override string Name => "ProjectConfig";



            Vector2 scroll;
            private void Sys()
            {
                GUI.enabled = true;

                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Height(2)), new Color(0.5f, 0.5f, 0.5f));

                scroll = GUILayout.BeginScrollView(scroll);

                GUILayout.BeginVertical("Box");

                GUILayout.Label("操作系统：" + SystemInfo.operatingSystem);
                GUILayout.Label("系统内存：" + SystemInfo.systemMemorySize + "MB");
                GUILayout.Label("处理器：" + SystemInfo.processorType);
                GUILayout.Label("处理器数量：" + SystemInfo.processorCount);
                GUILayout.Label("显卡：" + SystemInfo.graphicsDeviceName);
                GUILayout.Label("显卡类型：" + SystemInfo.graphicsDeviceType);
                GUILayout.Label("显存：" + SystemInfo.graphicsMemorySize + "MB");
                GUILayout.Label("显卡标识：" + SystemInfo.graphicsDeviceID);
                GUILayout.Label("显卡供应商：" + SystemInfo.graphicsDeviceVendor);
                GUILayout.Label("显卡供应商标识码：" + SystemInfo.graphicsDeviceVendorID);
                GUILayout.Label("设备模式：" + SystemInfo.deviceModel);
                GUILayout.Label("设备名称：" + SystemInfo.deviceName);
                GUILayout.Label("设备类型：" + SystemInfo.deviceType);
                GUILayout.Label("设备标识：" + SystemInfo.deviceUniqueIdentifier);

                GUILayout.Label("DPI：" + Screen.dpi);
                GUILayout.Label("分辨率：" + Screen.currentResolution.ToString());
                GUILayout.EndVertical();
                GUILayout.EndScrollView();


                GUILayout.FlexibleSpace();

            }
            public override void OnGUI(Rect position)
            {
                var Info = EditorTools.ProjectConfig.Info;
                GUILayout.Space(10);
                EditorGUI.BeginChangeCheck();

                GUI.enabled = false;
                EditorGUILayout.TextField(Contents.Name, EditorTools.ProjectConfig.UserName);
                GUI.enabled = !EditorApplication.isPlaying;
                Info.NameSpace = EditorGUILayout.TextField(Contents.Namespace, Info.NameSpace);


                Info.dockWindow = EditorGUILayout.Toggle(Contents.dockWindow, Info.dockWindow);

                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Height(2)), new Color(0.5f, 0.5f, 0.5f));
                GUILayout.Label(Contents.logset, EditorStyles.largeLabel);
                Info.enable_F = EditorGUILayout.Toggle(Contents.framelogenable, Info.enable_F);
                GUILayout.Space(10);
                Info.enable = EditorGUILayout.Toggle(Contents.enable, Info.enable);
                GUI.enabled &= Info.enable;
                Info.enable_L = EditorGUILayout.Toggle(Contents.logenable, Info.enable_L);
                Info.enable_W = EditorGUILayout.Toggle(Contents.wenable, Info.enable_W);
                Info.enable_E = EditorGUILayout.Toggle(Contents.errenable, Info.enable_E);

                GUI.enabled &= true;
                if (EditorGUI.EndChangeCheck())
                {
                    EditorTools.ProjectConfig.Save();
                    EditorTools.SetLogStatus();
                }
                //Sys();
                BuildFolders();

            }
            [System.Serializable]
            class FolderBuilder
            {
                public string projectPath = "Assets/Project";
                public List<string> folders = new List<string>() {
                "Scripts","Configs","Images","Fonts","Textures","Shaders","Materials"
            };
                public static FolderBuilder _context;
                public static FolderBuilder context
                {

                    get
                    {
                        if (_context == null)
                            _context = EditorTools.GetFromPrefs<FolderBuilder>(nameof(FolderBuilder));

                        if (_context == null)
                            _context = new FolderBuilder();

                        return _context;
                    }
                }
                public static void Save() => EditorTools.SaveToPrefs<FolderBuilder>(context, nameof(FolderBuilder));
            }

            private void BuildFolders()
            {
                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Height(2)), new Color(0.5f, 0.5f, 0.5f));

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Build Folder", EditorStyles.largeLabel);

                GUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                FolderBuilder.context.projectPath = EditorGUILayout.TextField(Contents.projectPath, FolderBuilder.context.projectPath);
                var folders = FolderBuilder.context.folders;
                var projectPath = FolderBuilder.context.projectPath;

                //GUILayout.FlexibleSpace();
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    folders.Add("FolderName");
                }
                if (GUILayout.Button("Build", GUILayout.Width(50)))
                {
                    var list = new List<string>() { projectPath }
                        .Concat(folders.Select(x => projectPath.CombinePath(x)));
                    EditorTools.CreateDirectories(list.ToList());
                }
                GUILayout.EndHorizontal();
                int cloumn_count = 4;
                for (int i = 0; i < folders.Count; i++)
                {
                    if (i % cloumn_count == 0)
                        GUILayout.BeginHorizontal();
                    var src = folders[i];
                    GUILayout.Space(10);
                    folders[i] = EditorGUILayout.TextField(src, GUILayout.MaxWidth(100));
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        folders.Remove(src);
                        EditorTools.ProjectConfig.Save();
                        GUIUtility.ExitGUI();
                    }
                    if (i % cloumn_count == cloumn_count - 1)
                        GUILayout.EndHorizontal();
                }
                if (folders.Count % cloumn_count != 0)
                    GUILayout.EndHorizontal();
                GUILayout.EndVertical();



                if (EditorGUI.EndChangeCheck())
                {
                    FolderBuilder.Save();
                }

                GUILayout.Space(10);
            }
        }

    }

}
