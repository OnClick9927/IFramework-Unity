/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-08-03
 *Description:    Description
 *History:        2022-08-03--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework.UI
{
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

        public void Dispose()
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
}
