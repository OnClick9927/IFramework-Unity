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

    public static class UIServiceEx
    {
        public const string defaultName = "UI";
        public static UIService UseUI(this Game game, UILayerData layer,
            PanelCollection collection, IViewBridge bridge, IUIDelegate del, Canvas canvas = null, string name = defaultName)
        {
            UIService ui = new UIService(layer, collection, canvas);
            ((IServiceCollection)game).Use(ui, name);
            ui.SetUIDelegate(del);
            ui.SetBridge(bridge);
            ui.CreateCanvas();
            ui.AdaptScreenByCanvasScaler();
            return ui;
        }
        public static UIService UI(this Game game, string name = defaultName) => game.GetService<UIService>(name);

    }


    public partial class UIService : ServiceBase
    {

        private LoadPart loadPart;
        private LayerPart layerPart;
        private IViewBridge bridgePart;
        private IUIDelegate delPart;
        public bool IsLoading => loadPart.IsLoading;
        public Canvas canvas { get; private set; }

        //private SimpleObjectPool<ShowPanelAsyncOperation> show_op = new SimpleObjectPool<ShowPanelAsyncOperation>();
        private UILayerData layer;
        private PanelCollection collection;

        public UIService(UILayerData layer, PanelCollection collection, Canvas canvas)
        {
            this.layer = layer;
            this.collection = collection;
            this.canvas = canvas;
            collection.ListToMap();
        }




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

        protected override void OnUse(IServiceCollection services)
        {
            layerPart = new LayerPart(this);
            loadPart = new LoadPart(this);
            check_show = new LayerChangeCheckData();
            check_hide = new LayerChangeCheckData();
            check_close = new LayerChangeCheckData();
        }
        protected override void OnEnter(IServiceCollection services)
        {
            
        }
        protected override void OnQuit(IServiceCollection services)
        {
            if (bridgePart != null)
                bridgePart.Dispose();
            if (canvas != null)
                GameObject.Destroy(canvas.gameObject);
        }

        internal void CreateCanvas()
        {
            var _canvas = canvas;
            if (_canvas == null)
            {
                var root = new GameObject();
                root.AddComponent<RectTransform>();
                _canvas = root.AddComponent<Canvas>();
                root.AddComponent<CanvasScaler>();
                root.AddComponent<GraphicRaycaster>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            _canvas.name = Name;
            this.canvas = _canvas;
            layerPart.CreateLayers(_canvas);
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



        private void UILoadComplete(string path, UIPanel ui, AsyncTask op)
        {
            //string path = op.path;

            if (ui != null)
            {
                ui.SetPath(path);

                ui.SetState(PanelState.OnLoad);
                layerPart.SetOrder(path, ui);
                bridgePart.Subscribe(path, ui);
                bridgePart.OnLoad(path);
                if (delPart != null)
                    delPart.OnPanelLoad(path);
            }
            CallPanelVisibleChange(ui, true);
            OnShowCallBack(path, false, ui, op);
        }

        private void OnShowCallBack(string path, bool exist, UIPanel panel, AsyncTask op)
        {
            //string path = op.path;
            if (panel != null)
            {
                if (exist)
                    layerPart.SetAsLastOrder(path, panel);
                panel.SetState(PanelState.OnShow);
                this.bridgePart.OnShow(path);
                if (delPart != null)
                    delPart.OnPanelShow(path);
            }
            var layer = GetPanelLayer(path);
            EndChangeLayerTopChangeCheck(layer, path, true, check_show);
            if (op != null)
                op.SetResult();
            //show_op.Set(op);
        }

        public AsyncTask Show(string path)
        {

            if (bridgePart == null)
                throw new Exception("Please Set Bridge First");
            if (delPart == null)
                throw new Exception("Please Set UIDelegate First");

            this.delPart?.OnShowPanelRequest(path);
            var show_op = AsyncTask.CreateFromPool();
            //show_op.path = path;
            var layer = GetPanelLayer(path);
            BeginChangeLayerTopChangeCheck(layer, check_show);
            loadPart.LoadPanel(path, layer, show_op);
            return show_op;
        }

        //private Queue<CMD> cmds = new Queue<CMD>();
        //public struct CMD
        //{
        //    public enum Type
        //    {
        //        Hide, Close,
        //    }
        //    public Type type;
        //    public string path;

        //    public CMD(Type type, string path)
        //    {
        //        this.type = type;
        //        this.path = path;
        //    }
        //}
        private void _Hide(string path)
        {
            var panel = loadPart.Find(path);
            if (panel != null)
            {
                var layer = GetPanelLayer(path);
                BeginChangeLayerTopChangeCheck(layer, check_hide);
                panel.SetState(PanelState.OnHide);
                this.bridgePart.OnHide(path);
                if (delPart != null)
                    delPart.OnPanelHide(path);
                EndChangeLayerTopChangeCheck(layer, path, false, check_hide);
            }
        }
        private void _Close(string path)
        {
            var panel = loadPart.Find(path);

            if (panel != null)
            {
                var layer = GetPanelLayer(path);
                BeginChangeLayerTopChangeCheck(layer, check_close);

                CallPanelVisibleChange(panel, false);

                panel.SetState(PanelState.OnClose);
                this.bridgePart.OnClose(path);

                bridgePart.UnSubscribe(path);
                layerPart.RemovePanel(path, panel);
                loadPart.RemovePanel(path);
                if (delPart != null)
                    delPart.OnPanelClose(path);
                EndChangeLayerTopChangeCheck(layer, path, false, check_close);
            }
        }


        public async void Hide(string path)
        {
            await AsyncTask.NextFrame();
            _Hide(path);

            //cmds.Enqueue(new CMD(CMD.Type.Hide, path));
        }
        public async void Close(string path)
        {
            await AsyncTask.NextFrame();
            _Close(path);
            //cmds.Enqueue(new CMD(CMD.Type.Close, path));
        }
        //protected override void OnUpdate()
        //{
        //    var count = cmds.Count;
        //    if (count == 0) return;
        //    for (int i = 0; i < count; i++)
        //    {
        //        var cmd = cmds.Dequeue();
        //        switch (cmd.type)
        //        {
        //            case CMD.Type.Hide:
        //                _Hide(cmd.path);
        //                break;
        //            case CMD.Type.Close:
        //                _Close(cmd.path);
        //                break;

        //        }

        //    }
        //}
        //private SimpleObjectPool<ClosePanelAsyncOperation> close_op = new SimpleObjectPool<ClosePanelAsyncOperation>();
        //private SimpleObjectPool<HidePanelAsyncOperation> hide_op = new SimpleObjectPool<HidePanelAsyncOperation>();
        //private List<PanelAsyncOperation> colse_hide_list = new List<PanelAsyncOperation>();
        public AsyncTask CloseAsync(string path)
        {
            if (loadPart.Find(path) == null) return AsyncTask.CompletedTask;

            var operation = AsyncTask.CreateFromPool();
            //operation.path = path;
            operation.ContinueWith(_ =>
            {
                Close(path);
            });
            this.bridgePart.OnCloseAsync(path, operation);
            this.delPart?.OnClosePanelAsync(path);
            ;
            //colse_hide_list.Add(operation);
            return operation;
        }
        public AsyncTask HideAsync(string path)
        {
            if (loadPart.Find(path) == null) return AsyncTask.CompletedTask;
            var operation = AsyncTask.CreateFromPool();
            operation.ContinueWith(_ =>
            {
                Hide(path);
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

    partial class UIService
    {
        //min 1125*2346
        //max 768*1024
        public void AdaptScreenByCanvasScaler(float min = 0.48f, float max = 0.75f)
        {
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null) return;
            var width = (float)Screen.width;
            var height = (float)Screen.height;
            var percent = width / height;
            var length = max - min;
            percent = (percent - min) / length;
            scaler.matchWidthOrHeight = percent;
        }
        //internal void SetAsset(UIAsset asset) => assetPart = asset;

        internal void SetBridge(IViewBridge bridge) => this.bridgePart = bridge;
        internal void SetUIDelegate(IUIDelegate del) => this.delPart = del;

        public void RefuseRayCast() => layerPart.RefuseRayCast();
        public void AcceptRayCast()
        {
            if (IsLoading) return;
            layerPart.AcceptRayCast();
        }

        public void ForceRefuseRayCast() => layerPart.ForceRefuseRayCast();
        public void ForceAcceptRayCast() => layerPart.ForceAcceptRayCast();
        private RectTransform GetLayerTransform(string layer) => layerPart.GetLayerTransform(layer);



        public PanelCollection.Data GetPanelData(string path) => collection?.GetData(path);
        public List<string> GetLayerNames() => layer.GetLayerNames();
        public virtual int GetPanelLayer(string path)
        {
            var data = GetPanelData(path);
            if (data != null)
                return data.layer;
            return 0;
        }
        public virtual bool GetPanelFullScreen(string path)
        {
            var data = GetPanelData(path);
            if (data != null)
                return data.fullScreen;
            return false;
        }
        public virtual string GetLayerName(int layer) => this.layer.GetLayerName(layer);
        public virtual int LayerNameToIndex(string layerName) => this.layer.LayerNameToIndex(layerName);

        //public int GetPanelLayer(string path) => this.assetPart.GetPanelLayer(path);
        //private bool GetPanelFullScreen(string path) => this.assetPart.GetPanelFullScreen(path);
        //public List<string> GetLayerNames() => this.assetPart.GetLayerNames();
        //public int LayerNameToIndex(string layerName) => this.assetPart.LayerNameToIndex(layerName);
        //public string GetLayerName(int layer) => this.assetPart.GetLayerName(layer);
        public bool GetIsPanelOpen(string path) => loadPart.Find(path) != null;

        public UIPanel FindPanel(string path) => loadPart.Find(path);

        public UIPanel GetLayerTop(int layer) => layerPart.GetLayerTop(layer);
        public UIPanel GetLayerTopShow(int layer) => layerPart.GetLayerTopShow(layer);
        public UIPanel GetTopShow() => layerPart.GetTopShow();
        public List<string> GetVisibleList() => layerPart.GetVisibleList();

        List<string> help_under = StaticPool.Get<List<string>>();
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
