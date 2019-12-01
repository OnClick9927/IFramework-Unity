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
    [CustomGUIElement(typeof(ScrollView))]
    public class ScrollViewEditor : ParentGUIElementEditor
    {
        private ScrollView ele { get { return element as ScrollView; } }
        private bool insFold = true;
        private GUIStyleDesign HstyleDrawer;
        private GUIStyleDesign VstyleDrawer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (HstyleDrawer == null)
                HstyleDrawer = new GUIStyleDesign(ele.Hstyle, "Hrizontal Style");
            if (VstyleDrawer == null)
                VstyleDrawer = new GUIStyleDesign(ele.Vstyle, "Vertical Style");
            insFold = FormatFoldGUI(insFold, "ScrollView", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.Toggle("Always Show Horizontal", ref ele.alwaysShowHorizontal)
                .Toggle("Always Show Vertical", ref ele.alwaysShowVertical)
                .Vector2Field("Value", ref ele.value);
            HstyleDrawer.OnGUI();
            VstyleDrawer.OnGUI();
        }
        public override void OnSceneGUI(Action child)
        {
            if (!ele.active) return;
            BeginGUI();
            ele.value = GUILayout.BeginScrollView(ele.value, ele.alwaysShowHorizontal, ele.alwaysShowVertical, ele.Hstyle, ele.Vstyle, CalcGUILayOutOptions());
            if (child != null) child();
            GUILayout.EndScrollView();
            EndGUI();
        }

    }
}
