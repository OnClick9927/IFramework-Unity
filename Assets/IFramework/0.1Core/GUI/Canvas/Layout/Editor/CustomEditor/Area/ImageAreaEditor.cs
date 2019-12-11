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

namespace IFramework.GUITool.LayoutDesign
{
    [CustomGUIElement(typeof(ImageArea))]
    public class ImageAreaEditor : ParentImageElementEditor
    {
        private ImageArea ele { get { return element as ImageArea; } }
        private bool insFold = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            insFold = FormatFoldGUI(insFold, "Area Position", null, ContentGUI);
        }
        private void ContentGUI()
        {
            ele.areaRect = EditorGUILayout.RectField("Area Rect", ele.areaRect);
        }
        public override void OnSceneGUI(Action child)
        {
            if (!ele.active) return;
            BeginGUI();
            GUILayout.BeginArea(ele.areaRect, ele.image, ele.imageStyle);
            if (child != null) child();
            GUILayout.EndArea();
            EndGUI();
        }
    }
}
