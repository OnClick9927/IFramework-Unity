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
    [CustomGUIElement(typeof(TextArea))]
    public class TextAreaEditor : TextElementEditor
    {
        private TextArea textElement { get { return element as TextArea; } }

        private GUIStyleEditor textStyleDrawer;
        public override void OnSceneGUI(Action children)
        {
            base.OnSceneGUI(children);

            if (!element.active) return;

            BeginGUI();
            textElement.text = GUI.TextArea(textElement.position, textElement.text, textElement.textStyle);
            if (children != null) children();

            EndGUI();
        }
    }
}
