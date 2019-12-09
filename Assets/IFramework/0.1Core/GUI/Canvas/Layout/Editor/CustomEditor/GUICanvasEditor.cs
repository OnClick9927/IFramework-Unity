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

    [CustomGUIElement(typeof(GUICanvas))]
    public class GUICanvasEditor : AreaEditor
    {
        private GUICanvas ele { get { return element as GUICanvas; } }
        private bool insFold = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            insFold = FormatFoldGUI(insFold, "Canvas", null, ContentGUI);
        }
        private void ContentGUI()
        {
            ele.canvasRect = EditorGUILayout.RectField("Canvas Rect", ele.canvasRect);
        }

        public override void OnSceneGUI(Action children)
        {
            if (!ele.active) return;
            ele.canvasRect.DrawOutLine(10, Color.black);

            GUILayout.BeginArea(ele.canvasRect);
            BeginGUI();
            if (children != null) children();
            EndGUI();
            GUILayout.EndArea();
        }
    }
}
