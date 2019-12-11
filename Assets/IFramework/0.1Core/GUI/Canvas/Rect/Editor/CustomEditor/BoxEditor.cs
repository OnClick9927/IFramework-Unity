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

namespace IFramework.GUITool.RectDesign
{
    [CustomGUIElement(typeof(Box))]
    public class BoxEditor : ImageElementEditor
    {
        private Box ele { get { return element as Box; } }
        private bool insFold = true;

        public override void OnSceneGUI(Action children)
        {
            if (!ele.active) return;
            BeginGUI();
            GUI.Box(ele.position, new GUIContent(ele.text, ele.image, ele.tooltip), ele.imageStyle);
            if (children != null) children();

            EndGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            insFold = FormatInspectorHeadGUI(insFold, "Content", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ETextField("Text", ref ele.text)
                 .ETextField("Tooltip", ref ele.tooltip);
        }
    }
}
