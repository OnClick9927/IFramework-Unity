/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    [CustomGUIElement(typeof(ScrollView))]
    public class ScrollViewEditor : GUIElementEditor
    {
        private ScrollView ele { get { return element as ScrollView; } }
        private bool insFold = true;
        private GUIStyleEditor HstyleDrawer;
        private GUIStyleEditor VstyleDrawer;

        public override void OnSceneGUI(Action children)
        {
            if (!ele.active) return;

            BeginGUI();
            ele.value = GUI.BeginScrollView(ele.position, ele.value, ele.contentRect, ele.alwaysShowHorizontal, ele.alwaysShowVertical, ele.Hstyle, ele.Vstyle);
            if (children != null) children();
            GUI.EndScrollView();

            EndGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (HstyleDrawer == null)
                HstyleDrawer = new GUIStyleEditor(ele.Hstyle, "Hrizontal Style");
            if (VstyleDrawer == null)
                VstyleDrawer = new GUIStyleEditor(ele.Vstyle, "Vertical Style");
            insFold = FormatInspectorHeadGUI(insFold, "ScrollView", null, ContentGUI);
        }
        private void ContentGUI()
        {
            using (new EditorGUI.DisabledScope(true)) EditorGUILayout.RectField("Perfect Content Rect", ele.perfectContentRect);
            ele.contentRect = EditorGUILayout.RectField("Content Rect", ele.contentRect);
            ele.alwaysShowHorizontal = EditorGUILayout.Toggle("Always Show Horizontal", ele.alwaysShowHorizontal);
            ele.alwaysShowVertical = EditorGUILayout.Toggle("Always Show Vertical", ele.alwaysShowVertical);
            ele.value = EditorGUILayout.Vector2Field("Value", ele.value);
            HstyleDrawer.OnGUI();
            VstyleDrawer.OnGUI();
        }

    }
}
