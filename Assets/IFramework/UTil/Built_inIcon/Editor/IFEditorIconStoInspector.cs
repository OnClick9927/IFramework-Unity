/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-05
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using IFramework.GUITool;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace IFramework
{
    [CustomEditor(typeof(IFEditorIconSto))]
    public class IFEditorIconStoInspector : Editor, ILayoutGUIDrawer
    {
        private IFEditorIconSto sto;
        private void OnEnable()
        {
            isEnable = true;
            if (sto == null) sto = target as IFEditorIconSto;
        }
        public override void OnInspectorGUI()
        {
            if (sto == null) sto = target as IFEditorIconSto;
            this.Label("Too  Large to Show");
            this.Space(10);
            this.Label("Simple Info");
            this.Label("Buildt_inIcon Count:\t" + sto.Buildt_inIcon.Count);
            this.Space(10);
            this.Button(() =>
            {
                ShowWindow();
            }, "Open", GUILayout.Height(25));
        }
        private static bool isEnable;
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
            IFEditorIconWindow.ShowWindow();
        }
    }
}
