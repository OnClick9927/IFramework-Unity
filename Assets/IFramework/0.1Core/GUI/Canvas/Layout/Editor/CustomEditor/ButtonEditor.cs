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
    [CustomGUIElement(typeof(Button))]
    public class ButtonEditor : TextElementEditor
    {
        private Button ele { get { return element as Button; } }
        private bool insFold = true;
        private GUIStyleDesign btnStyleDrawer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (btnStyleDrawer == null)
                btnStyleDrawer = new GUIStyleDesign(ele.btnStyle, "ButtonStyle");
            insFold = FormatFoldGUI(insFold, "Button", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ObjectField("Image", ref ele.image, false)
                .Pan(() => { ele.mode = (ScaleMode)EditorGUILayout.EnumPopup("Mode", ele.mode); })

                .Toggle("Alpha Blend", ref ele.alphaBlend)
                .FloatField("Image Aspect", ref ele.imageAspect)
                .FloatField("Border Radius", ref ele.borderRadius)
                .Vector4Field("Border Widths", ref ele.borderWidths);
            btnStyleDrawer.OnGUI();
        }
        public override void OnSceneGUI(Action child)
        {
            base.OnSceneGUI(child);

            if (!ele.active) return;
            BeginGUI();
            GUILayout.Label("", CalcGUILayOutOptions());
            ele.position = GUILayoutUtility.GetLastRect();
            BeginGUI();
            GUI.Button(ele.position, "", ele.btnStyle);
            if (ele.image != null)
                GUI.DrawTexture(ele.position, ele.image, ele.mode, ele.alphaBlend, ele.imageAspect, ele.color, ele.borderWidths, ele.borderRadius);
            GUI.Label(ele.position, new GUIContent(ele.text, ele.tooltip), ele.textStyle);
            EndGUI();
        }

    }
}
