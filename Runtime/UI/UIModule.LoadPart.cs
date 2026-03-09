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
using UnityEngine;
using UnityEngine.UI;

namespace IFramework.UI
{
    partial class UIModule
    {
        private class LoadPart
        {

            private int __Loading;
            private int _loading
            {
                get { return __Loading; }
                set
                {
                    if (__Loading == value) return;
                    __Loading = value;
                    if (value == 0)
                        module.AcceptRayCast();

                    else
                        module.RefuseRayCast();
                }
            }

            private UIModule module;
            //private Queue<LoadPanelAsyncOperation> asyncLoadQueue;
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
            //private SimpleObjectPool<LoadPanelAsyncOperation> load_op = new SimpleObjectPool<LoadPanelAsyncOperation>();

            public LoadPart(UIModule module)
            {
                this.module = module;
                //asyncLoadQueue = new Queue<LoadPanelAsyncOperation>();
                panels = new Dictionary<string, UIPanel>();
            }
            public UIPanel Find(string path)
            {
                UIPanel ui;
                panels.TryGetValue(path, out ui);
                return ui;
            }

            public void LoadPanel(int layer, PanelAsyncOperation show_op)
            {
                string path = show_op.path;
                var panel = Find(path);

                if (panel != null)
                    OnShowCallBack(true, panel, show_op);
                else
                {
                    RectTransform parent = module.GetLayerTransform(module.GetLayerName(layer));
                    var result = module.assetPart.LoadPanel(parent, path);
                    if (result != null)
                        UILoadComplete(result, show_op);
                    else
                    {
                        var op = LoadPanelAsyncOperation.CreateFromPool();
                        //op.path = path;
                        op.parent = parent;
                        op.show = show_op;
                        op.ContinueWith<LoadPanelAsyncOperation>(_ =>
                        {
                            UILoadComplete(_.result, _.show);
                            _loading--;
                        });
                        if (module.assetPart.LoadPanelAsync(op))
                        {
                            _loading++;
                            //asyncLoadQueue.Enqueue(op);
                        }
                        else
                            throw new Exception($"Can't load ui with Name: {path}");
                    }
                }
            }
            private void UILoadComplete(UIPanel ui, PanelAsyncOperation op)
            {
                string path = op.path;

                if (ui != null) panels.Add(path, ui);
                module.UILoadComplete(ui, op);
            }
            private void OnShowCallBack(bool exist, UIPanel panel, PanelAsyncOperation op)
            {
                module.OnShowCallBack(exist, panel, op);
            }
            //public void Update()
            //{


            //}

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
