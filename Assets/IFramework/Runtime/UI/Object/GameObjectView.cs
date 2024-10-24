/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-08-03
 *Description:    Description
 *History:        2022-08-03--
*********************************************************************************/

using System;
using UnityEngine;
using static IFramework.Events;
using static IFramework.UI.UnityEventHelper;

namespace IFramework.UI
{
    public abstract class GameObjectView
    {
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
        protected void DisposeEvents()
        {
            if (_eventBox != null)
            {
                _eventBox.Dispose();
                _eventBox = null;
            }
        }
        protected void DisposeUIEvents()
        {
            if (_eventBox != null)
            {
                __eventBox_ui.Dispose();
                __eventBox_ui = null;
            }
        }
        public void AddUIEvent(UIEventEntity uiEvent)
        {
            if (__eventBox_ui == null)
                __eventBox_ui = new UIEventBox();
            uiEvent.AddTo(__eventBox_ui);
        }
        public void DisposeUIEvent(UIEventEntity uiEvent)
        {
            if (__eventBox_ui == null)return;
            __eventBox_ui.Dispose(uiEvent);
        }


        public GameObject gameObject { get; private set; }
        public Transform transform { get; private set; }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        public void SetGameObject(GameObject gameObject)
        {
            this.gameObject = gameObject;
            transform = gameObject.transform;
            InitComponents();
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
    }
}
