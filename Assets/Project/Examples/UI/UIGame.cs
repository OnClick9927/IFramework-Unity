using IFramework;
using IFramework.UI;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class UIGame : Game, IUIDelegate
{
    async AsyncTask<UIPanel> IUIDelegate.LoadPanelAsync(RectTransform parent, PanelCollection.Data data)
    {
        await Task.Delay(2000);
        var go = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<UIPanel>(data.path), parent);
        return go;
    }
    void IUIDelegate.DestroyPanel(GameObject gameObject)
    {
        GameObject.Destroy(gameObject);
    }


    //public UIService ui;
    public TextAsset txt;
    public UILayerData layer;
    internal void CloseView()
    {
        this.GetService<UIService>().Close(PanelNames_UIGame.PanelOne);

        Events.Publish(nameof(AddArg), new AddArg() { time = Time.deltaTime });

    }
    protected async override void Startup()
    {
        this.UseValues();
        var ui = this.UseUI(layer, JsonUtility.FromJson<PanelCollection>(txt.text),
               new ViewBridge(PanelNames_UIGame.map),
               this);
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


    void IUIDelegate.OnTopShowChange(int layer, string path)
    {
        Log.L("OnTopShowChange");
    }

    void IUIDelegate.OnClosePanelAsync(string path)
    {
        Log.L("OnClosePanelAsync");
    }

    void IUIDelegate.OnHidePanelAsync(string path)
    {
        Log.L("OnHidePanelAsync");
    }

    void IUIDelegate.OnShowPanelRequest(string path)
    {
        Log.L("OnShowPanelRequest");
    }


}
