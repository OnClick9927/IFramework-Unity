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
    [GUIElement("Image/ImageLabel")]
    public class ImageLabel : ImageElement
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
        public ImageLabel() : base() { }
        public ImageLabel(ImageLabel other) : base(other) { }

        public override void OnGUI(Action child)
        {
            if (!active) return;
            BeginGUI();
            GUILayout.Label(image, imageStyle, CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            EndGUI();
        }
    }
}
