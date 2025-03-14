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


        class LayerChangeCheckData
        {
            public UIPanel layer_top = null;
            public UIPanel layer_top_show = null;
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
            layerPart.CreateLayers(_canvas.transform);
            canvas = _canvas;
        }


        private void BeginChangeLayerTopChangeCheck(int layer, LayerChangeCheckData data)
        {
            data.layer_top = layerPart.GetTopPanel(layer);
            data.layer_top_show = layerPart.GetTopShowPanel(layer);
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
            var top = layerPart.GetTopPanel(layer);
            var top_show = layerPart.GetTopShowPanel(layer);
            if (top != data.layer_top)
                delPart?.OnLayerTopChange(layer, top?.GetPath());
            if (top_show != data.layer_top_show)
            {
                delPart?.OnLayerTopVisibleChange(layer, top_show?.GetPath());
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
        public void CloseAll()
        {
            var paths = loadPart.GetPanelNames();
            for (int i = 0; i < paths.Count; i++)
            {
                Close(paths[i]);
            }
        }
    }

    partial class UIModule
    {

        public void SetAsset(UIAsset asset) => assetPart = asset;

        public void SetBridge(IViewBridge bridge) => this.bridgePart = bridge;
        public void SetUIDelegate(IUIDelegate del) => this.delPart = del;

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
