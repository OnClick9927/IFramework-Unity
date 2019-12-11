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
    [CustomGUIElement(typeof(Horizontal))]
    public class HorizontalEditor : ParentImageElementEditor
    {
        private Horizontal ele { get { return element as Horizontal; } }
        private bool insFold = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            insFold = FormatFoldGUI(insFold, "Content", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ETextField("Text", ref ele.text)
                 .ETextField("Tooltip", ref ele.tooltip);
        }
        public override void OnSceneGUI(Action child)
        {
            if (!ele.active) return;
            BeginGUI();
            GUILayout.BeginHorizontal(new GUIContent(ele.text, ele.image, ele.tooltip), ele.imageStyle, CalcGUILayOutOptions());
            if (child != null) child();

            GUILayout.EndHorizontal();
            EndGUI();
        }
    }
}
