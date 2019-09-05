/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-10-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Xml;
using UnityEditor;
using UnityEngine;
namespace IFramework.GUIDesign.LayoutDesign
{
    [EditorWindowCache("GUI Design Canvas.Layout")]
    public class GUIDesignCanvas : EditorWindow
    {
        [SerializeField]
        private string subwinXmlStr;
        private const float ToolBarHeight = 17;
        private Rect localPosition { get { return new Rect(Vector2.zero, position.size); } }
        private SubWinTree WinTree;
        private ToolBarTree ToolBarTree;

        private void Views(Rect rect)
        {
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
        }
        private void SubwinInit()
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
            if (!string.IsNullOrEmpty(subwinXmlStr))
            {
                WinTree.DeSerialize(subwinXmlStr);
                for (int i = 0; i < WinTree.allLeafCount; i++)
                {
                    var leaf = WinTree.allLeafs[i];
                    if (leaf.userData == "Design")
                    {
                        leaf.titleContent = new GUIContent("Design");
                        leaf.paintDelegate = DesignGUI;
                    }
                    if (leaf.userData == "Canvas")
                    {
                        leaf.titleContent = new GUIContent("Canvas");
                        leaf.paintDelegate = CanvasGUI;
                    }
                    if (leaf.userData == "Tree")
                    {
                        leaf.titleContent = new GUIContent("Tree");
                        leaf.paintDelegate = TreeGUI;
                    }
                }
            }
            else
                for (int i = 1; i <= 3; i++)
                {
                    string userdata = i == 1 ? "Design" : i == 2 ? "Canvas" : "Tree";
                    SubWinTree.TreeLeaf L = WinTree.CreateLeaf(new GUIContent(userdata));
                    L.userData = userdata;
                    if (i == 1)
                        L.paintDelegate = DesignGUI;
                    if (i == 2)
                        L.paintDelegate = CanvasGUI;
                    if (i == 3)
                        L.paintDelegate = TreeGUI;
                    WinTree.DockLeaf(L, SubWinTree.DockType.Left);
                }
            ToolBarTree = new ToolBarTree();
            ToolBarTree.DropDownButton(new GUIContent("Views"), Views, 60)
                            .FlexibleSpace()
                            .Delegate((r) =>
                            {
                                GUI.Label(r, new GUIContent(XmlPath, "XmlPath:  " + XmlPath));
                                r.DrawOutLine(2, Color.black);

                                Event e = Event.current;
                                if (!r.Contains(e.mousePosition)) return;

                                if (!string.IsNullOrEmpty(XmlPath)&&    e.clickCount==2)
                                {
                                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(XmlPath);
                                }
                                var info = DragAndDropUtil.Drag(e, r);
                                if (info.paths.Length == 1 && info.EnterArera && info.Finsh && info.paths[0].EndsWith(".xml") && System.IO.File.Exists(info.paths[0]))
                                    XmlPath = info.paths[0];

                            }, 200)
                            .Button(new GUIContent("Ser"), (rect) =>
                            {
                                if (string.IsNullOrEmpty(XmlPath) || !System.IO.File.Exists(XmlPath))
                                {
                                    ShowNotification(new GUIContent("Please check XmlPath"));
                                    return;
                                }
                                XmlDocument doc = new XmlDocument();
                                doc.AppendChild(guiCanvas.Serialize(doc));
                                doc.Save(XmlPath);
                                AssetDatabase.Refresh();
                            }, 60)
                             .Button(new GUIContent("DeSer"), (rect) =>
                             {
                                 if (string.IsNullOrEmpty(XmlPath) || !System.IO.File.Exists(XmlPath))
                                 {
                                     ShowNotification(new GUIContent("Please check XmlPath"));
                                     return;
                                 }
                                 XmlDocument doc = new XmlDocument();
                                 doc.Load(XmlPath);
                                 guiCanvas.DeSerialize(doc.DocumentElement);
                                 designTree.canvas = guiCanvas;
                             }, 60)
                            .Toggle(new GUIContent("ShowTitle"), (bo) => { WinTree.isShowTitle = bo; }, WinTree.isShowTitle, 60)
                            .Toggle(new GUIContent("Lock"), (bo) => { WinTree.isLocked = bo; }, WinTree.isLocked, 60);

        }

        private GUICanvasTree designTree = new GUICanvasTree();
        private ElementDesignView design = new ElementDesignView();
        private void TreeGUI(Rect rect)
        {
            designTree.OnGUI(rect);
        }
        private void DesignGUI(Rect rect)
        {
            design.OnGUI(rect);
        }
        private void CanvasGUI(Rect rect)
        {
            guiCanvas.CanvasRect = rect;
            guiCanvas.OnGUI();
        }

        private string XmlPath;
        [SerializeField]
        string tmpDesign;
        GUICanvas guiCanvas;
        private void OnDisable()
        {
            subwinXmlStr = WinTree.Serialize();
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(guiCanvas.Serialize(doc));
            tmpDesign = doc.InnerXml;
        }
        private void OnEnable()
        {
            if (string.IsNullOrEmpty(tmpDesign))
            {
                guiCanvas = new GUICanvas() { CanvasRect = new Rect(300, 300, 300, 300) }
                         .Element(new Horizontal() { name = "H", }
                                 .Element(new TextLabel() { name = "txt", text = "725", fontSize = 30 })
                                 .Element(new Space() { pixels = 10 })
                                 .Element(new TextArea() { name = "zzz", text = "<color=blue>666</color>", fontSize = 50 })
                                 .Element(new Space() { pixels = 10 })
                                 .Element(new TextField() { name = "zzz", text = "666", fontSize = 50 })
                                 )
                         .Element(new Button() { name = "ChangeColor", text = "ChangeColor" })
                         .Element(new Button() { name = "ChangeRotation", text = "ChangeRotation" });
                Button ChangeRotation = guiCanvas.Find<Button>("ChangeRotation");
                Button ChangeColor = guiCanvas.Find<Button>("ChangeColor");
                ChangeColor.OnClick = () =>
                {
                    ChangeColor.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
                };
                ChangeRotation.OnClick = () =>
                {
                    ChangeRotation.rotateAngle = Random.Range(0, 360f);
                };
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(tmpDesign);
                guiCanvas = new GUICanvas();
                guiCanvas.DeSerialize(doc.FirstChild as XmlElement);
            }
            designTree.canvas = guiCanvas;
            SubwinInit();
            WinTree.onDragWindow += () => { designTree.HandleEve = false; };
            WinTree.onResizeWindow += () => { designTree.HandleEve = false; };
            WinTree.onEndDragWindow += () => { designTree.HandleEve = true; };
            WinTree.onEndResizeWindow += () => { designTree.HandleEve = true; };

        }

        private void OnGUI()
        {
            var rs = localPosition.Zoom(AnchorType.MiddleCenter, -2).HorizontalSplit(ToolBarHeight, 4);
            ToolBarTree.OnGUI(rs[0]);
            WinTree.OnGUI(rs[1]);
            this.minSize = WinTree.minSize + new Vector2(0, ToolBarHeight);
            if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.sceneViewState.Toggle(true);
            Repaint();

        }

    }

}
