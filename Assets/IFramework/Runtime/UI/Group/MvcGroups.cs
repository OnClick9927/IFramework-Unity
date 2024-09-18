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

namespace IFramework.UI.MVC
{
    public class MvcGroups : IGroups
    {
        private Dictionary<string, IViewEventHandler> _views = new Dictionary<string, IViewEventHandler>();

        private Dictionary<string, Type> _typemap;

        public MvcGroups(params Dictionary<string, Type>[] maps)
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
        private IViewEventHandler FindView(string name)
        {
            _views.TryGetValue(name, out IViewEventHandler view);
            return view;
        }

        void IGroups.OnClose(string path) => (FindView(path) as IViewEventHandler).OnClose();

        void IGroups.OnHide(string path) => (FindView(path) as IViewEventHandler).OnHide();

        void IGroups.OnLoad(string path) => (FindView(path) as IViewEventHandler).OnLoad();

        void IGroups.OnShow(string path) => (FindView(path) as IViewEventHandler).OnShow();

        bool IGroups.Subscribe(string path, UIPanel panel)
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
            ui_view.SetGameObject(panel.gameObject);
            _view = ui_view;
            _views.Add(path, _view);
            return true;
        }

        bool IGroups.UnSubscribe(string path)
        {
            var group = FindView(path);
            if (group != null)
            {
                _views.Remove(path);
                return true;
            }
            return false;
        }
    }
}
