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
    [CustomGUIElement(typeof(Area))]
    public class AreaEditor : ImageAreaEditor
    {
        private Area ele { get { return element as Area; } }
        private bool insFold = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            insFold = FormatFoldGUI(insFold, "Area Content", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ETextField("Text", ref ele.text)
                  .ETextField("tooltip", ref ele.tooltip);
        }
        public override void OnSceneGUI(Action child)
        {
            if (!ele.active) return;
            BeginGUI();
            GUILayout.BeginArea(ele.areaRect, new GUIContent(ele.text, ele.image, ele.tooltip), ele.imageStyle);
            if (child != null) child();
            GUILayout.EndArea();
            EndGUI();
        }
    }
}
