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
    [CustomGUIElement(typeof(Image))]
    public class ImageEditor : GUIElementEditor
    {
        private Image image { get { return element as Image; } }
        private bool insFold = true;
        public override void OnSceneGUI(Action children)
        {
            if (!image.active) return;
            BeginGUI();
            if (image != null)
                GUI.DrawTexture(image.position, image.image, image.mode, image.alphaBlend, image.imageAspect, image.color, image.borderWidths, image.borderRadius);
            if (children != null) children();

            EndGUI();

        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            insFold = FormatInspectorHeadGUI(insFold, "Image", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ObjectField("Image", ref image.image, false)
                .Pan(() => { image.mode = (ScaleMode)EditorGUILayout.EnumPopup("Mode", image.mode); })

                .Toggle("Alpha Blend", ref image.alphaBlend)
                .FloatField("Image Aspect", ref image.imageAspect)
                .FloatField("Border Radius", ref image.borderRadius)
                .Vector4Field("Border Widths", ref image.borderWidths);
        }
    }
}
