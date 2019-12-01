/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-10-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Xml;
using UnityEditor;
using UnityEngine;
namespace IFramework.GUITool.LayoutDesign
{
    [EditorWindowCache("GUITool.Canvas.Layout")]
    partial class GUIDesignWindow
    {
        private enum EditType
        {
            Design = 0, Result = 1
        }
        [SerializeField]
        private string tmpSubWinLayout;
        private const float ToolBarHeight = 22;
        private Rect localPosition { get { return new Rect(Vector2.zero, position.size); } }
        private SubWinTree subWin;
        private ToolBarTree titleToolBar;
        private EditType editType;
        private GUICanvasHierarchyTree HierarchyView = new GUICanvasHierarchyTree();
        private GUIElementInspectorView InspectorView = new GUIElementInspectorView();
        private GUIElementSceneView SceneView = new GUIElementSceneView();
        private string XmlPath;
        [SerializeField]
        private string tmpDesign;
        private GUICanvas guiCanvas;
    }
    partial class GUIDesignWindow : EditorWindow, ILayoutGUIDrawer
    {
        private const string SceneViewName = "Canvas";
        private const string InspectorViewName = "Design";
        private const string HierarchyViewName = "Tree";

        private void SubwinInit()
        {
            subWin = new SubWinTree();
            subWin.repaintEve += Repaint;
            subWin.drawCursorEve += (rect, sp) =>
            {
                if (sp == SplitType.Vertical)
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
                else
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);
            };
            if (string.IsNullOrEmpty(tmpSubWinLayout))
            {
                for (int i = 1; i <= 3; i++)
                {
                    string userdata = i == 1 ? InspectorViewName : i == 2 ? SceneViewName : HierarchyViewName;
                    SubWinTree.TreeLeaf L = subWin.CreateLeaf(new GUIContent(userdata));
                    L.userData = userdata;
                    subWin.DockLeaf(L, SubWinTree.DockType.Left);
                }
            }
            else
            {
                subWin.DeSerialize(tmpSubWinLayout);

            }
            subWin[InspectorViewName].titleContent = new GUIContent(InspectorViewName);
            subWin[InspectorViewName].paintDelegate += DesignGUI;

            subWin[SceneViewName].titleContent = new GUIContent(SceneViewName);
            subWin[SceneViewName].paintDelegate += CanvasGUI;

            subWin[HierarchyViewName].titleContent = new GUIContent(HierarchyViewName);
            subWin[HierarchyViewName].paintDelegate += TreeGUI;


