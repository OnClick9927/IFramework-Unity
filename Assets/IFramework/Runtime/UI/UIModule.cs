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
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static IFramework.UI.UIPanel;

namespace IFramework.UI
{
    public partial class UIModule : UpdateModule
    {

        private LoadPart loadPart;
        private LayerPart layerPart;
        internal UIAsset assetPart;
        private ItemsPool itemPart;
        private IViewBridge bridgePart;
        private IUIDelegate delPart;
        public Canvas canvas { get; private set; }


        class LayerChangeCheckData
        {
            public UIPanel layer_top = null;
            public UIPanel layer_top_visible = null;
            public int fullScreenCount;
        }
        private int _fullScreenCount;
        private LayerChangeCheckData check_show;
        private LayerChangeCheckData check_hide;
        private LayerChangeCheckData check_close;

        protected override void Awake()
        {
            layerPart = new LayerPart(this);
            loadPart = new LoadPart(this);
            itemPart = new ItemsPool(this);
            check_show = new LayerChangeCheckData();
            check_hide = new LayerChangeCheckData();
            check_close = new LayerChangeCheckData();
        }
        protected override void OnDispose()
        {
            if (bridgePart != null)
                bridgePart.Dispose();
            layerPart.Clear();
            loadPart.DeleteCanvas();
            itemPart.Clear();
        }
        protected override void OnUpdate() => loadPart.Update();
        public void CreateCanvas()
        {
            var _canvas = loadPart.CreateCanvas();
            layerPart.CreateLayers(_canvas.transform);
            canvas = _canvas;
        }


        private void BeginChangeLayerTopChangeCheck(int layer, LayerChangeCheckData data)
        {
            data.layer_top = layerPart.GetTopPanel(layer);
            data.layer_top_visible = layerPart.GetTopVisiblePanel(layer);
            data.fullScreenCount = _fullScreenCount;
        }
        private void CalcHideSceneCount(string path, bool show)
        {
            var bo = GetPanelFullScreen(path);
            if (!bo) return;
            if (show)
                _fullScreenCount++;
            else
                _fullScreenCount--;
        }

        private void CallPanelEnableChange(UIPanel panel, bool enable)
        {
            //var state = panel.lastState;
            var path = panel.GetPath();
            if (enable)
                bridgePart.OnEnable(path);
            else
                bridgePart.OnDisable(path);
        }


        private void EndChangeLayerTopChangeCheck(int layer, string path, bool show, LayerChangeCheckData data)
        {
            CalcHideSceneCount(path, show);
            var top = layerPart.GetTopPanel(layer);
            var top_visible = layerPart.GetTopVisiblePanel(layer);
            if (top != data.layer_top)
                delPart?.OnLayerTopChange(layer, top?.GetPath());
            if (top_visible != data.layer_top_visible)
            {
                delPart?.OnLayerTopVisibleChange(layer, top_visible?.GetPath());
                layerPart.LegalLayerPanelVisible();
            }
            if (data.fullScreenCount != _fullScreenCount)
                delPart?.OnFullScreenCount(_fullScreenCount > 0, _fullScreenCount);

            data.layer_top = null;
            data.layer_top_visible = null;
            data.fullScreenCount = -1;

        }



        private void UILoadComplete(UIPanel ui, string path, ShowPanelAsyncOperation op)
        {
            if (ui != null)
            {
                ui.SetPath(path);

                layerPart.SetOrder(path, ui);
                bridgePart.Subscribe(path, ui);
                bridgePart.OnLoad(path);
                ui.SetState(PanelState.OnLoad);
                if (delPart != null)
                    delPart.OnPanelLoad(path);
            }
            CallPanelEnableChange(ui, true);
            OnShowCallBack(false, path, ui, op);
        }

        private void OnShowCallBack(bool exist, string path, UIPanel panel, ShowPanelAsyncOperation op)
        {
            if (panel != null)
            {
                if (exist)
                    layerPart.SetAsLastOrder(path, panel);
                this.bridgePart.OnShow(path);
                panel.SetState(PanelState.OnShow);
                if (delPart != null)
                    delPart.OnPanelShow(path);
            }
            var layer = GetPanelLayer(path);
            EndChangeLayerTopChangeCheck(layer, path, true, check_show);
            if (op != null)
                op.SetComplete();
        }

