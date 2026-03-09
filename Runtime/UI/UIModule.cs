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
    public partial class UIModule : Module
    {

        private LoadPart loadPart;
        private LayerPart layerPart;
        internal UIAsset assetPart;
        private IViewBridge bridgePart;
        private IUIDelegate delPart;
        public Canvas canvas { get; private set; }

        //private SimpleObjectPool<ShowPanelAsyncOperation> show_op = new SimpleObjectPool<ShowPanelAsyncOperation>();

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
        //protected override void OnUpdate()
        //{
        //    //loadPart.Update();
        //    //if (colse_hide_list.Count > 0)
        //    //{
        //    //    for (int i = colse_hide_list.Count - 1; i >= 0; i--)
        //    //    {
        //    //        var op = colse_hide_list[i];
        //    //        if (op.IsCompleted)
        //    //        {
        //    //            if (op is HidePanelAsyncOperation)
        //    //            {
        //    //                var _op = op as HidePanelAsyncOperation;
        //    //                Hide(_op.path);
        //    //                hide_op.Set(_op);
        //    //            }
        //    //            else if (op is ClosePanelAsyncOperation)
        //    //            {
        //    //                var _op = op as ClosePanelAsyncOperation;
        //    //                Close(_op.path);
        //    //                close_op.Set(_op);
        //    //            }
        //    //            colse_hide_list.RemoveAt(i);
        //    //        }
        //    //    }
        //    //}
        //}

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
                string _path = top_show?.GetPath();
                if (top_show == null)
                    delPart?.OnTopShowChange(0, _path);
                else
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



        private void UILoadComplete(UIPanel ui, PanelAsyncOperation op)
        {
            string path = op.path;

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
            OnShowCallBack(false, ui, op);
        }

        private void OnShowCallBack(bool exist, UIPanel panel, PanelAsyncOperation op)
        {
            string path = op.path;
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
                op.SetResult();
            //show_op.Set(op);
        }

        public PanelAsyncOperation Show(string path)
        {

            if (bridgePart == null)
                throw new Exception("Please Set Bridge First");
            if (assetPart == null)
                throw new Exception("Please Set UILoader First");

            this.delPart?.OnShowPanelRequest(path);
            var show_op = StaticPool<PanelAsyncOperation>.Get();
            show_op.path = path;
            var layer = GetPanelLayer(path);
            BeginChangeLayerTopChangeCheck(layer, check_show);
            loadPart.LoadPanel(layer, show_op);
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

        //private SimpleObjectPool<ClosePanelAsyncOperation> close_op = new SimpleObjectPool<ClosePanelAsyncOperation>();
        //private SimpleObjectPool<HidePanelAsyncOperation> hide_op = new SimpleObjectPool<HidePanelAsyncOperation>();
        //private List<PanelAsyncOperation> colse_hide_list = new List<PanelAsyncOperation>();
        public AsyncTask CloseAsync(string path)
        {
            if (loadPart.Find(path) == null) return PanelAsyncOperation.CompletedTask;

            var operation = PanelAsyncOperation.CreateFromPool();
            operation.path = path;
            operation.ContinueWith<PanelAsyncOperation>(_ =>
            {
                Close(_.path);
            });
            this.bridgePart.OnCloseAsync(path, operation);
            this.delPart?.OnClosePanelAsync(path);
       ;
            //colse_hide_list.Add(operation);
            return operation;
        }
        public AsyncTask HideAsync(string path)
        {
            if (loadPart.Find(path) == null) return PanelAsyncOperation.CompletedTask;
            var operation = PanelAsyncOperation.CreateFromPool();
            operation.path = path;
            operation.ContinueWith<PanelAsyncOperation>(_ =>
            {
                Hide(_.path);
            });
            this.bridgePart.OnHideAsync(path, operation);
            this.delPart?.OnHidePanelAsync(path);
            //colse_hide_list.Add(operation);


            return operation;
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

        List<string> help_under = StaticPool<List<string>>.Get();
        public List<string> GetVisibleListUnder(int layer)
        {
            var all = GetVisibleList();
            help_under.Clear();
            bool ready = false;
            for (int i = 0; i < all.Count; i++)
            {
                string path = all[i];
                if (ready || GetPanelLayer(path) <= layer)
                {
                    ready = true;
                    help_under.Add(path);
                }
            }
            return help_under;

        }


    }
}
