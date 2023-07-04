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

namespace IFramework.Hotfix.Lua
{
    public class LuaGroups : IGroups
    {
        public event Action onDispose;
        public event Func<string,UIPanel, bool> onSubscribe;
        public event Func<string, bool> onUnSubscribe;
        public event Action<string> onLoad;
        public event Action<string> onShow;
        public event Action<string> onHide;
        public event Action<string> onClose;

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

        void IGroups.OnLoad(string path) => onLoad?.Invoke(path);
        void IGroups.OnClose(string path) => onClose?.Invoke(path);
        void IGroups.OnHide(string path)=> onHide?.Invoke(path);
        void IGroups.OnShow(string path) => onShow?.Invoke(path);
        bool IGroups.Subscribe(string path,UIPanel panel) => onSubscribe != null ? onSubscribe(path,panel) : false;
        bool IGroups.UnSubscribe(string path) => onUnSubscribe != null ? onUnSubscribe(path) : false;
    }

}
