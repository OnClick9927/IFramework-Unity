using IFramework;
using IFramework.UI;
using IFramework.UI.MVC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class UIGame : Game, IUIDelegate
{
    private class Asset : UIAsset
    {
        public Asset(PanelCollection collection) : base(collection)
        {
        }

        public override bool LoadItemAsync( LoadItemAsyncOperation op)
        {
            op.SetValue(AssetDatabase.LoadAssetAtPath<GameObject>(op.path));
            return true;
        }

        public override UIPanel LoadPanel(RectTransform parent, string path)
        {
            return null;
        }
        public override void ReleaseItemAsset(GameObject gameObject)
        {

        }
        public override bool LoadPanelAsync(LoadPanelAsyncOperation op)
        {
            _LoadPanelAsync(op);
            return true;
        }

        protected async void _LoadPanelAsync(LoadPanelAsyncOperation op)
        {
            await Task.Delay(2000);
            var go = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<UIPanel>(op.path), op.parent);
            op.SetValue(go);
        }
    }
    public UIModule ui;
    public TextAsset txt;
    internal void CloseView()
    {
        ui.Close(PanelNames_UIGame.PanelOne);
    }
    public override async void Init()
    {
        ui = this.modules.CreateModule<UIModule>();
        ui.SetUIDelegate(this);
        ui.SetAsset(new Asset(JsonUtility.FromJson<PanelCollection>(txt.text)));
        ui.SetGroups(new MvcGroups(new Dictionary<string, Type>() {

            { PanelNames_UIGame.PanelOne,typeof(PanelOneView)}

        })); ;
        ui.CreateCanvas();
        Log.L("BeginShow");
        await ui.Show(PanelNames_UIGame.PanelOne);
        Log.L("EndShow");
    }

    void IUIDelegate.OnFullScreenCount(bool hide, int count)
    {
        Log.L("OnFullScreenCount");
    }

    void IUIDelegate.OnLayerTopChange(UILayer layer, string path)
    {
        Log.L("OnLayerTopChange");
    }

    void IUIDelegate.OnLayerTopVisibleChange(UILayer layer, string path)
    {
        Log.L("OnLayerTopVisibleChange");

    }

    void IUIDelegate.OnPanelClose(string path)
    {
        Log.L("OnPanelClose");
    }

    void IUIDelegate.OnPanelHide(string path)
    {
        Log.L("OnPanelHide");

    }

    void IUIDelegate.OnPanelLoad(string path)
    {
        Log.L("OnPanelLoad");

    }

    void IUIDelegate.OnPanelShow(string path)
    {
        Log.L("OnPanelShow");

    }

    public override void Startup()
    {

    }


}
