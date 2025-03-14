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
using static IFramework.Events;
using static IFramework.UI.UnityEventHelper;

namespace IFramework.UI
{
    public abstract class GameObjectView
    {

        public GameObjectView parent { get; private set; }
        public GameObjectView root
        {
            get
            {
                var tmp = this;
                while (tmp.parent != null)
                    tmp = tmp.parent;
                return tmp;
            }
        }


        internal void SetParent(GameObjectView parent)
        {
            this.parent = parent;
        }

        private ScriptCreatorContext _context;

        private ScriptCreatorContext context
        {
            get
            {
                if (_context == null)
                    _context = gameObject.GetComponent<ScriptCreatorContext>();
                return _context;
            }
        }
        Dictionary<string, GameObject> _prefabsName;


        public GameObject FindPrefab(string name)
        {
            if (_prefabsName == null) _prefabsName = new Dictionary<string, GameObject>();
            GameObject prefab = null;
            if (_prefabsName.TryGetValue(name, out prefab))
            {
                return prefab;
            }
            prefab = context.FindPrefab(name);
            if (prefab != null)
            {
                _prefabsName[name] = prefab;
            }
            else
            {
                Log.FE($"Not Find Prefab with Name {name} in {context.name}");
            }

            return prefab;
        }

        private EventBox _eventBox;
        private UIEventBox __eventBox_ui;
        protected EventEntity SubscribeEvent(string msg, Action<IEventArgs> action)
        {
            if (_eventBox == null)
                _eventBox = new EventBox();
            return _eventBox.Subscribe(msg, action);
        }
        protected void UnSubscribeEvent(string msg, Action<IEventArgs> action)
        {
            if (_eventBox == null) return;
            _eventBox.UnSubscribe(msg, action);
        }
        protected void UnSubscribeEvent(EventEntity entity)
        {
            if (_eventBox == null) return;
            _eventBox.UnSubscribe(entity);
        }
        public void DisposeEvents()
        {
            if (_eventBox != null)
            {
                _eventBox.Dispose();
                _eventBox = null;
            }
        }
        public void DisposeUIEvents()
        {
            if (__eventBox_ui != null)
            {
                __eventBox_ui.Dispose();
                __eventBox_ui = null;
            }
        }
        public void ClearPrefabs()
        {
            if (pools != null)
            {
                foreach (var pool in pools.Values)
                {
                    pool.Clear();
                }
                pools.Clear();
            }
            if (_prefabsName != null)
            {
                _prefabsName.Clear();
            }
        }



        internal void AddUIEvent(UIEventEntity uiEvent)
        {
            if (__eventBox_ui == null)
                __eventBox_ui = new UIEventBox();
            uiEvent.AddTo(__eventBox_ui);
        }
        protected void DisposeUIEvent(UIEventEntity uiEvent)
        {
            if (__eventBox_ui == null) return;
            __eventBox_ui.Dispose(uiEvent);
        }


        public GameObject gameObject { get; private set; }
        public Transform transform { get; private set; }

        public virtual void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        public void SetGameObject(GameObject gameObject)
        {
            if (this.gameObject != gameObject)
            {
                this.gameObject = gameObject;
                transform = gameObject.transform;
                InitComponents();
            }
        }

        public Transform GetTransform(string path)
        {
            if (string.IsNullOrEmpty(path))
                return transform;
            return transform.Find(path);
        }
        public GameObject GetGameObject(string path)
        {
            return GetTransform(path)?.gameObject;
        }

        public T GetComponent<T>(string path)
        {
            var trans = GetTransform(path);
            if (trans != null)
                return trans.GetComponent<T>();
            return default;
        }
        protected abstract void InitComponents();



        public T CreateView<T>(GameObject gameObject) where T : GameObjectView, new()
        {
            T t = new T();
            InitView(t, gameObject);
            return t;
        }
        public T InitView<T>(T view, GameObject gameObject) where T : GameObjectView
        {
            view.SetParent(this);
            view.SetGameObject(gameObject);
            return view;
        }
        public void SetAsChild(GameObjectView view)
        {
            view.SetParent(this);
        }

        private Dictionary<GameObject, IItemPool> pools;
        public ItemPool<T> CreateItemPool<T>(GameObject prefab, Transform parent, Func<T> createClass, bool inParent = false) where T : GameObjectView
        {
            if (pools == null) pools = new Dictionary<GameObject, IItemPool>();
            if (pools.TryGetValue(prefab, out IItemPool pool))
            {
                return pool as ItemPool<T>;
            }
            else
            {
                var _pool = new ItemPool<T>(this, prefab, parent, createClass, inParent);
                pools[prefab] = _pool;
                return _pool;
            }
        }

        public ItemPool<T> FindItemPool<T>(GameObject prefab) where T : GameObjectView
        {
            if (pools == null) return null;
            if (pools.TryGetValue(prefab, out IItemPool pool))
            {
                return pool as ItemPool<T>;
            }
            return null;
        }

    }
}
