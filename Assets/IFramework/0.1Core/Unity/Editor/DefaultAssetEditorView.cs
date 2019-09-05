/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
namespace IFramework
{
    [CustomEditor(typeof(UnityEditor.DefaultAsset))]
	public class DefaultAssetEditorView:Editor,ILayoutGUIDrawer
	{
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            string path = AssetDatabase.GetAssetPath(this.target);
            GUIUtil.enabled = true;
            if (path.IsDirectory())
            {
                this.Button(() => {
                    ProcessUtil.OpenFloder(path);
                }, "Open");
            }
        }
    }
}
