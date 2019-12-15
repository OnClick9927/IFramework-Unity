/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    Description的
 *History:        2019-12-07--
*********************************************************************************/
using System.Xml;
using IFramework;
using IFramework.GUITool;
using UnityEditor;
using UnityEngine;

namespace IFramework_Demo
{
    [EditorWindowCache("IFramework_Demo.SubWin Exampe")]
    public partial class SubWinExample: EditorWindow,ILayoutGUIDrawer
    {
        [SerializeField]
        private string tmpLayout;           //暂时布局，貌似以字符方式存储。
        private const float ToolBarHeight = 17;         //常量 工具条的高度
        private Rect localPosition { get { return new Rect(Vector2.zero, position.size); } }    // 根据当前窗口的大小，和原点构建一 Rect
        private SubWinTree WinTree;                     
        private ToolBarTree ToolBarTree;
        private string XmlPath= "Assets/Examples/Editor/SubWin/Trees.xml";
    }
    public partial class SubWinExample 
    {
        private void CheckLayoutXML()
        {
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

        private void OnEnable()
        {
            CheckLayoutXML();
            Init();
        }
        private void Init()
        {
            WinTree = new SubWinTree();
            WinTree.repaintEve += Repaint;
            WinTree.drawCursorEve += (rect, sp) =>
            {
                if (sp == SplitType.Vertical)
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
                else
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);
            };

            if (!string.IsNullOrEmpty(tmpLayout))
                WinTree.DeSerialize(tmpLayout);
            else
                for (int i = 1; i <= 5; i++)
                {
                    SubWinTree.TreeLeaf L = WinTree.CreateLeaf(new GUIContent(i.ToString()));
                    L.userData = i.ToString();
                    WinTree.DockLeaf(L, (SubWinTree.DockType)(i % 4));
                }
            for (int i = 1; i <= 5; i++)
            {
                WinTree[i.ToString()].titleContent = new GUIContent(i.ToString());
                WinTree[i.ToString()].paintDelegate += (rect)=> {
                    rect.DrawOutLine(2, Color.black);
                    this.BeginArea(rect)
                            .Label("绘制区域")
                        .EndArea();
                };
            }
            ToolBarTree = new ToolBarTree();
            ToolBarTree.DropDownButton(new GUIContent("Views"), (rect)=> {
                                GenericMenu menu = new GenericMenu();
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
                            }, 60)
                            .Toggle(new GUIContent("Title"), (bo) => { WinTree.isShowTitle = bo; }, WinTree.isShowTitle, 60)
                            .Toggle(new GUIContent("Lock"), (bo) => { WinTree.isLocked = bo; }, WinTree.isLocked, 60)
                            .FlexibleSpace()
                            .Space(10)
                            .DropDownButton(new GUIContent("Layout"), Layout, 60);
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

        private class LayoutSavePop : EditorWindow, ILayoutGUIDrawer
        {
            public SubWinTree Tree;
            public XmlElement element;
            public string Name;
            public string path;

            public void OnGUI()
            {

                this.TextField(ref Name)
                    .Button(()=> {
                        if (!Name.IsNullOrEmpty())
                        {
                            element.AppendChild(Tree.Serialize(element.OwnerDocument, Name));
                            element.OwnerDocument.Save(path);
                            AssetDatabase.Refresh();
                        }
                        else
                        {
                            ShowNotification(new GUIContent("Name isNull"));
                        }
                    },"Save");
            }
        }
        private class LayoutEditPop : EditorWindow, ILayoutGUIDrawer
        {
            public XmlElement root;
            public string path;
            public void OnGUI()
            {
                this.DrawArea(() =>
                {
                    for (int i = root.ChildNodes.Count - 1; i >= 0; i--)
                    {
                        XmlElement e = root.ChildNodes[i] as XmlElement;

                        this.BeginHorizontal()
                                .Label(e.GetAttribute("Name"))
                                .Button(() => {
                                    root.RemoveChild(e);
                                    root.OwnerDocument.Save(path);
                                    AssetDatabase.Refresh();
                                }, "Delete")
                            .EndHorizontal();
                    }
                }, new Rect(Vector2.zero, position.size));

            }
        }

        private void OnDisable()
        {
            tmpLayout = WinTree.Serialize();
        }


        private void OnGUI()
        {
            var rs = localPosition.Zoom(AnchorType.MiddleCenter, -2).HorizontalSplit(ToolBarHeight, 4);
            DrawWindows(rs[1]);
            DrawToolBar(rs[0]);
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
    }
}
