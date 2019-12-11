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
    [GUIElement("Image/ImageBox")]
    public class ImageBox : ImageElement
    {
        public ImageBox() : base() { }
        public ImageBox(ImageBox other) : base(other) { }
        public override void OnGUI(Action child)
        {
            if (active)
            {
                BeginGUI();
                GUI.Box(position, image, imageStyle);
                if (child != null) child();

                EndGUI();
            }
        }

    }
}
