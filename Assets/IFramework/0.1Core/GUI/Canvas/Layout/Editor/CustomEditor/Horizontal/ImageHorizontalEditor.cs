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
    [CustomGUIElement(typeof(ImageHorizontal))]
    public class ImageHorizontalEditor : ParentImageElementEditor
    {
        private ImageHorizontal ele { get { return element as ImageHorizontal; } }
        public override void OnSceneGUI(Action child)
        {
            if (!ele.active) return;
            BeginGUI();
            GUILayout.BeginHorizontal(ele.image, ele.imageStyle, CalcGUILayOutOptions());
            if (child != null) child();

            GUILayout.EndHorizontal();
            EndGUI();
        }
    }
}
