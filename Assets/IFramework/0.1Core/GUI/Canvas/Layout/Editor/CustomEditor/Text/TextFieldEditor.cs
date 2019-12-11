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
    [CustomGUIElement(typeof(TextField))]
    public class TextFieldEditor : TextElementEditor
    {
        private TextField textElement { get { return element as TextField; } }

        private GUIStyleDesign textStyleDrawer;
        public override void OnSceneGUI(Action children)
        {
            base.OnSceneGUI(children);

            if (!element.active) return;

            BeginGUI();
            textElement.text = GUILayout.TextField(textElement.text, textElement.textStyle, CalcGUILayOutOptions());
            textElement.position = GUILayoutUtility.GetLastRect();

            EndGUI();
        }
    }
}