        public ShowPanelAsyncOperation Show(string path)
        {

            if (bridgePart == null)
                throw new Exception("Please Set Bridge First");
            if (assetPart == null)
                throw new Exception("Please Set UILoader First");


            ShowPanelAsyncOperation show_op = new ShowPanelAsyncOperation();
            var layer = GetPanelLayer(path);
            BeginChangeLayerTopChangeCheck(layer, check_show);
            loadPart.LoadPanel(path, layer, show_op);
            return show_op;
        }
        public void Hide(string path)
        {
            var panel = loadPart.Find(path);
            if (panel != null)
            {
                var layer = GetPanelLayer(path);
                BeginChangeLayerTopChangeCheck(layer, check_hide);
                this.bridgePart.OnHide(path);
                panel.SetState(PanelState.OnHide);
                if (delPart != null)
                    delPart.OnPanelHide(path);
                EndChangeLayerTopChangeCheck(layer, path, false, check_hide);
            }
        }
        public void Close(string path)
        {
            var panel = loadPart.Find(path);

            if (panel != null)
            {
                var layer = GetPanelLayer(path);
                BeginChangeLayerTopChangeCheck(layer, check_close);

                CallPanelEnableChange(panel, false);

                this.bridgePart.OnClose(path);
                panel.SetState(PanelState.OnClose);

                bridgePart.UnSubscribe(path);
                layerPart.RemovePanel(path, panel);
                loadPart.RemovePanel(path);
                if (delPart != null)
                    delPart.OnPanelClose(path);
                EndChangeLayerTopChangeCheck(layer, path, false, check_close);
            }
        }
        public void CloseAll()
        {
            var paths = loadPart.GetPanelNames();
            for (int i = 0; i < paths.Count; i++)
            {
                Close(paths[i]);
            }
        }
    }




    public class RunTimeUILayerData
    {
        public RectTransform rect;
        //public CanvasGroup group;
        public Transform parent;
        public string name;
    }
    partial class UIModule
    {
        private class LayerPart
        {
            private Dictionary<string, List<UIPanel>> _panelOrders;
            private Dictionary<string, RunTimeUILayerData> _layers;
            private UIModule module;
            private Empty4Raycast raycast;
            private bool _force_show_raycast;
            private List<string> layerNames;
            public LayerPart(UIModule module)
            {
                this.module = module;
                _panelOrders = new Dictionary<string, List<UIPanel>>();
                _layers = new Dictionary<string, RunTimeUILayerData>();
            }
            private RunTimeUILayerData CreateLayer(string layerName, Transform parent)
            {
                GameObject go = new GameObject(layerName);
                RectTransform rect = go.AddComponent<RectTransform>();
                var group = rect.gameObject.AddComponent<CanvasGroup>();
                rect.SetParent(parent);
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.localPosition = Vector3.zero;
                rect.sizeDelta = Vector3.zero;
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
                var data = new RunTimeUILayerData()
                {
                    //group = group,
                    rect = rect,
                    parent = parent,
                    name = layerName,
                };
                _layers.Add(layerName, data);

                return data;
            }
            public void CreateLayers(Transform parent)
            {
                layerNames = module.GetLayerNames();
                foreach (var item in layerNames)
                    CreateLayer(item, parent);
                CreateLayer(UILayerData.item_layer, parent);
                //SwitchLayerVisible(UILayerData.item_layer, false);
                var ray = CreateLayer(UILayerData.rayCast_layer, parent);
                raycast = ray.rect.gameObject.AddComponent<Empty4Raycast>();
                HideRayCast();
            }
            //private void SwitchLayerVisible(string layerName, bool visible)
            //{
            //    var layer = GetRTLayerData(layerName);
            //    layer.group.alpha = visible ? 1 : 0;
            //    layer.group.blocksRaycasts = visible ? true : false;
            //    layer.group.interactable = visible ? true : false;
            //}
            public RunTimeUILayerData GetRTLayerData(string layer) => _layers[layer];


            public void ShowRayCast() => raycast.raycastTarget = true;
            public void HideRayCast()
            {
                if (_force_show_raycast) return;
                raycast.raycastTarget = false;
            }
            public void ForceShowRayCast()
            {
                _force_show_raycast = true;
                ShowRayCast();
            }
            public void ForceHideRayCast()
            {
                _force_show_raycast = false;
                HideRayCast();
            }
            public void Clear()
            {
                _layers.Clear();
            }

