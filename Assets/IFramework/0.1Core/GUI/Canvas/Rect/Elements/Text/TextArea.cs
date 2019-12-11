/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    [GUIElement("Text/TextArea")]
    public class TextArea : TextElement
    {
        public Action<string> onValueChange { get; set; }
        public override GUIStyle textStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.textArea);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }

        public TextArea() : base() { }
        public TextArea(TextElement other) : base(other) { }

        protected override void DrawGUI()
        {
            string tmp = GUI.TextArea(position, text, textStyle);
            if (tmp != text)
            {
                text = tmp;
                if (onValueChange != null) onValueChange(tmp);
            }
        }
    }

}
