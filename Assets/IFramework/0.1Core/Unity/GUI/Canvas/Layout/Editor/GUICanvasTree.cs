/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.14f1
 *Date:           2019-10-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace IFramework.GUIDesign.LayoutDesign
{

    public class GUICanvasTree
    {
        private class Trunk
        {
            private Element element;
            private GUICanvasTree m_tree;
            public GUICanvasTree tree
            {
                get
                {

                    if (m_tree == null && parent.tree != null)
                        m_tree = parent.tree;
                    return m_tree;
                }
                set
                {
                    m_tree = value;
                }
            }
            //private Trunk dragTrunk { get { return tree.dragTrunk; } set { tree.dragTrunk = value; } }

            private Trunk m_parent;
            public Trunk parent
            {
                get { return m_parent; }
                set
                {
                    if (m_parent == value) return;
                    if (m_parent != null)
                        m_parent.children.Remove(this);
                    if (value == null)
                        m_parent = value;
                    else
                    {
                        if (!value.element.GetType().IsSubclassOf(typeof(HaveChildElement)))
                        {
                            Log.E(" can't HaveChild");
                            return;
                        }
                        m_parent = value;
                        m_parent.children.Add(this);
                    }

                }

            }

            private List<Trunk> children;
            public Trunk(Trunk parent, Element element)
            {
                RenameLabel = new RenameLabelDrawer() {
                    value=element.name,
                    onEndEdit = (str) => { element.name = str.Trim(); }
                };
                this.element = element;
                this.m_parent = parent;
                children = new List<Trunk>();
                if (element.Children.Count > 0)
                {
                    for (int i = 0; i < element.Children.Count; i++)
                    {
                        children.Add(new Trunk(this, element.Children[i] as Element));
                    }
                }
            }

            public void OnTreeChange()
            {
                List<Trunk> tep = new List<Trunk>();
                tep.AddRange(children);
                children.Clear();
                for (int i = 0; i < element.Children.Count; i++)
                {
                   var trunk = tep.Find((t) => { return t.element == element.Children[i]; });
                    if (trunk == null)
                    {
                        children.Add(new Trunk(this, element.Children[i] as Element));
                    }
                    else
                    {
                        tep.Remove(trunk);
                        children.Add(trunk);
                    }
                }
                tep.ForEach((t) => { t.RenameLabel.Dispose(); });
                children.ForEach((t) => { t.OnTreeChange(); });
            }


            private bool foldOn = true;
            public static float LineHeight = 17;
            public float Height { get; private set; }
            public float CalcHeight()
            {
                if (!foldOn || children.Count == 0)
                {
                    Height = LineHeight;
                }
                else
                {
                    Height = LineHeight;
                    for (int i = 0; i < children.Count; i++)
                    {
                        Height += children[i].CalcHeight();
                    }
                }
                return Height;
            }
            private RenameLabelDrawer RenameLabel;
            public void OnGUI(Rect rect, Event e)
            {
                var rs = rect.HorizontalSplit(LineHeight);
                Rect selfRect = rs[0];
                if (ElementSelection.element == this.element && e.type == EventType.Repaint)
                    new GUIStyle("SelectionRect").Draw(selfRect, false, false, false, false);
                selfRect.xMin += 20 * element.depth;
                Rect childrenRect = rs[1];
                childrenRect.xMin += 20 * element.depth;

                if (children.Count > 0)
                {
                    var rss = selfRect.VerticalSplit(12);
                    foldOn = EditorGUI.Foldout(rss[0], foldOn, "", false);
                    RenameLabel.OnGUI(rss[1]);
                    //GUI.Label(rss[1], element.name);
                    if (tree.HandleEve)
                    {
                        Eve(rss[1], e);
                    }
                    if (!foldOn) return;
                    float y = 0;
                    for (int i = 0; i < children.Count; i++)
                    {
                        Rect r = new Rect(rect.x, childrenRect.y + y, rect.width, children[i].Height);
                        y += children[i].Height;
                        children[i].OnGUI(r, e);
                    }
                }
                else
                {
                    RenameLabel.OnGUI(selfRect);

                    if (tree.HandleEve)
                    {
                        Eve(selfRect, e);
                    }
                }
            }
            private void Eve(Rect r, Event e)
            {
                MouseDragEve(r, e);
                if (r.Contains(e.mousePosition) /*&& e.type == EventType.MouseDown */&& e.clickCount == 1 && e.button == 0) ElementSelection.element = this.element;
                if (r.Contains(e.mousePosition) && ElementSelection.element == this.element)
                {
                    if (e.type == EventType.KeyUp)
                    {
                        if (e.modifiers == EventModifiers.Control && e.keyCode == KeyCode.C)
                        {
                            OnCtrlC();
                            e.Use();
                        }
                        if (e.modifiers == EventModifiers.Control && e.keyCode == KeyCode.V && ElementSelection.copyElement != null)
                        {
                            OnCtrlV();
                            e.Use();
                        }
                        if (e.modifiers == EventModifiers.Control && e.keyCode == KeyCode.D)
                        {
                            OnCtrlD();
                            e.Use();
                        }
                        if (e.keyCode == KeyCode.Delete)
                        {
                            OnDelete();
                            e.Use();
                        }
                    }

                    if (e.button == 1 && e.clickCount == 1)
                    {
                        GenericMenu menu = new GenericMenu();
                        OnMenu(menu);
                        menu.ShowAsContext();
                        if (e.type!= EventType.Layout)
                        e.Use();
                    }
                }

            }
            private void MouseDragEve(Rect r, Event e)
            {

                bool CouldPutdown = r.Contains(e.mousePosition) && ElementSelection.dragElement != null && ElementSelection.dragElement != this.element;
                IElement tmp = this.element;
                while (tmp.parent != null)
                {
                    tmp = tmp.parent;
                    if (tmp == ElementSelection.dragElement)
                    {
                        CouldPutdown = false;
                        break;
                    }
                }
                if (CouldPutdown)
                    GUI.Box(r, "", this.element.GetType().IsSubclassOf(typeof(HaveChildElement)) && foldOn ? "SelectionRect" : "PR Insertion");
                if (CouldPutdown && e.type == EventType.MouseUp)
                {
                    if (this.element.GetType().IsSubclassOf(typeof(HaveChildElement)) && foldOn)
                    {
                        ElementSelection.dragElement.parent = this.element;
                        tree.OnTreeChange();
                    }
                    else
                    {
                        ElementSelection.dragElement.parent = this.element.parent;
                        element.parent.Children.Remove(ElementSelection.dragElement);
                        element.parent.Children.Insert(element.siblingIndex + 1, ElementSelection.dragElement);
                        tree.OnTreeChange();
                    }
                    ElementSelection.dragElement = null;
                }
                else if (ElementSelection.element == this.element)
                {
                    if (e.type == EventType.MouseDrag)
                    {
                        ElementSelection.dragElement = ElementSelection.element;
                    }
                    else if (e.type == EventType.MouseUp)
                    {
                        ElementSelection.dragElement = null;
                    }
                }


            }
            private void OnMenu(GenericMenu menu)
            {
                var types = typeof(Element).GetSubTypesInAssemblys()
                .ToList()
                .FindAll((type) => { return !type.IsAbstract && type.IsDefined(typeof(GUICreateMenuPathAttribute), false); });
                types.ForEach((type) =>
                {
                    string createPath = (type.GetCustomAttributes(typeof(GUICreateMenuPathAttribute), false).First() as GUICreateMenuPathAttribute).path;
                    menu.AddItem(new GUIContent("Create/" + createPath), false, () => { OnCeateElement(type); });
                });
                if (ElementSelection.copyElement == null)
                    menu.AddDisabledItem(new GUIContent("Paste"));
                else
                    menu.AddItem(new GUIContent("Paste"), false, OnCtrlV);
                menu.AddItem(new GUIContent("Reset"), false, element.Reset);
                if (!(element is GUICanvas))
                {
                    menu.AddItem(new GUIContent("Copy"), false, OnCtrlC);
                    menu.AddItem(new GUIContent("Duplicate"), false, OnCtrlD);
                    menu.AddItem(new GUIContent("Delete"), false, OnDelete);
                    if (element.siblingIndex == 0)
                        menu.AddDisabledItem(new GUIContent("MoveUp"));
                    else
                        menu.AddItem(new GUIContent("MoveUp"), false, OnMoveUp);
                    if (element.siblingIndex == element.parent.Children.Count - 1)
                        menu.AddDisabledItem(new GUIContent("MoveDown"));
                    else
                        menu.AddItem(new GUIContent("MoveDown"), false, OnMoveDown);
                }
            }

            private void OnCeateElement(Type type)
            {
                Element copy = Activator.CreateInstance(type) as Element;
                if (!this.element.GetType().IsSubclassOf(typeof(HaveChildElement)))
                {
                    copy.parent = this.element.parent;
                    tree.OnTreeChange();
                }
                else
                {
                    copy.parent = this.element;
                    tree.OnTreeChange();
                }
            }
            private void OnMoveUp()
            {
                int tmp = element.siblingIndex;
                if (tmp != 0)
                {
                    element.parent.Children.Remove(this.element);
                    element.parent.Children.Insert(tmp - 1, this.element);
                    tree.OnTreeChange();
                }
            }
            private void OnMoveDown()
            {
                int tmp = element.siblingIndex;
                if (tmp != element.parent.Children.Count - 1)
                {
                    element.parent.Children.Remove(this.element);
                    element.parent.Children.Insert(tmp + 1, this.element);
                    tree.OnTreeChange();
                }
            }
            protected virtual void OnCtrlC()
            {
                if (this.element.GetType() != typeof(GUICanvas))
                {
                    ElementSelection.copyElement = this.element;
                }
            }
            protected virtual void OnCtrlV()
            {
                if (ElementSelection.copyElement == null) return;
                Element copy = Activator.CreateInstance(ElementSelection.copyElement.GetType(), ElementSelection.copyElement) as Element;
                if (this.element.GetType() != typeof(GUICanvas))
                {
                    copy.parent = this.element.parent;
                    tree.OnTreeChange();
                }
                else
                {
                    copy.parent = this.element;
                    tree.OnTreeChange();
                }

                ElementSelection.copyElement = null;
            }
            protected virtual void OnCtrlD()
            {
                if (this.element.GetType() == typeof(GUICanvas)) return;
                Element copy = Activator.CreateInstance(this.element.GetType(), this.element) as Element;
                copy.parent = this.element.parent;
                tree.OnTreeChange();
                ElementSelection.copyElement = null;
            }
            protected virtual void OnDelete()
            {
                if (this.element.GetType() == typeof(GUICanvas)) return;
                element.parent = null; ElementSelection.element = null;
                tree.OnTreeChange();
            }
        }
        //private Trunk dragTrunk;
        public bool HandleEve=true;
        private GUICanvas m_canvas;
        public GUICanvas canvas { get { return m_canvas; } set { m_canvas = value; root = new Trunk(null, m_canvas); root.tree = this; } }
        private Trunk root;
        private Vector2 scroll;
        private void OnTreeChange()
        {
            root .OnTreeChange();
            if (onTreeChange != null)
                onTreeChange();
        }
        public event Action onTreeChange; 
        public void OnGUI(Rect rect)
        {
            if (root == null) return;
            Event e = Event.current;
            if (!rect.Contains(Event.current.mousePosition))
                ElementSelection.dragElement = null;
            root.CalcHeight();
            var rs = rect.HorizontalSplit(root.Height);
            scroll = GUI.BeginScrollView(rect, scroll, rs[0]);
            root.OnGUI(rs[0], e);
            GUI.EndScrollView();
            if (HandleEve)
            {
                EmptyEve(e, rs[1]);
            }
        }
        private void EmptyEve(Event e,Rect r)
        {
            //r.DrawOutLine(12, Color.red);
            if (r.height>0 &&r.Contains(e.mousePosition) && e.type== EventType.MouseUp)
            {
                //dragTrunk = null;
                ElementSelection.element = null;
                ElementSelection.dragElement = null;
            }
        }
       
    }
}
