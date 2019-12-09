/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using System.IO;
using UnityEngine;
using IFramework.GUITool;

namespace IFramework
{
    [EditorWindowCache("ProjectConfig")]
    partial class ProjectConfigWindow
    {
        public static void ShowWindow()
        {
            GetWindow<ProjectConfigWindow>(false, "ProSetting", true);
        }
    }
    partial class ProjectConfigWindow : EditorWindow, ILayoutGUIDrawer
    {
        private ProjectConfigInfo Info;
        private void OnEnable()
        {
            if (File.Exists(EditorProjectConfig.ProjectConfigInfoPath))
                Info = ScriptableObj.Load<ProjectConfigInfo>(EditorProjectConfig.ProjectConfigInfoPath);
            else
                Info = ScriptableObj.Create<ProjectConfigInfo>(EditorProjectConfig.ProjectConfigInfoPath);
        }
        private void OnDisable()
        {
            ScriptableObj.Update(Info);
        }

        private void OnGUI()
        {
            this.Space()
                    .ETextField("UserName", ref Info.UserName)
                    .ETextField("Version", ref Info.Version)
                    .LabelField("NameSpace")
                    .TextArea(ref Info.NameSpace)
                    .TextArea(ref Info.Description, GUILayout.Height(100))
                    //.FlexibleSpace()
                    .BeginHorizontal()
                        .FlexibleSpace()
                        .Button(() => { ScriptableObj.Update(Info); }, "Save")
                    .EndHorizontal();
        }


    }
}
