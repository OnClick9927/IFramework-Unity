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
    public abstract class GameObjectView : IUIEventOwner, IEventsOwner
    {
        public GameObject gameObject { get; private set; }
        public Transform transform { get; private set; }
        private IScriptCreatorContext context;

        Dictionary<string, GameObject> _prefabsName;
        public GameObject FindPrefab(string name)
        {
            if (_prefabsName == null) _prefabsName = new Dictionary<string, GameObject>();
            GameObject prefab = null;
            if (_prefabsName.TryGetValue(name, out prefab))
            {
                return prefab;
            }
            var prefabs = context.GetPrefabs();
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (prefabs[i].name != name) continue;
                prefab = prefabs[i];
                break;
            }
            if (prefab != null)
                _prefabsName[name] = prefab;
            else
                Log.FE($"Not Find Prefab with Name {name} in {context.name}");
            return prefab;

        }
        public virtual void SetActive(bool active) => gameObject.SetActive(active);
        public bool SetGameObject(GameObject gameObject)
        {
            if (gameObject == null)
            {
                Log.FE($"{GetType()}-->  Can not SetGameObject With Null GameObject");
                return false;
            }
            if (this.gameObject != gameObject)
            {
                ClearFields();
                this.gameObject = gameObject;
                transform = gameObject.transform;
                context = GetComponent<IScriptCreatorContext>(string.Empty);
                InitComponents();
                return true;
            }
            return false;
        }

        public Transform GetTransform(string path)
        {
            var tans = string.IsNullOrEmpty(path) ? transform : transform.Find(path);
            if (tans == null)
                Log.FE($"{GetType()}-->  GetTransform result is Null! path-->{path}");
            return tans;
        }

        public GameObject GetGameObject(string path) => GetTransform(path)?.gameObject;
        public T GetComponent<T>(string path)
        {
            var trans = GetTransform(path);
            if (trans != null)
                return trans.GetComponent<T>();
            return default;
        }
        protected abstract void InitComponents();




        public GameObjectView parent { get; private set; }
        private GameObjectView _root;
        public GameObjectView root
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


        public T FindViewInParent<T>() where T : GameObjectView
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
        public List<T> FindViewsInChildren<T>(List<T> result) where T : GameObjectView
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
        public T FindViewInChildren<T>() where T : GameObjectView
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



        internal void SetParent(GameObjectView parent)
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

        public void SetAsChild(GameObjectView view) => view.SetParent(this);






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
            if (_prefabsName != null)
            {
                _prefabsName.Clear();
            }
        }
        protected virtual void OnClearFields() { }

        public void ClearFields()
        {
            OnClearFields();
            DisposeChildren();
            this.DisposeEvents();
            this.DisposeUIEvents();
            ClearWidgetPools();
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

        private List<GameObjectView> children = new List<GameObjectView>();


        public T CreateWidget<T>(GameObject gameObject) where T : GameObjectView, new()
        {
            T t = new T();
            InitWidget(t, gameObject);
            return t;
        }
        internal T InitWidget<T>(T view, GameObject gameObject) where T : GameObjectView
        {
            view.SetParent(this);
            view.SetGameObject(gameObject);
            return view;
        }


        private Dictionary<GameObject, IWidgetPool> widgetPools;
        public WidgetPool<T> CreateWidgetPool<T>(GameObjectView parentView, GameObject prefab, Transform parent, Func<T> createClass) where T : GameObjectView
        {
            if (widgetPools == null) widgetPools = new Dictionary<GameObject, IWidgetPool>();
            if (widgetPools.TryGetValue(prefab, out IWidgetPool pool))
            {
                return pool as WidgetPool<T>;
            }
            else
            {
                var _pool = new WidgetPool<T>(parentView, prefab, parent, createClass, prefab.transform.IsChildOf(transform));
                widgetPools[prefab] = _pool;
                return _pool;
            }
        }
        public WidgetPool<T> CreateWidgetPool<T>(GameObjectView parentView, GameObject prefab, Transform parent) where T : GameObjectView, new() => CreateWidgetPool<T>(parentView, prefab, parent, () => new T());
        public WidgetPool<T> CreateWidgetPool<T>(GameObject prefab, Transform parent, Func<T> createClass) where T : GameObjectView => CreateWidgetPool<T>(this, prefab, parent, createClass);
        public WidgetPool<T> CreateWidgetPool<T>(GameObject prefab, Transform parent) where T : GameObjectView, new() => CreateWidgetPool<T>(this, prefab, parent);


        public WidgetPool<T> FindWidgetPool<T>(GameObject prefab) where T : GameObjectView
        {
            if (widgetPools == null) return null;
            if (widgetPools.TryGetValue(prefab, out IWidgetPool pool))
                return pool as WidgetPool<T>;
            return null;
        }

    }
}
