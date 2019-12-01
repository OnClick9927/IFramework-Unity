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
    [GUIElement("Text/Label")]
    public class Label : TextElement
    {
        public Label() : base() { }
        public Label(TextElement other) : base(other) { }

        public override void OnGUI(Action child)
        {
            base.OnGUI(child);
            if (!active) return;
            BeginGUI();
            GUILayout.Label(new GUIContent(text, tooltip), textStyle, CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            EndGUI();
        }
    }
}
