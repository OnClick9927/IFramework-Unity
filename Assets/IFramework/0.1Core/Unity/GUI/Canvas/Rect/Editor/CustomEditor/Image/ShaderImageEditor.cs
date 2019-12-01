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
    [CustomGUIElement(typeof(ShaderImage))]
    public class ShaderImageEditor : GUIElementEditor
    {
        private ShaderImage image { get { return element as ShaderImage; } }
        private bool insFold = true;

        public override void OnSceneGUI(Action children)
        {
            if (!image.active) return;
            BeginGUI();
            if (image.image != null)
            {
                if (image.material != null)
                {
                    if (image.pass > image.material.passCount)
                        image.pass = image.material.passCount - 1;
                }
                else image.pass = -1;
                Graphics.DrawTexture(image.position, image.image, image.sourceRect, image.leftBorder, image.rightBorder, image.topBorder, image.bottomBorder, image.material, image.pass);
            }
            if (children != null) children();

            EndGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            insFold = FormatInspectorHeadGUI(insFold, "ShaderImage", null, ContentGUI);
        }

        private void ContentGUI()
        {
            image.pass = EditorGUILayout.IntField("Pass", image.pass);
            if (image.material != null)
            {
                if (image.pass > image.material.passCount)
                    image.pass = image.material.passCount - 1;
            }
            this.IntField("Left Border", ref image.leftBorder)
                .IntField("Right Border", ref image.rightBorder)
                .IntField("Top Border", ref image.topBorder)
                .IntField("Bottom Border", ref image.bottomBorder)
                .RectField("Source Rect", ref image.sourceRect)
                .ObjectField("Image", ref image.image, false)
                .ObjectField("Image", ref image.material, false);
        }
    }
}
