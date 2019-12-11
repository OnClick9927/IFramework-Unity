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
    [GUIElement("Vertical/ImageVertical")]
    public class ImageVertical : ParentImageElement
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
        public ImageVertical() : base() { }
        public ImageVertical(ImageVertical other) : base(other) { }

        public override void OnGUI(Action child)
        {
            if (!active) return;

            BeginGUI();
            GUILayout.BeginVertical(image, imageStyle, CalcGUILayOutOptions());
            if (child != null) child();

            GUILayout.EndVertical();
            EndGUI();
        }
    }
}
