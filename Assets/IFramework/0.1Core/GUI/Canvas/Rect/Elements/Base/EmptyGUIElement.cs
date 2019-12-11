/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.GUITool.RectDesign
{
    [GUIElement("Empty")]
    public class EmptyGUIElement : GUIElement
    {
        public event Action<EmptyGUIElement> pan;
        public override void OnGUI(Action child)
        {
            if (!active) return;
            BeginGUI();
            if (pan!=null)
            {
                pan(this);
            }
            if (child != null)
                child();
            EndGUI();
        }
    }
}
