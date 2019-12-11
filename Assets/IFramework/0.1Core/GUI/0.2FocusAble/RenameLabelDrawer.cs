/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-26
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using System;

namespace IFramework.GUITool
{
    [Serializable]
	public class RenameLabelDrawer:FocusAbleGUIDrawer,IRectGUIDrawer
	{
        public RenameLabelDrawer() : base() { }
        public override void Dispose()
        {
            base.Dispose();
            onValueChange = null;
            onEndEdit = null;
        }
        private int clickCount = 0;

        public event Action<string> onValueChange;
        public event Action<string> onEndEdit;

        public string value;

        public bool HaveClicked { get { return clickCount > 0; } }
        public bool IsEditing { get { return Focused && clickCount == 2; } }

        protected override void OnLostFous()
        {
            clickCount = 0;
        }
        protected override void OnFocusOther()
        {
            clickCount = 0;
        }

        public override void OnGUI(Rect position)
        {
            base.OnGUI(position);
            Event e = Event.current;
            if (IsEditing)
            {
                GUI.SetNextControlName(FocusID);
                string tmp = value;
                this.TextField(position, ref tmp);
                if (value != tmp)
                {
                    value = tmp;
                    if (onValueChange != null)
                        onValueChange(value);
                }
            }
            else
            {
                this.Label(position, value);
            }

            if (position.Contains(e.mousePosition))
            {
                if (!Focused)
                {
                    if (e.type == EventType.MouseDown && e.clickCount == 1)
                    {
                        if (clickCount<2) clickCount++;
                        if (clickCount==2)
                        {
                            GUIFocusControl.Focus(this);
                            if (e.type != EventType.Repaint && e.type != EventType.Layout)
                                Event.current.Use();
                        }
                    }
                    if (e.keyCode == KeyCode.F2)
                    {
                        clickCount = 2;
                        GUIFocusControl.Focus(this);
                        if (e.type != EventType.Repaint && e.type != EventType.Layout)
                            Event.current.Use();
                    }
                }
            }
            else
            {
                if (e.type == EventType.MouseDown && e.clickCount == 1)
                {
                    clickCount = 0;
                    GUIFocusControl.Diffuse(this);
                    if (onEndEdit != null) onEndEdit(value);
                }
            }
            if(e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Escape || e.character == '\n' || e.button==1)
            {
                GUIFocusControl.Diffuse(this);
                if (e.type != EventType.Repaint && e.type != EventType.Layout)
                    Event.current.Use();
                if (onEndEdit != null) onEndEdit(value);
            }
        }
        public override void Focus()
        {
            base.Focus();
            clickCount = 2;
        }
        public override void Difuse()
        {
            base.Difuse();
            clickCount = 0;
        }
    }
}
