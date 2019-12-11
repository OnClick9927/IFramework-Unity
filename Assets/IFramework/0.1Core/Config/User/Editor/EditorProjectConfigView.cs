/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using UnityEditor;
using UnityEditor.Callbacks;
namespace IFramework
{
    [CustomEditor(typeof(ProjectConfigInfo))]
    class EditorProjectConfigView : Editor, ILayoutGUIDrawer
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
            bool open = isEnable && EditorWindow.focusedWindow.titleContent.text == "Project";
            if (open)
                ProjectConfigWindow.ShowWindow();
            return open;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            this.Space(10)
                .Button(ProjectConfigWindow.ShowWindow, "Open");
        }
    }
}
