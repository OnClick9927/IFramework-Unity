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
using static IFramework.UI.UIPanel;

namespace IFramework.UI
{
    public partial class UIModule : UpdateModule
    {

        private LoadPart loadPart;
        private LayerPart layerPart;
        internal UIAsset assetPart;
        private IViewBridge bridgePart;
        private IUIDelegate delPart;
        public Canvas canvas { get; private set; }

        private SimpleObjectPool<ShowPanelAsyncOperation> show_op = new SimpleObjectPool<ShowPanelAsyncOperation>();

        class LayerChangeCheckData
        {
            public UIPanel layer_top = null;
            public UIPanel layer_top_show = null;
            public UIPanel top_show = null;
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
        }
        protected override void OnUpdate() => loadPart.Update();
        public void CreateCanvas()
        {
            var _canvas = loadPart.CreateCanvas();
            layerPart.CreateLayers(_canvas);
            canvas = _canvas;
        }


        private void BeginChangeLayerTopChangeCheck(int layer, LayerChangeCheckData data)
        {
            data.layer_top = GetLayerTop(layer);
            data.layer_top_show = GetLayerTopShow(layer);
            data.top_show = GetTopShow();

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

        private void CallPanelVisibleChange(UIPanel panel, bool visible)
        {
            //var state = panel.lastState;
            var path = panel.GetPath();
            if (delPart != null)
                delPart.OnVisibleChange(path, visible);
            if (visible)
                bridgePart.OnBecameVisible(path);
            else
                bridgePart.OnBecameInvisible(path);
        }


        private void EndChangeLayerTopChangeCheck(int layer, string path, bool show, LayerChangeCheckData data)
        {
            CalcHideSceneCount(path, show);
            var layer_top = GetLayerTop(layer);
            var layer_top_show = GetLayerTopShow(layer);
            var top_show = GetTopShow();

            if (top_show != data.top_show)
            {
                string _path = top_show.GetPath();
                delPart?.OnTopShowChange(GetPanelLayer(_path), _path);
            }
            if (layer_top != data.layer_top)
                delPart?.OnLayerTopChange(layer, layer_top?.GetPath());
            if (layer_top_show != data.layer_top_show)
            {
                delPart?.OnLayerTopShowChange(layer, layer_top_show?.GetPath());
                layerPart.LegalLayerPanelVisible();
            }
            if (data.fullScreenCount != _fullScreenCount)
                delPart?.OnFullScreenCount(_fullScreenCount > 0, _fullScreenCount);

            data.layer_top = null;
            data.layer_top_show = null;
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
            CallPanelVisibleChange(ui, true);
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
            show_op.Set(op);
        }

        public ShowPanelAsyncOperation Show(string path)
        {

            if (bridgePart == null)
                throw new Exception("Please Set Bridge First");
            if (assetPart == null)
                throw new Exception("Please Set UILoader First");


            ShowPanelAsyncOperation show_op = this.show_op.Get();
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

                CallPanelVisibleChange(panel, false);

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


        Queue<string> close_all_help_queue = new Queue<string>();
        public void CloseAll()
        {
            var loaded = loadPart.GetLoadedPanelPaths();
            foreach (var item in loaded)
            {
                this.close_all_help_queue.Enqueue(item);
            }
            while (this.close_all_help_queue.Count > 0)
            {
                var path = this.close_all_help_queue.Dequeue();
                Close(path);
            }
        }
        public void CloseWithout(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
            {
                CloseAll();
            }
            else
            {
                var loaded = loadPart.GetLoadedPanelPaths();
                foreach (var item in loaded)
                {
                    this.close_all_help_queue.Enqueue(item);
                }
                while (this.close_all_help_queue.Count > 0)
                {
                    var path = this.close_all_help_queue.Dequeue();
                    if (paths.Any(x => x == path)) continue;
                    Close(path);
                }
            }
        }
        public void CloseByLayer(string layerName)
        {
            var list = layerPart.FindPanelsByLayerName(layerName);
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                close_all_help_queue.Enqueue(list[i].GetPath());
            }
            while (this.close_all_help_queue.Count > 0)
            {
                var path = this.close_all_help_queue.Dequeue();
                Close(path);
            }
        }
    }

    partial class UIModule
    {
        //min 1125*2346
        //max 768*1024
        public void AdaptScreenByCanvasScaler(float min = 0.48f, float max = 0.75f)
        {
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            var width = (float)Screen.width;
            var height = (float)Screen.height;
            var percent = width / height;
            var length = max - min;
            percent = (percent - min) / length;
            scaler.matchWidthOrHeight = percent;
        }
        public void SetAsset(UIAsset asset) => assetPart = asset;

        public void SetBridge(IViewBridge bridge) => this.bridgePart = bridge;
        public void SetUIDelegate(IUIDelegate del) => this.delPart = del;

        public void RefuseRayCast() => layerPart.RefuseRayCast();
        public void AcceptRayCast() => layerPart.AcceptRayCast();
        public void ForceRefuseRayCast() => layerPart.ForceRefuseRayCast();
        public void ForceAcceptRayCast() => layerPart.ForceAcceptRayCast();
        private RectTransform GetLayerTransform(string layer) => layerPart.GetLayerTransform(layer);


        public int GetPanelLayer(string path) => this.assetPart.GetPanelLayer(path);
        private bool GetPanelFullScreen(string path) => this.assetPart.GetPanelFullScreen(path);
        public List<string> GetLayerNames() => this.assetPart.GetLayerNames();
        public int LayerNameToIndex(string layerName) => this.assetPart.LayerNameToIndex(layerName);
        public string GetLayerName(int layer) => this.assetPart.GetLayerName(layer);
        public bool GetIsPanelOpen(string path) => loadPart.Find(path) != null;

        public UIPanel FindPanel(string path) => loadPart.Find(path);

        public UIPanel GetLayerTop(int layer) => layerPart.GetLayerTop(layer);
        public UIPanel GetLayerTopShow(int layer) => layerPart.GetLayerTopShow(layer);
        public UIPanel GetTopShow() => layerPart.GetTopShow();
        public List<string> GetVisibleList() => layerPart.GetVisibleList();



    }
}
