/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    [GUIElement("Text/Label")]
    public class Label : TextElement
    {
        public Label() : base() { }
        public Label(TextElement other) : base(other) { }

        protected override void DrawGUI()
        {
            GUI.Label(position, new GUIContent(text, tooltip), textStyle);
        }
    }
}
