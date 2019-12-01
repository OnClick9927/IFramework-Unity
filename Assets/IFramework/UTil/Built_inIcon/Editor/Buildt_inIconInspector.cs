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
using UnityEngine;

namespace IFramework
{
    [CustomEditor(typeof(Buildt_inIcon))]
	public class Buildt_inIconInspector: UnityEditor.Editor,ILayoutGUIDrawer
    {
        private Buildt_inIcon icon;
        private void OnEnable()
        {
            if (icon == null) icon = target as Buildt_inIcon;

        }
        public override void OnInspectorGUI()
        {
            if (icon == null) icon = target as Buildt_inIcon;
            this.Label("Name\t" + icon.iconName);
            this.Button(() =>
            {
                GUIUtility.systemCopyBuffer = icon.iconName;
            }, "Copy", GUILayout.Height(15));
            this.Label("View\t" );
            this.Label(icon.content);
        }
    }
}
