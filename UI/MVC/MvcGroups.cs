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
        private Dictionary<string, UIView> _views = new Dictionary<string, UIView>();

        private Dictionary<string, Type> _typemap;

        public MvcGroups(params Dictionary<string, Type>[] maps)
        {
            if (maps!=null)
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
        private UIView FindView(string name)
        {
            if (!_views.ContainsKey(name))
                return null;
            return _views[name];
        }

        void IGroups.OnClose(string path)
        {
            (FindView(path) as IViewEventHandler).OnClose();
        }

        void IGroups.OnHide(string path)
        {
            (FindView(path) as IViewEventHandler).OnHide();
        }

        void IGroups.OnLoad(string path)
        {
            (FindView(path) as IViewEventHandler).OnLoad();

        }

        void IGroups.OnShow(string path)
        {
            (FindView(path) as IViewEventHandler).OnShow();

        }

        bool IGroups.Subscribe(string path, UIPanel panel)
        {
            var _view = FindView(path);
            if (_view != null)
            {
                UnityEngine.Debug.LogError(string.Format("Have Subscribe Panel Name: {0} ready", path));
                return false;
            }
            Type viewType;
            if (!_typemap.TryGetValue(path, out viewType))
            {
                return false;
            }
            _view = Activator.CreateInstance(viewType) as UIView;
            _view.SetGameObject(panel.gameObject);
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
