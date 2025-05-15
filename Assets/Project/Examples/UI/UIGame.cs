using IFramework;
using IFramework.UI;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class UIGame : Game, IUIDelegate
{
    public class Asset : UIAsset
    {
        public Asset(UILayerData layer, PanelCollection collection) : base(layer, collection)
        {
        }



        public override UIPanel LoadPanel(RectTransform parent, string path)
        {
            return null;
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
    public UILayerData layer;
    internal void CloseView()
    {
        ui.Close(PanelNames_UIGame.PanelOne);

        Events.Publish(nameof(AddArg), new AddArg() { time = Time.deltaTime });

    }
    protected override async void Init()
    {
        ui = this.modules.CreateModule<UIModule>();

        ui.SetUIDelegate(this);
        ui.SetAsset(new Asset(layer, JsonUtility.FromJson<PanelCollection>(txt.text)));
        ui.SetBridge(new ViewBridge(PanelNames_UIGame.map));
        ui.CreateCanvas();
        Log.L("BeginShow");
        await ui.Show(PanelNames_UIGame.PanelOne);
        Log.L("EndShow");
        //Test();
    }


    void IUIDelegate.OnFullScreenCount(bool hide, int count)
    {
        Log.L("OnFullScreenCount");
    }

    void IUIDelegate.OnLayerTopChange(int layer, string path)
    {
        Log.L("OnLayerTopChange");
    }

    void IUIDelegate.OnLayerTopShowChange(int layer, string path)
    {
        Log.L("OnLayerTopShowChange");

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
    void IUIDelegate.OnVisibleChange(string path, bool visible)
    {
        Log.L("OnVisibleChange");
    }
    protected override void Startup()
    {

    }

    void IUIDelegate.OnTopShowChange(int layer, string path)
    {
        Log.L("OnTopShowChange");
    }
}
