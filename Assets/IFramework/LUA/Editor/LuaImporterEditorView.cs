/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
#if UNITY_2018_1_OR_NEWER

namespace IFramework
{
    [CustomEditor(typeof(LuaImporter))]
    public class LuaImporterEditorView: ScriptedImporterEditor,ILayoutGUIDrawer
    {
        LuaImporter im { get { return this.target as LuaImporter; } }
        public override void OnInspectorGUI()
        {
            this.Button(() =>
            {
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(im.assetPath, 0);

            }, "Edit");
        }
    }
}
#endif