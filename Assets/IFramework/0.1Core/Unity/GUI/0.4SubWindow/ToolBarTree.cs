/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.GUITool
{
    public class ToolBarTree
    {
        private class Styles
        {
            public static GUIStyle ToolBar = new GUIStyle("ToolBar");
            public static GUIStyle toolbarbutton =new GUIStyle("toolbarbutton");
            public static GUIStyle Tooltip = new GUIStyle("Tooltip");
            public static GUIStyle DropDown = ("ToolbarDropDown");
        }
        public abstract class ToolbarNode:ILayoutGUIDrawer
        {
            protected GUIContent content;
            protected float width;
            public ToolbarNode(GUIContent content, int width) { this.width = width; this.content = content; }
            public ToolbarNode(GUIContent content) : this(content, 100) { }

            public abstract void OnGUI();
        }
        private class ToolBarSpace : ToolbarNode
        {
            public ToolBarSpace(int width) : base(null, width) { }
            public ToolBarSpace(GUIContent content, int width) : base(content, width) { }
            public override void OnGUI()
            {
                this.Space(width);
            }
        }
        private class ToolBarFlexibleSpace : ToolbarNode
        {
            public ToolBarFlexibleSpace() : base(null, 100) { }
            public ToolBarFlexibleSpace(GUIContent content, int width) : base(content, width) { }
            public override void OnGUI()
            {
                this.FlexibleSpace();
            }
        }

        private class DelegateLabel : ToolbarNode
        {
            public event Action<Rect> panDel;
            public DelegateLabel(Action<Rect> panDel, int width) : base(null, width) { this.panDel = panDel; }
            public override void OnGUI()
            {
                this.Label("", GUILayout.Width(width));
                if (panDel != null)
                    panDel(GUILayoutUtility.GetLastRect());
            }
        }

        private class ToolBarLabel : ToolbarNode
        {
            public ToolBarLabel(GUIContent content, int width) : base(content, width) { }
            public override void OnGUI()
            {
                this.Label(content, GUILayout.Width(width));
            }
        }
        private class ToolBarToolTip : ToolbarNode
        {
            public ToolBarToolTip(GUIContent content, int width) : base(content, width) { }
            public override void OnGUI()
            {
                this.Label(content, Styles.Tooltip, GUILayout.Width(width));
            }
        }

        private class ToolBarSearchField : ToolbarNode
        {

            private event Action<string> onValueChange;
            public ToolBarSearchField(Action<string> onValueChange, string value, int width = 100) : this(null, onValueChange, value, width) { }

            public ToolBarSearchField(GUIContent content, Action<string> onValueChange, string value, int width = 100) : base(content, width)
            {
                this.onValueChange = onValueChange;

                s = new SearchFieldDrawer() { value=value,};
                s.onValueChange+= (str)=> {
                    if (this.onValueChange != null)
                        this.onValueChange(str);
                    value = str;
                };
            }
            private SearchFieldDrawer s;
            public override void OnGUI()
            {
                this.Label("", GUILayout.Width(width));
                s.OnGUI(GUILayoutUtility.GetLastRect());
            }
        }
        private class ToolBarButton : ToolbarNode
        {
            private event Action<Rect> onClick;
            public ToolBarButton(GUIContent content, Action<Rect> onClick, int width = 100) : base(content, width) { this.onClick = onClick; }

            public override void OnGUI()
            {
                this.Label(string.Empty, GUILayout.Width(width));
                Rect r = GUILayoutUtility.GetLastRect();
                this.GetRectDrawer().Button(() => { if (onClick != null) onClick(r); }, r, content,Styles.toolbarbutton);
            }
        }
        private class ToolBarDropDownButton : ToolbarNode
        {
            private event Action<Rect> onClick;
            public ToolBarDropDownButton(GUIContent content, Action<Rect> onClick, int width = 100) : base(content, width) { this.onClick = onClick; }

            public override void OnGUI()
            {
                GUILayout.Label(string.Empty, GUILayout.Width(width));
                Rect r = GUILayoutUtility.GetLastRect();
                this.GetRectDrawer().Button(() => { if (onClick != null) onClick(r); }, r, content, Styles.DropDown);
            }
        }
        private class ToolBarToggle : ToolbarNode
        {
            private bool value;
            private event Action<bool> onValueChange;
            public ToolBarToggle(GUIContent content, Action<bool> onValueChange, bool value = true, int width = 100) : base(content, width)
            {
                this.onValueChange = onValueChange;
                this.value = value;
            }

            public override void OnGUI()
            {

                bool val = value;
                this.Toggle(ref val, content, Styles.toolbarbutton, GUILayout.Width(width));
                if (val != value)
                {
                    value = val;
                    if (onValueChange != null) onValueChange(value);
                }
            }
        }
      
        private List<ToolbarNode> Nodes = new List<ToolbarNode>();
        public void OnGUI(Rect position)
        {
            Styles.ToolBar.fixedHeight = position.height;
            GUILayout.BeginArea(position);
                GUILayout.BeginHorizontal(Styles.ToolBar, GUILayout.Width(position.width));
                    Nodes.ForEach((n) => { n.OnGUI(); });
                GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public ToolBarTree Button(GUIContent content, Action<Rect> onClick, int width = 100)
        {
            ToolBarButton btn = new ToolBarButton(content, onClick, width);
            DockNode(btn);
            return this;
        }
        public ToolBarTree DropDownButton(GUIContent content, Action<Rect> onClick, int width = 100)
        {
            ToolBarDropDownButton btn = new ToolBarDropDownButton(content, onClick, width);
            DockNode(btn);
            return this;
        }
        public ToolBarTree Toggle(GUIContent content, Action<bool> onValueChange, bool value = true, int width = 100)
        {
            ToolBarToggle tog = new ToolBarToggle(content, onValueChange, value, width);
            DockNode(tog);
            return this;
        }
        public ToolBarTree SearchField(Action<string> onValueChange, string value = "", int width = 100)
        {
            ToolBarSearchField tog = new ToolBarSearchField(onValueChange, value, width);
            DockNode(tog);
            return this;
        }
        public ToolBarTree Label(GUIContent content, int width = 100)
        {
            ToolBarLabel la = new ToolBarLabel(content, width);
            DockNode(la);
            return this;
        }
        public ToolBarTree ToolTip(GUIContent content, int width = 100)
        {
            ToolBarToolTip la = new ToolBarToolTip(content, width);
            DockNode(la);
            return this;
        }
        public ToolBarTree Space(int width = 100)
        {
            ToolBarSpace sp = new ToolBarSpace(width);
            DockNode(sp);
            return this;
        }
        public ToolBarTree Delegate(Action<Rect> panDel,int width = 100)
        {
            DelegateLabel sp = new DelegateLabel(panDel,width);
            DockNode(sp);
            return this;
        }

        public ToolBarTree FlexibleSpace()
        {
            ToolBarFlexibleSpace sp = new ToolBarFlexibleSpace();
            DockNode(sp);
            return this;
        }
        public ToolBarTree DockNode(ToolbarNode node)
        {
            Nodes.Add(node);
            return this;
        }
    }
}
