﻿/*********************************************************************************
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

namespace IFramework.UI
{
    interface IWidgetPool
    {
        void Clear();
    }
    public interface IPoolAbleWidget
    {
        void OnSet();
    }
    public class WidgetPool<T> : IWidgetPool where T : GameObjectView
    {
        private GameObject prefab;
        private Transform parent;
        private Queue<T> classes = new Queue<T>();
        Func<T> createClass;
        protected Queue<GameObject> pool = new Queue<GameObject>();
        private GameObjectView parentView;

        public int count { get { return pool.Count; } }

        HideFlags _hideflag;
        public WidgetPool(GameObjectView parentView, GameObject prefab, Transform parent, Func<T> createClass, bool inParent)
        {
            this.parentView = parentView;
            if (inParent)
            {
                prefab.gameObject.SetActive(false);
                _hideflag = prefab.hideFlags;
                prefab.hideFlags = HideFlags.HideInHierarchy;
            }
            this.prefab = prefab;
            this.parent = parent;
            this.createClass = createClass;
        }

        private GameObject GetGameObject(Transform parent)
        {
            if (parent == null)
                parent = this.parent;
            GameObject result;
            if (pool.Count > 0)
            {
                result = pool.Dequeue();
                if (result.transform.parent != parent)
                    result.transform.SetParent(parent);
            }
            else
            {
                result = GameObject.Instantiate(prefab, parent);
                result.hideFlags = _hideflag;
            }
            result.SetActive(true);

            return result;
        }



        private void SetGameobject(GameObject t)
        {
            if (!pool.Contains(t))
            {
                t.gameObject.SetActive(false);

                pool.Enqueue(t);

            }

        }


        public void Clear()
        {
            classes.Clear();
            while (pool.Count > 0)
            {
                var t = pool.Dequeue();
                GameObject.Destroy(t);
            }
        }



        public T Get(Transform parent = null)
        {
            T t;
            if (classes.Count == 0)
                t = createClass();
            else
                t = classes.Dequeue();
            if (parent == null)
                parent = this.parent;
            var go = GetGameObject(parent);
            var tran = go.transform;
            var index = tran.GetSiblingIndex();
            if (tran.parent.childCount - 1 != index)
                tran.SetAsLastSibling();
            parentView.InitWidget(t, go);
            return t;
        }
        public void Set(T t)
        {
            (t as IPoolAbleWidget)?.OnSet();
            SetGameobject(t.gameObject);
            classes.Enqueue(t);
            t.ClearFields();
        }

    }
}
