/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-11-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Xml;
using UnityEditor;
using UnityEngine;
namespace IFramework.GUIDesign.RectDesign
{
    [EditorWindowCache("GUI Design Canvas.Rect")]
    public class GUIDesignCanvas:EditorWindow
	{
        [SerializeField]
        private string subwinXmlStr;
        private const float ToolBarHeight = 22;
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
                            .Toggle(new GUIContent("Title"), (bo) => { WinTree.isShowTitle = bo; }, WinTree.isShowTitle, 60)
                            .Toggle(new GUIContent("Lock"), (bo) => { WinTree.isLocked = bo; }, WinTree.isLocked, 60)
                            .FlexibleSpace()
                             .Delegate((r) =>
                             {
                                 editType = (EditType)GUILayout.Toolbar((int)editType, System.Enum.GetNames(typeof(EditType)));
                             }, 0)
                            .Delegate((r) =>
                            {
                                GUI.Label(r, new GUIContent(XmlPath, "XmlPath:  " + XmlPath));
                                r.DrawOutLine(2, Color.black);

                                Event e = Event.current;
                                if (!r.Contains(e.mousePosition)) return;

                                if (!string.IsNullOrEmpty(XmlPath) && e.clickCount == 2)
                                {
                                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(XmlPath);
                                }
                                var info = DragAndDropUtil.Drag(e, r);
                                if (info.paths.Length == 1 && info.EnterArera && info.Finsh && info.paths[0].EndsWith(".xml") && System.IO.File.Exists(info.paths[0]))
                                    XmlPath = info.paths[0];

                            }, 200)
                            .Button(new GUIContent("Save"), (rect) =>
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
                             .Button(new GUIContent("Load"), (rect) =>
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
                                 scene.canvas = guiCanvas;
                             }, 60);

        }
        private enum EditType
        {
             Design=0,Result=1
        }

        private EditType editType;
        private GUICanvasTree designTree = new GUICanvasTree();
        private ElementDesignView design = new ElementDesignView();
        private ElementSceneView scene = new ElementSceneView();

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

            switch (editType)
            {
                case EditType.Design:
                    scene.OnGUI(rect);
                    break;
                case EditType.Result:
                    guiCanvas.canvasRect = rect;
                    guiCanvas.OnGUI();
                    break;
            }
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
                guiCanvas = new GUICanvas() { canvasRect = new Rect(300, 300, 300, 300) }
                         .Element(new TextLabel() { name = "H", text = "123" });

            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(tmpDesign);
                guiCanvas = new GUICanvas();
                guiCanvas.DeSerialize(doc.FirstChild as XmlElement);
            }
            designTree.canvas = guiCanvas;
            scene.canvas = guiCanvas;
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