            titleToolBar = new ToolBarTree();
            titleToolBar.DropDownButton(new GUIContent("Views"), (rect) => {
                GenericMenu menu = new GenericMenu();

                for (int i = 0; i < subWin.allLeafCount; i++)
                {
                    SubWinTree.TreeLeaf leaf = subWin.allLeafs[i];
                    menu.AddItem(leaf.titleContent, !subWin.closedLeafs.Contains(leaf), () => {
                        if (subWin.closedLeafs.Contains(leaf))
                            subWin.DockLeaf(leaf, SubWinTree.DockType.Left);
                        else
                            subWin.CloseLeaf(leaf);
                    });
                }
                menu.DropDown(rect);
                Event.current.Use();

            }, 50)
                        .Toggle(new GUIContent("Title"), (bo) => { subWin.isShowTitle = bo; }, subWin.isShowTitle, 50)
                        .Toggle(new GUIContent("Lock"), (bo) => { subWin.isLocked = bo; }, subWin.isLocked, 50)
                        .FlexibleSpace()

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
                                ShowNotification(new GUIContent("Please Set Legal XmlPath"));
                                return;
                            }
                            XmlDocument doc = new XmlDocument();
                            doc.AppendChild(guiCanvas.Serialize(doc));
                            doc.Save(XmlPath);
                            AssetDatabase.Refresh();
                        }, 50)
                        .Button(new GUIContent("Load"), (rect) =>
                        {
                            if (string.IsNullOrEmpty(XmlPath) || !System.IO.File.Exists(XmlPath))
                            {
                                ShowNotification(new GUIContent("Please Set Legal XmlPath"));
                                return;
                            }
                            XmlDocument doc = new XmlDocument();
                            doc.Load(XmlPath);
                            guiCanvas.DeSerialize(doc.DocumentElement);
                            HierarchyView.canvas = guiCanvas;
                            SceneView.canvas = guiCanvas;
                        }, 50);



        }
        private class CanvasSetting : PopupWindowContent, ILayoutGUIDrawer
        {
            public GUIDesignWindow window;
            public override void OnGUI(Rect rect)
            {
                if (window == null) return;
                this.RectField("Canvas Rect", ref window.guiCanvas.canvasRect)
                    .FloatField("zoomDelta", ref window.zoomDelta)
                    .FloatField("minZoom", ref window.minZoom)
                    .FloatField("maxZoom", ref window.maxZoom)
                    .Pan(() => {
                        window.ZoomScale = EditorGUILayout.FloatField("ZoomScale", window.ZoomScale);
                    })
                    .FloatField("panSpeed", ref window.panSpeed)
                    .Vector2Field("panOffset", ref window.panOffset)
                                            .FlexibleSpace()

                    .BeginHorizontal()
                        .FlexibleSpace()
                        .Button(() => {
                            window.zoomDelta = 0.01f;
                            window.minZoom = 1f;
                            window.maxZoom = 8f;
                            window.panSpeed = 1.2f;
                            window.ZoomScale = 1f;
                            window.panOffset = Vector2.zero;
                        }, "Reset")
                    .EndHorizontal();
            }
        }

        private void TreeGUI(Rect rect)
        {
            HierarchyView.OnCanvasTreeGUI(rect);
        }
        private void DesignGUI(Rect rect)
        {
            InspectorView.OnGUI(rect);
        }


        private float zoomDelta = 0.01f;
        private float minZoom = 1f;
        private float maxZoom = 8f;
        private float ZoomScale
        {
            get { return _zoom.x; }
            set
            {
                float z = Mathf.Clamp(value, minZoom, maxZoom);
                _zoom.Set(z, z);
            }
        }
        private float panSpeed = 1.2f;
        private Vector2 panOffset = Vector2.zero;

        private Vector2 _zoomAdjustment;
        private Vector2 _zoom = Vector2.one;
        private Vector2 GraphToScreenSpace(Vector2 graphPos)
        {
            return graphPos + _zoomAdjustment + panOffset;
        }
        private void Pan(Vector2 delta)
        {
            panOffset += delta * ZoomScale * panSpeed;
        }
        private void Zoom(float zoomDirection)
        {
            float scale = (zoomDirection < 0f) ? (1f - zoomDelta) : (1f + zoomDelta);
            _zoom *= scale;
            float cap = Mathf.Clamp(_zoom.x, minZoom, maxZoom);
            _zoom.Set(cap, cap);
        }
        private void DrawAxes(Rect rect, Color color, float width = 2f)
        {
            Vector2 down = Vector2.up * rect.height * ZoomScale;
            Vector2 right = Vector2.right * rect.width * ZoomScale;
            Vector2 up = -down;
            Vector2 left = -right;
            up = GraphToScreenSpace(up);
            down = GraphToScreenSpace(down);
            right = GraphToScreenSpace(right);
            left = GraphToScreenSpace(left);
            DrawLine(right, left, color, width);
            DrawLine(up, down, color, width);
        }

        public void DrawLine(Vector2 start, Vector2 end, Color color, float width = 2f)
        {
            var handleColor = Handles.color;
            Handles.color = color;
            Handles.DrawAAPolyLine(width, start, end);
            Handles.color = handleColor;
        }

        private ToolBarTree CanvasToolBar = new ToolBarTree();
        private void CanvasGUI(Rect rect)
        {
            var rs = rect.HorizontalSplit(ToolBarHeight, 1);
            Rect graphRect = new Rect(new Vector2(5, ToolBarHeight * 1.2f), rs[1].size);
            var center = graphRect.size / 2f;
            _zoomAdjustment = GUIScaleUtility.BeginScale(ref graphRect, center, ZoomScale, false);
            graphRect.position = GraphToScreenSpace(guiCanvas.position.position);
            DrawAxes(graphRect, Color.grey, 10);
            GUI.BeginClip(graphRect);
            switch (editType)
            {
                case EditType.Design:
                    SceneView.OnGUI(rect);
                    break;
                case EditType.Result:
                    guiCanvas.OnGUI();
                    break;
            }
            GUI.EndClip();
            GUIScaleUtility.EndScale();
            if (Event.current.type == EventType.ScrollWheel && rs[1].Contains(Event.current.mousePosition))
            {
                Zoom(Event.current.delta.y);
                Repaint();
            }
            if (Event.current.type == EventType.MouseDrag && rs[1].Contains(Event.current.mousePosition))
            {
                Pan(Event.current.delta);
                Repaint();
            }
            CanvasToolBar.OnGUI(rs[0].Zoom(AnchorType.LowerCenter, new Vector2(8, 2)).MoveUp(2));
        }


        private void GUICanvasEditorViewInit()
        {
            if (string.IsNullOrEmpty(tmpDesign))
            {
                guiCanvas = new GUICanvas() { canvasRect = new Rect(0, 0, 500, 500) }
                         .Element(new Button() { name = "Button", text = "123" });
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(tmpDesign);
                guiCanvas = new GUICanvas();
                guiCanvas.DeSerialize(doc.FirstChild as XmlElement);
            }
            HierarchyView.canvas = guiCanvas;
            SceneView.canvas = guiCanvas;
            subWin.onDragWindow += () => { HierarchyView.HandleEve = false; };
            subWin.onResizeWindow += () => { HierarchyView.HandleEve = false; };
            subWin.onEndDragWindow += () => { HierarchyView.HandleEve = true; };
            subWin.onEndResizeWindow += () => { HierarchyView.HandleEve = true; };
            CanvasToolBar
                .Delegate((r) =>
                {
                    editType = (EditType)GUILayout.Toolbar((int)editType, System.Enum.GetNames(typeof(EditType)));
                }, 0)
                .FlexibleSpace()
                .Button(new GUIContent("ReCeter"), (rec) =>
                {
                    panOffset = -guiCanvas.canvasRect.center;
                }, 50)
                .Button(new GUIContent("Setting"), (Rect rect) => {
                    PopupWindow.Show(rect, new CanvasSetting() { window = this });
                }, 50);
        }

        private void OnEnable()
        {
            SubwinInit();
            GUICanvasEditorViewInit();

            GUIScaleUtility.CheckInit();
            this.titleContent = new GUIContent("LayoutCanvas");
        }


        private void SaveTmpInfo()
        {
            tmpSubWinLayout = subWin.Serialize();
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(guiCanvas.Serialize(doc));
            tmpDesign = doc.InnerXml;
        }
        private void OnDisable()
        {
            SaveTmpInfo();
        }


        private void OnGUI()
        {
            var rs = localPosition.Zoom(AnchorType.MiddleCenter, -2).HorizontalSplit(ToolBarHeight, 4);
            titleToolBar.OnGUI(rs[0]);
            subWin.OnGUI(rs[1]);
            this.minSize = subWin.minSize + new Vector2(0, ToolBarHeight);
            if (UnityEditor.SceneView.lastActiveSceneView != null) UnityEditor.SceneView.lastActiveSceneView.sceneViewState.Toggle(true);
            Repaint();
        }
    }


}
