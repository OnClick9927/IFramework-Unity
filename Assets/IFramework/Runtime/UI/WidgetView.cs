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
using UnityEngine;
using static IFramework.UI.UnityEventHelper;
namespace IFramework.UI
{
    public abstract class WidgetView : GameObjectView, IUIEventOwner, IEventsOwner
    {



        public override void SetActive(bool active)
        {
            if (!_canvasGroup)
                gameObject.SetActive(active);
            else
            {
                _canvasGroup.interactable = active;
                _canvasGroup.blocksRaycasts = active;
                _canvasGroup.alpha = active ? 1 : 0;
            }
        }

        private CanvasGroup _canvasGroup;
        protected override void OnSetGameObject(GameObject gameObject)
        {
            ClearFields();
            base.OnSetGameObject(gameObject);
            _canvasGroup = GetComponent<CanvasGroup>(string.Empty);
        }







        public WidgetView parent { get; private set; }
        private WidgetView _root;
        public WidgetView root
        {
            get
            {
                if (gameObject == null) return null;
                //var tmp = this;
                //while (tmp.parent != null)
                //    tmp = tmp.parent;
                if (parent == null) return this;
                return _root;
            }
        }


        public T FindViewInParent<T>() where T : WidgetView
        {
            var tmp = this.parent;
            while (tmp != null)
            {
                if (tmp is T)
                    return tmp as T;
                tmp = tmp.parent;
            }
            return null;
        }
        public List<T> FindViewsInChildren<T>(List<T> result) where T : WidgetView
        {
            if (result == null) result = new List<T>();

            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];

                if (child is T)
                    result.Add(child as T);
                child.FindViewsInChildren(result);
            }
            return result;
        }
        public T FindViewInChildren<T>() where T : WidgetView
        {

            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];

                if (child is T)
                    return child as T;
                var inchild = child.FindViewInChildren<T>();
                if (inchild != null)
                    return inchild;
            }
            return null;
        }



        internal void SetParent(WidgetView parent)
        {
            var parent_last = this.parent;
            if (parent_last == parent) return;
            if (parent_last != null)
                parent_last.children.Remove(this);
            if (parent != null)
            {
                parent.children.Add(this);
                this.parent = parent;


                var tmp = parent;
                while (tmp.parent != null)
                    tmp = tmp.parent;
                _root = tmp;
            }
            else
            {
                Log.FE("SetParent   Parent Can Not Be Null");
            }
        }

        public void SetAsChild(WidgetView view) => view.SetParent(this);






        public void ClearWidgetPools()
        {
            if (widgetPools != null)
            {
                foreach (var pool in widgetPools.Values)
                {
                    pool.Clear();
                }
                widgetPools.Clear();
            }
        }
        protected virtual void OnClearFields() { }

        internal void ClearFields()
        {
            OnClearFields();
            DisposeChildren();
            this.DisposeEvents();
            this.DisposeUIEvents();
            ClearWidgetPools();
            ClearPrefabs();
            this.gameObject = null;
        }
        private void DisposeChildren()
        {
            if (children.Count > 0)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    child.ClearFields();
                }
                children.Clear();
            }
        }





        //protected IReadOnlyList<GameObjectView> GetChildren() => children;

        private List<WidgetView> children = new List<WidgetView>();


        protected T CreateWidget<T>(GameObject gameObject) where T : WidgetView, new()
        {
            T t = new T();
            InitWidget(t, gameObject);
            return t;
        }
        internal T InitWidget<T>(T view, GameObject gameObject) where T : WidgetView
        {
            view.SetParent(this);
            view.SetGameObject(gameObject);
            return view;
        }


        private Dictionary<GameObject, IWidgetPool> widgetPools;
        public WidgetPool<T> CreateWidgetPool<T>(WidgetView parentView, GameObject prefab, Transform parent, Func<T> createClass) where T : WidgetView
        {
            if (widgetPools == null) widgetPools = new Dictionary<GameObject, IWidgetPool>();
            if (widgetPools.TryGetValue(prefab, out IWidgetPool pool))
            {
                return pool as WidgetPool<T>;
            }
            else
            {
                var _pool = WidgetPool<T>.Allocate(parentView, prefab, parent, createClass, prefab.transform.IsChildOf(transform));
                widgetPools[prefab] = _pool;
                return _pool;
            }
        }
        public WidgetPool<T> CreateWidgetPool<T>(WidgetView parentView, GameObject prefab, Transform parent) where T : WidgetView, new() => CreateWidgetPool<T>(parentView, prefab, parent, () => new T());
        public WidgetPool<T> CreateWidgetPool<T>(GameObject prefab, Transform parent, Func<T> createClass) where T : WidgetView => CreateWidgetPool<T>(this, prefab, parent, createClass);
        public WidgetPool<T> CreateWidgetPool<T>(GameObject prefab, Transform parent) where T : WidgetView, new() => CreateWidgetPool<T>(this, prefab, parent);


        public WidgetPool<T> FindWidgetPool<T>(GameObject prefab) where T : WidgetView
        {
            if (widgetPools == null) return null;
            if (widgetPools.TryGetValue(prefab, out IWidgetPool pool))
                return pool as WidgetPool<T>;
            return null;
        }

    }
}
