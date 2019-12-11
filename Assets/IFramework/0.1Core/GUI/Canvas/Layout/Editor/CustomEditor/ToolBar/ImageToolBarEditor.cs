/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{

    [CustomGUIElement(typeof(ImageToolBar))]
    public class ImageToolBarEditor : ToolBarElementEditor
    {
        private ImageToolBar toolbar { get { return element as ImageToolBar; } }
        private bool insFold = true;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            insFold = FormatFoldGUI(insFold, "ToolBar Values", null, ContentGUI);
        }
        private void ContentGUI()
        {
            int size = EditorGUILayout.IntField("Size", toolbar.texs.Length);
            if (size > toolbar.texs.Length)
            {
                var list = toolbar.texs.ToList();
                while (list.Count < size)
                    list.Add(new Texture2D(100, 100));
                toolbar.texs = list.ToArray();
            }
            else if (size < toolbar.texs.Length)
            {
                var list = toolbar.texs.ToList();
                while (list.Count > size)
                    list.RemoveAt(list.Count - 1);
                toolbar.texs = list.ToArray();
            }
            for (int i = 0; i < toolbar.texs.Length; i++)
                this.ObjectField("Image", ref toolbar.texs[i], false);
        }
        public override void OnSceneGUI(Action child)
        {
            if (!toolbar.active) return;
            BeginGUI();
            GUILayout.Label("", CalcGUILayOutOptions());
            toolbar.position = GUILayoutUtility.GetLastRect();
            toolbar.value = GUI.Toolbar(toolbar.position, toolbar.value, toolbar.texs, toolbar.style);
            EndGUI();
        }
    }
}
