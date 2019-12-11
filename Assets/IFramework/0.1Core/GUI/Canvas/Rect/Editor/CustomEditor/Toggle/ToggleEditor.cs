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
    [CustomGUIElement(typeof(Toggle))]
    public class ToggleEditor : ImageToggleEditor
    {
        private Toggle toggle { get { return element as Toggle; } }
        private bool insFold = true;

        public override void OnSceneGUI(Action children)
        {
            if (!toggle.active) return;
            BeginGUI();
            toggle.value = GUI.Toggle(toggle.position, toggle.value, new GUIContent(toggle.text, toggle.image, toggle.tooltip), toggle.imageStyle);
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
            this.ETextField("Text", ref toggle.text)
                .ETextField("Tooltip", ref toggle.tooltip);
        }
    }
}
