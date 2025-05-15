/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.115
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.UI;
using System;
using UnityEngine;

namespace IFramework.Lua
{
    public class LuaGame : Game, IUIDelegate
    {
        public LuaEnvConfig envConfig;

        public UIModule module;
        public TextAsset txt;
        public UILayerData layer;
        protected override void Init()
        {

        }



        protected override void Startup()
        {
            module=  base.modules.CreateModule<UIModule>();
            module.SetUIDelegate(this);
            module.SetAsset(new UIGame.Asset(layer, JsonUtility.FromJson<PanelCollection>(txt.text)));
            module.CreateCanvas();
            LuaHotFix.Instance.Init(envConfig, LoadScript);
        }

        private byte[] LoadScript(string path)
        {
            var handle = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            return handle.bytes;
        }
        void IUIDelegate.OnFullScreenCount(bool hide, int count)
        {
            Log.L("OnFullScreenCount");
        }
        public void OnVisibleChange(string path, bool visible)
        {
            Log.L("OnVisibleChange");
        }
        void IUIDelegate.OnLayerTopChange(int layer, string path)
        {
            Log.L("OnLayerTopChange");
        }

        void IUIDelegate.OnLayerTopShowChange(int layer, string path)
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

        public void OnTopShowChange(int layer, string path)
        {
        }
    }
}
