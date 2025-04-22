/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.318
 *UnityVersion:   2018.4.17f1
 *Date:           2020-06-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System;
using IFramework.UI;

namespace IFramework.Lua
{
    public class LuaBridge : IViewBridge
    {
        public event Action onDispose;
        public event Func<string,UIPanel, bool> onSubscribe;
        public event Func<string, bool> onUnSubscribe;
        public event Action<string> onLoad;
        public event Action<string> onShow;
        public event Action<string> onHide;
        public event Action<string> onClose;
        public event Action<string> onBecameVisible;
        public event Action<string> onBecameInvisible;

        void IDisposable.Dispose()
        {
            if (onDispose != null)
            {
                onDispose();
            }
            onDispose = null;
            onSubscribe = null;
            onUnSubscribe = null;
        }

        void IViewBridge.OnLoad(string path) => onLoad?.Invoke(path);
        void IViewBridge.OnClose(string path) => onClose?.Invoke(path);
        void IViewBridge.OnHide(string path)=> onHide?.Invoke(path);
        void IViewBridge.OnShow(string path) => onShow?.Invoke(path);
        bool IViewBridge.Subscribe(string path,UIPanel panel) => onSubscribe != null ? onSubscribe(path,panel) : false;
        bool IViewBridge.UnSubscribe(string path) => onUnSubscribe != null ? onUnSubscribe(path) : false;

        void IViewBridge.OnBecameVisible(string path) => onBecameVisible ?.Invoke(path);

        void IViewBridge.OnBecameInvisible(string path) => onBecameInvisible?.Invoke(path);
    }

}
