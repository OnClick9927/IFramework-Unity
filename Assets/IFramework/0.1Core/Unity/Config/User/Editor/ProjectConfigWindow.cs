/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using System.IO;
using UnityEngine;
namespace IFramework
{
    [EditorWindowCacheAttribute("ProjectConfig", "IFramework/ProjectConfig")]
    internal partial class ProjectConfigWindow
	{
        [MenuItem("IFramework/ProjectConfig")]
        public static void ShowWindow()
        {
            GetWindow<ProjectConfigWindow>(false, "ProSetting", true);
        }
        private class ProjectSettingView : ILayoutGUIDrawer
        {
            public ProjectConfigInfo Info;

            public  void OnGUI()
            {
                this.  Space();
                this. ETextField("UserName",ref Info.UserName);
                this. ETextField("Version",ref Info.Version);
               this.Toggle("IsUseNameSpace",ref Info.IsUseNameSpace);
                if (Info.IsUseNameSpace)
                {
                   this. LabelField("NameSpace");
                    this. TextArea(ref Info.NameSpace);
                }
               this.TextArea(ref Info.Description, GUIUtil.Height(100));
            }
        }
    }
    internal partial class ProjectConfigWindow:EditorWindow
    {
        private ProjectConfigInfo projectSetting;
        private ProjectSettingView view;
        private void OnEnable()
        {
            if (File.Exists(ProjectConfigEditor.ProjectConfigInfoPath))
                projectSetting = ScriptableObj.Load<ProjectConfigInfo>(ProjectConfigEditor.ProjectConfigInfoPath);
            else
                projectSetting = ScriptableObj.Create<ProjectConfigInfo>(ProjectConfigEditor.ProjectConfigInfoPath);

            if (view==null)
            {
                view = new ProjectSettingView();
                view.Info = projectSetting;
            }
        }
        private void OnDisable()
        {
            ScriptableObj.Update<ProjectConfigInfo>(projectSetting);
        }
        private void OnDestroy()
        {
            ScriptableObj.Update<ProjectConfigInfo>(projectSetting);
        }
        private void OnGUI()
        {
            view.OnGUI();
        }

       
    }
}
