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

namespace IFramework.UI
{
    /// <summary>
    /// ui 组
    /// </summary>
    public interface IViewBridge : IDisposable
    {

        /// <summary>
        /// 添加 ui 入组
        /// </summary>
        /// <param name="panel"></param>
        bool Subscribe(string path, UIPanel panel);
        /// <summary>
        /// 移除 ui
        /// </summary>
        /// <param name="panel"></param>
        bool UnSubscribe(string path);
        /// <summary>
        /// 加载完毕
        /// </summary>
        /// <param name="name"></param>
        void OnLoad(string path);
        /// <summary>
        /// 要求显示
        /// </summary>
        /// <param name="name"></param>
        void OnShow(string path);
        /// <summary>
        /// 要求隐藏
        /// </summary>
        /// <param name="name"></param>
        void OnHide(string path);
        /// <summary>
        /// 要求关闭
        /// </summary>
        /// <param name="name"></param>
        void OnClose(string path);

        void OnBecameVisible(string path);
        void OnBecameInvisible(string path);

    }

    public class ViewBridge : IViewBridge
    {
        private Dictionary<string, IUIView> _views = new Dictionary<string, IUIView>();

        private Dictionary<string, Type> _typemap;

        public ViewBridge(params Dictionary<string, Type>[] maps)
        {
            if (maps != null)
            {
                _typemap = new Dictionary<string, Type>();
                foreach (var item in maps)
                {
                    foreach (var _item in item)
                    {
                        this._typemap.Add(_item.Key, _item.Value);
                    }
                }
            }
        }

        void IDisposable.Dispose()
        {
        }

        void IViewBridge.OnBecameInvisible(string path) => FindView(path).OnBecameInvisible();

        void IViewBridge.OnBecameVisible(string path) => FindView(path).OnBecameVisible();

        private IUIView FindView(string name)
        {
            _views.TryGetValue(name, out IUIView view);
            return view;
        }

        void IViewBridge.OnClose(string path) => FindView(path).OnClose();

        void IViewBridge.OnHide(string path) => FindView(path).OnHide();

        void IViewBridge.OnLoad(string path) => FindView(path).OnLoad();

        void IViewBridge.OnShow(string path) => FindView(path).OnShow();

        bool IViewBridge.Subscribe(string path, UIPanel panel)
        {
            var _view = FindView(path);
            if (_view != null)
            {
                UnityEngine.Debug.LogError($"Have Subscribe Panel Name: {path} ready");
                return false;
            }
            Type viewType;
            if (!_typemap.TryGetValue(path, out viewType))
            {
                UnityEngine.Debug.LogError($"Panel Subscribe Failed : No View Script {path}");
                return false;
            }
            UIView ui_view = Activator.CreateInstance(viewType) as UIView;
            ui_view.SetPanel(panel);
            _view = ui_view;
            _views.Add(path, _view);
            return true;
        }

        bool IViewBridge.UnSubscribe(string path)
        {
            var view = FindView(path);
            if (view != null)
            {
                _views.Remove(path);
                return true;
            }
            return false;
        }
    }
    public class MixedViewBridge : IViewBridge
    {
        private readonly IViewBridge[] _bridges;
        private Dictionary<string, IViewBridge> _nameMap;
        public MixedViewBridge(IViewBridge[] bridges)
        {
            this._bridges = bridges;
            _nameMap = new Dictionary<string, IViewBridge>();
        }

        void IDisposable.Dispose()
        {
            for (int i = 0; i < _bridges.Length; i++)
            {
                _bridges[i].Dispose();
            }
        }


        void IViewBridge.OnClose(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                _nameMap[path].OnClose(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
            }
        }

        void IViewBridge.OnHide(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                _nameMap[path].OnHide(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
            }
        }

        void IViewBridge.OnShow(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                _nameMap[path].OnShow(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
            }
        }
        bool IViewBridge.Subscribe(string path, UIPanel panel)
        {
            bool sucess = false;
            for (int i = 0; i < _bridges.Length; i++)
            {
                sucess |= _bridges[i].Subscribe(path, panel);
                if (sucess)
                {
                    if (_nameMap.ContainsKey(path))
                    {
                        UnityEngine.Debug.LogError("Same name, can't Subscribe the panel with name " + path);
                        return false;
                    }
                    _nameMap[path] = _bridges[i];
                    break;
                }
            }
            if (!sucess)
            {
                UnityEngine.Debug.LogError("can't Subscribe the panel with name " + path);
            }
            return sucess;
        }

        bool IViewBridge.UnSubscribe(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                return _nameMap[path].UnSubscribe(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
                return false;
            }
        }

        void IViewBridge.OnLoad(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                _nameMap[path].OnLoad(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
            }
        }

        void IViewBridge.OnBecameVisible(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                _nameMap[path].OnBecameVisible(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
            }
        }

        void IViewBridge.OnBecameInvisible(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                _nameMap[path].OnBecameInvisible(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
            }
        }
    }

}
