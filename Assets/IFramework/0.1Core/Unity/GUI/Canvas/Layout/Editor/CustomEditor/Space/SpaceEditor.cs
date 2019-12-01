/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{
    [CustomGUIElement(typeof(Space))]
    public class SpaceEditor : GUIElementEditor
    {
        private Space ele { get { return element as Space; } }
        private bool insFold = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            insFold = FormatFoldGUI(insFold, "Space", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.FloatField("Pixels", ref ele.pixels);
        }
        public override void OnSceneGUI(Action child)
        {
            if (!ele.active) return;

            BeginGUI();
            GUILayout.Space(ele.pixels);
            ele.position = GUILayoutUtility.GetLastRect();
            EndGUI();
        }
    }
}