            public void SetAsLastOrder(string path, UIPanel panel)
            {
                var layer = module.GetPanelLayer(path);
                var layerName = module.GetLayerName(layer);

                if (!_panelOrders.ContainsKey(layerName))
                    _panelOrders.Add(layerName, new List<UIPanel>());
                var list = _panelOrders[layerName];
                list.Remove(panel);
                list.Add(panel);
                panel.SetSiblingIndex(list.Count);
            }
            public void SetOrder(string path, UIPanel panel)
            {
                var layer = module.GetPanelLayer(path);
                var layerName = module.GetLayerName(layer);
                if (!_panelOrders.ContainsKey(layerName))
                    _panelOrders.Add(layerName, new List<UIPanel>());
                var list = _panelOrders[layerName];
                if (module.GetIgnoreOrder())
                {
                    SetAsLastOrder(path, panel);
                }
                else
                {
                    if (list.Contains(panel)) return;
                    int order = module.GetPanelLayerOrder(path);
                    bool instert = false;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (module.GetPanelLayerOrder(list[i].GetPath()) > order)
                        {
                            var sbindex = list[i].GetSiblingIndex();
                            panel.SetSiblingIndex(sbindex);
                            list.Insert(sbindex, panel);
                            instert = true;
                            break;
                        }
                    }
                    if (!instert)
                        list.Add(panel);

                }

            }
            public UIPanel GetTopVisiblePanel(int layer)
            {
                var layerName = module.GetLayerName(layer);

                if (!_panelOrders.ContainsKey(layerName))
                    return null;
                var list = _panelOrders[layerName];
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].visible)
                    {
                        return list[i];
                    }
                }
                return null;
            }
            public UIPanel GetTopPanel(int layer)
            {
                var layerName = module.GetLayerName(layer);
                if (!_panelOrders.ContainsKey(layerName))
                    return null;
                var list = _panelOrders[layerName];
                if (list.Count == 0) return null;
                return list[list.Count - 1];
            }
            public void RemovePanel(string path, UIPanel panel)
            {
                var layer = module.GetPanelLayer(path);
                var layerName = module.GetLayerName(layer);

                var list = _panelOrders[layerName];
                list.Remove(panel);
            }

            public void LegalLayerPanelVisible()
            {
                bool visible = true;

                for (int i = layerNames.Count - 1; i >= 0; i--)
                {
                    var layerName = layerNames[i];

                    if (_panelOrders.TryGetValue(layerName, out var list))
                    {
                        for (int j = list.Count - 1; j >= 0; j--)
                        {
                            var panel = list[j];
                            var _visible = visible && panel.visible;
                            if (panel.SwitchVisible(_visible))
                                module.CallPanelEnableChange(panel, _visible);
                            if (_visible)
                            {
                                var path = panel.GetPath();
                                if (module.GetPanelFullScreen(path))
                                    visible = false;
                            }
                        }
                    }





                }
            }

        }

        private class LoadPart
        {
            private bool _loading = false;
            private UIModule module;
            private Queue<LoadPanelAsyncOperation> asyncLoadQueue;
            private Dictionary<string, UIPanel> panels = new Dictionary<string, UIPanel>();
            public Canvas canvas { get; private set; }

            public List<string> GetPanelNames() => panels.Keys.ToList();
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
                        module.HideRayCast();
                        _loading = false;
                    }
                }
                else
                {
                    module.ShowRayCast();
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
    partial class UIModule
    {

        public void SetAsset(UIAsset asset) => assetPart = asset;

        public void SetBridge(IViewBridge bridge) => this.bridgePart = bridge;
        public void SetUIDelegate(IUIDelegate del) => this.delPart = del;
        public void ClearUselessItems() => itemPart.ClearUseless();

        public UIItemOperation GetItem(string path) => itemPart.Get(path);
        public void SetItem(string path, UIItemOperation go) => itemPart.Set(path, go);
        public void SetItem(string path, GameObject go) => itemPart.Set(path, go);

        public void ShowRayCast() => layerPart.ShowRayCast();
        public void HideRayCast() => layerPart.HideRayCast();
        public void ForceShowRayCast() => layerPart.ForceShowRayCast();
        public void ForceHideRayCast() => layerPart.ForceHideRayCast();
        public RunTimeUILayerData GetRTLayerData(string layer) => layerPart.GetRTLayerData(layer);


        public int GetPanelLayer(string path) => this.assetPart.GetPanelLayer(path);
        public int GetPanelLayerOrder(string path) => this.assetPart.GetPanelLayerOrder(path);
        private bool GetPanelFullScreen(string path) => this.assetPart.GetPanelFullScreen(path);
        public List<string> GetLayerNames() => this.assetPart.GetLayerNames();
        public bool GetIgnoreOrder() => this.assetPart.GetIgnoreOrder();
        public int LayerNameToIndex(string layerName) => this.assetPart.LayerNameToIndex(layerName);
        public string GetLayerName(int layer) => this.assetPart.GetLayerName(layer);
        public bool GetIsPanelOpen(string path) => loadPart.Find(path) != null;
        //public void LegalLayerPanelVisible() => this.layerPart.LegalLayerPanelVisible();
    }
}
