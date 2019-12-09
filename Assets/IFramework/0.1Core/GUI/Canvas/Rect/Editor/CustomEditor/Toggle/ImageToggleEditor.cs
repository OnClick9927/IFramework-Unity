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
    [CustomGUIElement(typeof(ImageToggle))]
    public class ImageToggleEditor : ImageElementEditor
    {
        private ImageToggle toggle { get { return element as ImageToggle; } }
        private bool insFold = true;



        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            insFold = FormatInspectorHeadGUI(insFold, "Toggle", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.Toggle("Value", ref toggle.value);
        }
        public override void OnSceneGUI(Action children)
        {
            if (!toggle.active) return;
            BeginGUI();
            toggle.value = GUI.Toggle(toggle.position, toggle.value, toggle.image, toggle.imageStyle);
            if (children != null) children();

            EndGUI();


        }
    }
}
