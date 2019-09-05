/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-26
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using System;

namespace IFramework
{
    [Serializable]
	public class RenameLabelDrawer:FocusAbleGUIDrawer,IRectGUIDrawer
	{
        public RenameLabelDrawer() : base() { }
        public Action<string> onValueChange;
        public Action<string> onEndEdit;
        public string value;

        public override void OnGUI(Rect position)
        {
            base.OnGUI(position);
            Event e = Event.current;
            if (Focused)
            {
                GUI.SetNextControlName(FocusID);
                string tmp = value;
                this.TextField(position,ref tmp);
                if (value!=tmp)
                {
                    value = tmp;
                    if (onValueChange!=null)
                    {
                        onValueChange(value);
                    }
                }
            }
            else this.Label(position, value);
        
            if (position.Contains(e.mousePosition))
            {
                if (!Focused)
                    if ((e.type == EventType.MouseDown && e.clickCount == 2) || e.keyCode == KeyCode.F2)
                    {
                        Focused = true;
                        GUIFocusControl.Focus(FocusID, Focused);
                        if (e.type != EventType.Repaint && e.type != EventType.Layout)
                            Event.current.Use();
                    }
            }
            if((e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Escape || e.character=='\n'))
            {
                GUIFocusControl.Focus(null, Focused);
                Focused = false;
                if (e.type != EventType.Repaint && e.type != EventType.Layout)
                    Event.current.Use();
                if (onEndEdit!=null)
                {
                    onEndEdit(value);
                }
            }
        }

       
    }
}
