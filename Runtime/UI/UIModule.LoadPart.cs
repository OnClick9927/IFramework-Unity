/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace IFramework.UI
{
    partial class UIModule
    {
        private class LoadPart
        {
            private bool _loading = false;
            private UIModule module;
            private Queue<LoadPanelAsyncOperation> asyncLoadQueue;
            private Dictionary<string, UIPanel> panels = new Dictionary<string, UIPanel>();
            public Canvas canvas { get; private set; }

            public IEnumerable<string> GetLoadedPanelPaths() => panels.Keys;
            public void RemovePanel(string path)
            {

                if (panels.TryGetValue(path, out var panel))
                {
                    module.assetPart.DestroyPanel(panel.gameObject);
                    panels.Remove(path);
                }
            }
            public LoadPart(UIModule module)
            {
                this.module = module;
                asyncLoadQueue = new Queue<LoadPanelAsyncOperation>();
                panels = new Dictionary<string, UIPanel>();
            }
            public UIPanel Find(string path)
            {
                UIPanel ui;
                panels.TryGetValue(path, out ui);
                return ui;
            }

            public void LoadPanel(string path, int layer, ShowPanelAsyncOperation show_op)
            {
                var panel = Find(path);

                if (panel != null)
                    OnShowCallBack(true, path, panel, show_op);
                else
                {
                    RectTransform parent = module.GetRTLayerData(module.GetLayerName(layer)).rect;
                    var result = module.assetPart.LoadPanel(parent, path);
                    if (result != null)
                        UILoadComplete(result, path, show_op);
                    else
                    {
                        LoadPanelAsyncOperation op = new LoadPanelAsyncOperation();
                        op.path = path;
                        op.parent = parent;
                        op.show = show_op;
                        if (module.assetPart.LoadPanelAsync(op))
                        {
                            _loading = true;
                            asyncLoadQueue.Enqueue(op);
                        }
                        else
                            throw new Exception($"Can't load ui with Name: {path}");
                    }
                }
            }
            private void UILoadComplete(UIPanel ui, string path, ShowPanelAsyncOperation op)
            {
                if (ui != null) panels.Add(path, ui);
                module.UILoadComplete(ui, path, op);
            }
            private void OnShowCallBack(bool exist, string path, UIPanel panel, ShowPanelAsyncOperation op)
            {
                module.OnShowCallBack(exist, path, panel, op);
            }
            public void Update()
            {
                if (asyncLoadQueue.Count == 0)
                {
                    if (_loading)
                    {
                        module.AcceptRayCast();
                        _loading = false;
                    }
                }
                else
                {
                    module.RefuseRayCast();
                    while (asyncLoadQueue.Count > 0 && asyncLoadQueue.Peek().isDone)
                    {
                        LoadPanelAsyncOperation op = asyncLoadQueue.Dequeue();
                        UILoadComplete(op.value, op.path, op.show);
                    }
                }

            }

            public Canvas CreateCanvas()
            {
                var _canvas = module.assetPart.GetCanvas();
                if (_canvas == null)
                {
                    var root = new GameObject();
                    root.AddComponent<RectTransform>();
                    _canvas = root.AddComponent<Canvas>();
                    root.AddComponent<CanvasScaler>();
                    root.AddComponent<GraphicRaycaster>();
                    _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                }
                _canvas.name = module.name;
                this.canvas = _canvas;
                return _canvas;
            }

            internal void DeleteCanvas()
            {
                if (canvas != null)
                    GameObject.Destroy(canvas.gameObject);
            }


      
        }
    }
}
