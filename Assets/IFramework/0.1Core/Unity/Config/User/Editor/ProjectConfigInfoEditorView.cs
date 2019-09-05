/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
namespace IFramework
{
    [CustomEditor(typeof(ProjectConfigInfo))]
    public class ProjectConfigInfoEditorView : UnityEditor.Editor
    {
        private static bool isEnable;
        private void OnEnable()
        {
            isEnable = true;
        }

        private void OnDisable()
        {
            isEnable = false;
        }

        //固定写法，双击打开自定义面板
        [OnOpenAsset(1)]
        public static bool OpenAsset(int instanceID, int line)
        {
            if (isEnable == false)
                return false;
            if (EditorWindow.focusedWindow.titleContent.text != "Project") return false;
            ShowWindow();
            return true;
        }

        private static void ShowWindow()
        {
            ProjectConfigWindow.ShowWindow();
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            if (GUILayout.Button("Open"))
            {
                ShowWindow();
            }
           
        }
    }
}
