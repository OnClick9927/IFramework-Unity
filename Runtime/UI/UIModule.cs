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
    public partial class UIModule : UpdateModule
    {
        const string item_layer = "Items";
        const string raycast_layer = "RayCast";
        public Canvas canvas { get; private set; }
        private IGroups _groups;
        private UIAsset _asset;
        private Dictionary<UILayer, List<UIPanel>> _panelOrders;
        private Dictionary<string, RectTransform> _layers;
        private Queue<LoadPanelAsyncOperation> asyncLoadQueue;
        private ItemsPool _itemPool;
        private Dictionary<string, UIPanel> panels = new Dictionary<string, UIPanel>();
        private Empty4Raycast raycast;
        private bool _loading = false;
        private bool _force_show_raycast;
        private IUIDelegate del;
        private int _fullScreenCount;

        protected override void Awake()
        {
            _panelOrders = new Dictionary<UILayer, List<UIPanel>>();
            _layers = new Dictionary<string, RectTransform>();
            asyncLoadQueue = new Queue<LoadPanelAsyncOperation>();
            _itemPool = new ItemsPool(this);
        }


        protected override void OnDispose()
        {
            if (_groups != null)
                _groups.Dispose();
            asyncLoadQueue.Clear();
            _layers.Clear();
            if (canvas != null)
                GameObject.Destroy(canvas.gameObject);
            _itemPool.Clear();
        }
        protected override void OnUpdate()
        {
            CheckAsyncLoad();
        }


        private RectTransform CreateLayer(string layerName)
        {
            GameObject go = new GameObject(layerName);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.SetParent(canvas.transform);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.localPosition = Vector3.zero;
            rect.sizeDelta = Vector3.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
            _layers.Add(layerName, rect);
            return rect;
        }
        private void CreateLayers()
        {
            foreach (UILayer item in Enum.GetValues(typeof(UILayer)))
                CreateLayer(item.ToString());
            var items = CreateLayer(item_layer);
            CanvasGroup group = items.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.interactable = false;
            var ray = CreateLayer(raycast_layer);
            raycast = ray.gameObject.AddComponent<Empty4Raycast>();
        }

        private RectTransform GetLayerRectTransform(string layer)
        {
            return _layers[layer];
        }
        private void SetOrder(string path, UIPanel panel)
        {
            UILayer layer = GetPanelLayer(path);
            if (!_panelOrders.ContainsKey(layer))
                _panelOrders.Add(layer, new List<UIPanel>());
            var list = _panelOrders[layer];
            if (list.Contains(panel)) return;
            int order = GetPanelLayerOrder(path);
            bool instert = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (GetPanelLayerOrder(list[i].GetPath()) > order)
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
        UIPanel layer_top = null;
        UIPanel layer_top_visible = null;
        int lastfullScreenCount;
        private void BeginChangeLayerTopChangeCheck(UILayer layer)
        {
            layer_top = GetTopPanel(layer);
            layer_top_visible = GetTopVisiblePanel(layer);
            lastfullScreenCount = _fullScreenCount;
        }
        private void CalcHideSceneCount(string path, bool show)
        {
            var bo = GetPanelHideScene(path);
            if (!bo) return;
            if (show)
                _fullScreenCount++;
            else
                _fullScreenCount--;
        }
        private void EndChangeLayerTopChangeCheck(UILayer layer, string path, bool show)
        {
            CalcHideSceneCount(path, show);
            var top = GetTopPanel(layer);
            var top_visible = GetTopVisiblePanel(layer);



            if (top != layer_top)
                del?.OnLayerTopChange(layer, top?.GetPath());
            if (top_visible != layer_top_visible)
                del?.OnLayerTopVisibleChange(layer, top_visible?.GetPath());
            if (lastfullScreenCount != _fullScreenCount)
                del?.OnFullScreenCount(_fullScreenCount > 0, _fullScreenCount);

            layer_top = null;
            layer_top_visible = null;
            lastfullScreenCount = -1;

        }
        private UIPanel GetTopVisiblePanel(UILayer layer)
        {
            if (!_panelOrders.ContainsKey(layer))
                return null;
            var list = _panelOrders[layer];
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].lastState == PanelState.OnShow)
                {
                    return list[i];
                }
            }
            return null;
        }
        private UIPanel GetTopPanel(UILayer layer)
        {
            if (!_panelOrders.ContainsKey(layer))
                return null;
            var list = _panelOrders[layer];
            if (list.Count == 0) return null;
            return list[list.Count - 1];
        }
        private void DestroyPanel(string path, UIPanel panel)
        {
            var layer = GetPanelLayer(path);
            var list = _panelOrders[layer];
            list.Remove(panel);
            _asset.DestroyPanel(panel.gameObject);
        }
        private UILayer GetPanelLayer(string path) => this._asset.GetPanelLayer(path);
        private int GetPanelLayerOrder(string path) => this._asset.GetPanelLayerOrder(path);
        private bool GetPanelHideScene(string path) => this._asset.GetPanelHideScene(path);






        private void UILoadComplete(UIPanel ui, string path, ShowPanelAsyncOperation op)
        {
            if (ui != null)
            {

                ui.SetPath(path);
                SetOrder(path, ui);
                panels.Add(path, ui);
                _groups.Subscribe(path, ui);
                _groups.OnLoad(path);
                ui.SetState(PanelState.OnLoad);
                if (del != null)
                    del.OnPanelLoad(path);
            }
            OnShowCallBack(path, ui, op);
        }
        private void CheckAsyncLoad()
        {
            if (asyncLoadQueue.Count == 0)
            {
                if (_loading)
                {
                    HideRayCast();
                    _loading = false;
                }
            }
            else
            {
                ShowRayCast();
                while (asyncLoadQueue.Count > 0 && asyncLoadQueue.Peek().isDone)
                {
                    LoadPanelAsyncOperation op = asyncLoadQueue.Dequeue();
                    UILoadComplete(op.value, op.path, op.show);
                }
            }

        }

        private UIPanel Find(string path)
        {
            UIPanel ui;
            panels.TryGetValue(path, out ui);
            return ui;
        }
        public bool GetIsPanelOpen(string path) => Find(path) != null;

        public void ClearUselessItems()
        {
            _itemPool.ClearUseless();
        }

        public UIItemOperation GetItem(string path)
        {
            return _itemPool.Get(path);
        }
        public void SetItem(string path, UIItemOperation go)
        {
            _itemPool.Set(path, go);
        }
        public void SetItem(string path, GameObject go)
        {
            _itemPool.Set(path, go);
        }

        public void ShowRayCast()
        {
            raycast.raycastTarget = true;
        }
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

        public void CreateCanvas()
        {
            var canvas = _asset.GetCanvas();
            if (canvas == null)
            {
                var root = new GameObject(name);
                root.AddComponent<RectTransform>();
                this.canvas = root.AddComponent<Canvas>();
                root.AddComponent<CanvasScaler>();
                root.AddComponent<GraphicRaycaster>();
                this.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            else
            {
                this.canvas = canvas;
                this.canvas.name = name;
            }
            CreateLayers();
            HideRayCast();
        }


        public void SetAsset(UIAsset asset) => _asset = asset;

        public void SetGroups(IGroups groups) => this._groups = groups;
        public void SetUIDelegate(IUIDelegate del) => this.del = del;
        private void OnShowCallBack(string path, UIPanel panel, ShowPanelAsyncOperation op)
        {
            if (panel != null)
            {
                this._groups.OnShow(path);
                panel.SetState(PanelState.OnShow);
                if (del != null)
                    del.OnPanelShow(path);
            }
            UILayer layer = GetPanelLayer(path);

            EndChangeLayerTopChangeCheck(layer, path, true);
            if (op != null)
                op.SetCompelete();
        }

        public ShowPanelAsyncOperation Show(string path)
        {

            if (_groups == null)
                throw new Exception("Please Set IGroups First");
            if (_asset == null)
                throw new Exception("Please Set UILoader First");
            ShowPanelAsyncOperation show_op = new ShowPanelAsyncOperation();
            UILayer layer = GetPanelLayer(path);
            BeginChangeLayerTopChangeCheck(layer);
            var panel = Find(path);
            if (panel != null)
                OnShowCallBack(path, panel, show_op);
            else
            {
                RectTransform parent = GetLayerRectTransform(GetPanelLayer(path).ToString());
                var result = _asset.LoadPanel(parent, path);
                if (result != null)
                    UILoadComplete(result, path, show_op);
                else
                {
                    LoadPanelAsyncOperation op = new LoadPanelAsyncOperation();
                    op.path = path;
                    op.parent = parent;
                    op.show = show_op;
                    if (_asset.LoadPanelAsync(path, op))
                    {
                        _loading = true;
                        asyncLoadQueue.Enqueue(op);
                    }
                    else
                        throw new Exception($"Can't load ui with Name: {path}");
                }
            }
            return show_op;
        }

        public void Hide(string path)
        {
            var panel = Find(path);
            if (panel != null)
            {
                UILayer layer = GetPanelLayer(path);
                BeginChangeLayerTopChangeCheck(layer);
                this._groups.OnHide(path);
                panel.SetState(PanelState.OnHide);
                if (del != null)
                    del.OnPanelHide(path);
                EndChangeLayerTopChangeCheck(layer, path, false);
            }
        }

        public void Close(string path)
        {
            var panel = Find(path);

            if (panel != null)
            {
                this._groups.OnClose(path);
                panel.SetState(PanelState.OnClose);

                _groups.UnSubscribe(path);
                panels.Remove(path);
                UILayer layer = GetPanelLayer(path);
                BeginChangeLayerTopChangeCheck(layer);
                DestroyPanel(path, panel);
                if (del != null)
                    del.OnPanelClose(path);
                EndChangeLayerTopChangeCheck(layer, path, false);
            }
        }

        public void CloseAll()
        {
            var paths = panels.Keys.ToArray();
            for (int i = 0; i < paths.Length; i++)
            {
                Close(paths[i]);
            }
        }
    }
}
