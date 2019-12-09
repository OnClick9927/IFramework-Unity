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
    [CustomGUIElement(typeof(ImageVertical))]
    public class ImageVerticalEditor : ParentImageElementEditor
    {
        private ImageVertical ele { get { return element as ImageVertical; } }
        public override void OnSceneGUI(Action child)
        {
            if (!ele.active) return;
            BeginGUI();
            GUILayout.BeginVertical(ele.image, ele.imageStyle, CalcGUILayOutOptions());
            if (child != null) child();

            GUILayout.EndVertical();
            EndGUI();
        }
    }
}
