/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using IFramework.Utility;
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    [CustomEditor(typeof(DefaultAsset))]
	public class DefaultAssetEditorView:Editor,ILayoutGUIDrawer
	{
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            string path = AssetDatabase.GetAssetPath(this.target);
            GUI.enabled = true;
            if (path.IsDirectory())
            {
                this.Button(() => {
                    ProcessUtil.OpenFloder(path);
                }, "Open");
            }
        }
    }
}
