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
    [CustomGUIElement(typeof(TextElement))]
    public class TextElementEditor : GUIElementEditor
    {
        private TextElement textElement { get { return element as TextElement; } }
        private bool insFold = true;
        private GUIStyleDesign textStyleDrawer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (textStyleDrawer == null)
                textStyleDrawer = new GUIStyleDesign(textElement.textStyle, "Text Style");
            insFold = FormatFoldGUI(insFold, "Text Element", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.LabelField("Text")
                .TextArea(ref textElement.text, GUILayout.Height(50))
                .LabelField("Tooltip")
                .TextArea(ref textElement.tooltip, GUILayout.Height(50))
                .ObjectField("Font", ref textElement.font, false)
                .IntField("Font Stize", ref textElement.fontSize)
                .Toggle("Rich Text", ref textElement.richText)
                .Pan(() => {
                    textElement.overflow = (TextClipping)EditorGUILayout.EnumPopup("Over flow", textElement.overflow);
                    textElement.alignment = (TextAnchor)EditorGUILayout.EnumPopup("Alignment", textElement.alignment);
                    textElement.fontStyle = (FontStyle)EditorGUILayout.EnumPopup("Font Style", textElement.fontStyle);
                });

            textStyleDrawer.OnGUI();
        }
        public override void OnSceneGUI(Action children)
        {
            if (element.active)
            {
                textElement.textStyle.font = textElement.font;
                textElement.textStyle.fontStyle = textElement.fontStyle;
                textElement.textStyle.fontSize = textElement.fontSize;
                textElement.textStyle.alignment = textElement.alignment;
                textElement.textStyle.clipping = textElement.overflow;
                textElement.textStyle.richText = textElement.richText;
            }
        }
    }
}
