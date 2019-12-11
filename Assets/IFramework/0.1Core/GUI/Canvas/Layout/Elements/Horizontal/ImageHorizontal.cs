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
    [GUIElement("Horizontal/ImageHorizontal")]
    public class ImageHorizontal : ParentImageElement
    {
        public override GUIStyle imageStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.label);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }

        public ImageHorizontal() : base() { }
        public ImageHorizontal(ImageHorizontal other) : base(other) { }

        public override void OnGUI(Action child)
        {
            if (!active) return;
            BeginGUI();
            GUILayout.BeginHorizontal(image, imageStyle, CalcGUILayOutOptions());
            if (child != null) child();

            GUILayout.EndHorizontal();
            EndGUI();
        }
    }
}
