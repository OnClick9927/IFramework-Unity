/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Xml;

namespace IFramework.Example
{
    [EditorWindowCache("SubWin Exampe")]
    public class SubWin : EditorWindow
    {
        [SerializeField]
        private string xmlStr;
        [SerializeField]
        private List<SubWinTree.TreeLeaf> closeLeafs = new List<SubWinTree.TreeLeaf>();
        [SerializeField]
        private List<SubWinTree.TreeLeaf> Leafs = new List<SubWinTree.TreeLeaf>();
        private const float ToolBarHeight = 17;
        private Rect localPosition { get { return new Rect(Vector2.zero, position.size); } }
        private SubWinTree WinTree;
        private ToolBarTree ToolBarTree;
        private string XmlPath;
        private void OnDisable()
        {
            xmlStr = WinTree.Serialize();
        }     
        private void OnEnable()
        {
            CheckXML();
            Init();
        }
        private void CheckXML()
        {
            XmlPath = FrameworkConfig.UnityCorePath.CombinePath("GUI/SubWindow/Trees.xml");
            if (!System.IO.File.Exists(XmlPath))
            {
                XmlDocument doc = new XmlDocument();
                XmlElement root = doc.CreateElement("Trees");
                doc.AppendChild(root);
                doc.Save(XmlPath);
                AssetDatabase.Refresh();
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(XmlPath);
                XmlElement root = doc.SelectSingleNode("Trees") as XmlElement;
                if (root == null)
                {
                    root = doc.CreateElement("Trees");
                    doc.AppendChild(root);
                    doc.Save(XmlPath);
                    AssetDatabase.Refresh();
                }
            }
        }
        private void Init()
        {
            WinTree = new SubWinTree();
            WinTree.repaintEve = Repaint;
            WinTree.drawCursorEve = (rect, sp) =>
            {
                if (sp == SplitType.Vertical)
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
                else
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);
            };

            WinTree.onCreatLeaf = (leaf) =>
            {
                leaf.titleContent = new GUIContent((Leafs.Count + 1).ToString());
                leaf.paintDelegate = PP;
                Leafs.Add(leaf);
            };
            WinTree.onDockLeaf = (leaf) => { closeLeafs.Remove(leaf); };
            WinTree.onCloseLeaf = (leaf) => { closeLeafs.Add(leaf); };
            WinTree.onClearLeaf = (leaf) =>
            {
                Leafs.Remove(leaf);
                if (closeLeafs.Contains(leaf))
                    closeLeafs.Remove(leaf);
            };
            if (!string.IsNullOrEmpty(xmlStr))
                WinTree.DeSerialize(xmlStr);
            else
            for (int i = 1; i <= 5; i++)
            {
                SubWinTree.TreeLeaf L = WinTree.CreateLeaf(new GUIContent(i.ToString()));
                if (i==5) L.minSize = new Vector2(250, 300);
               WinTree.DockLeaf(L,(SubWinTree.DockType)(i%4 ));
            }
            ToolBarTree = new ToolBarTree();
            ToolBarTree.DropDownButton(new GUIContent("Views"),Views,60)
                            .FlexibleSpace()
                            .Toggle(new GUIContent("ShowTitle"), (bo) => { WinTree.isShowTitle =bo; }, WinTree.isShowTitle,60)
                            .Toggle(new GUIContent("Lock"), (bo) => { WinTree.isLocked = bo; }, WinTree.isLocked, 60)

                            .Space(10)
                            .DropDownButton(new GUIContent("Layout"), Layout,60);
        }
        private void Layout(Rect rect)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(XmlPath);
            XmlElement root = doc.DocumentElement;
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                XmlElement e = root.ChildNodes[i] as XmlElement;
                menu.AddItem(new GUIContent(e.GetAttribute("Name")), false, () => {
                    WinTree.DeSerialize(e);
                });
            }
            menu.AddItem(new GUIContent("Save"), false, () => {
                LayoutSavePop pop = LayoutSavePop.CreateInstance<LayoutSavePop>();
                pop.Tree = WinTree;
                pop.path = XmlPath;
                pop.element = root;
                pop.ShowAuxWindow();
            });
            menu.AddItem(new GUIContent("Delete"), false, () => {
                LayoutEditPop pop = LayoutEditPop.CreateInstance<LayoutEditPop>();
                pop.path = XmlPath;
                pop.root = root;
                pop.ShowAuxWindow();
            });
            menu.DropDown(rect);
            Event.current.Use();
        }
        private void Views(Rect rect)
        {
            GenericMenu menu = new GenericMenu();
            //for (int i = 0; i < Leafs.Count; i++)
            //{
            //    SubWinTree.TreeLeaf leaf = Leafs[i];
            //    menu.AddItem(leaf.titleContent, !closeLeafs.Contains(leaf), () => {

            //        if (closeLeafs.Contains(leaf))
            //            WinTree.DockLeaf(leaf, SubWinTree.DockType.Left);
            //        else
            //            WinTree.CloseLeaf(leaf);
            //    });
            //}
            for (int i = 0; i < WinTree.allLeafCount; i++)
            {
                SubWinTree.TreeLeaf leaf = WinTree.allLeafs[i];
                menu.AddItem(leaf.titleContent, !WinTree.closedLeafs.Contains(leaf), () => {
                    if (WinTree.closedLeafs.Contains(leaf))
                        WinTree.DockLeaf(leaf, SubWinTree.DockType.Left);
                    else
                        WinTree.CloseLeaf(leaf);
                });
            }
            menu.DropDown(rect);
            Event.current.Use();
        }
        private class LayoutSavePop : EditorWindow
        {
            public SubWinTree Tree;
            public XmlElement element;
            public string Name;
            public string path;

            public  void OnGUI()
            {
                Name = GUILayout.TextField(Name);
                if (GUILayout.Button("Save"))
                {
                    if (!Name.IsNullOrEmpty())
                    {
                        element.AppendChild(Tree.Serialize(element.OwnerDocument,Name));
                        element.OwnerDocument.Save(path);
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        ShowNotification(new GUIContent("Name isNull"));
                    }

                }
            }
        }
        private class LayoutEditPop : EditorWindow
        {
            public XmlElement root;
            public string path;
            public void OnGUI()
            {
                GUILayout.BeginArea(new Rect(Vector2.zero, position.size));
                for (int i = root.ChildNodes.Count-1; i >=0 ; i--)
                {
                    XmlElement e = root.ChildNodes[i] as XmlElement;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(e.GetAttribute("Name"));
                    if (GUILayout.Button("Delete",GUILayout.Width(60)))
                    {
                        root.RemoveChild(e);
                        root.OwnerDocument.Save(path);
                        AssetDatabase.Refresh();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }
        }

        private void DrawToolBar(Rect rect)
        {
            ToolBarTree.OnGUI(rect);
        }
        private void DrawWindows(Rect rect)
        {
            WinTree.OnGUI(rect);
            this.minSize = WinTree.minSize + new Vector2(0, ToolBarHeight);
        }
        private void OnGUI()
        {
            var rs = localPosition.Zoom(AnchorType.MiddleCenter, -2).HorizontalSplit(ToolBarHeight,4);
            DrawWindows(rs[1]);
            DrawToolBar(rs[0]);
        }


        private void PP(Rect rect)
        {
            rect.DrawOutLine(2, Color.black);
            GUILayout.BeginArea(rect);
            GUILayout.Label("绘制区域");
            GUILayout.EndArea();

        }


      

    }
}
