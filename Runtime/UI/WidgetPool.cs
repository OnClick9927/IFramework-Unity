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

namespace IFramework.UI
{
    interface IWidgetPool
    {
        void Clear();
    }
    public class WidgetPool<T> : IWidgetPool where T : GameObjectView
    {
        private GameObject prefab;
        private Transform parent;
        private Queue<T> classes = new Queue<T>();
        Func<T> createClass;
        protected Queue<GameObject> pool = new Queue<GameObject>();
        private GameObjectView owner;

        public int count { get { return pool.Count; } }

        public WidgetPool(GameObjectView owner, GameObject prefab, Transform parent, Func<T> createClass, bool inParent)
        {
            this.owner = owner;
            if (inParent)
                SetGameobject(prefab);
            this.prefab = prefab;
            this.parent = parent;
            this.createClass = createClass;
        }

        private GameObject GetGameObject(Transform parent)
        {
            if (parent == null)
                parent = this.parent;
            GameObject t;
            if (pool.Count > 0)
            {
                t = pool.Dequeue();
                if (t.transform.parent != parent)
                    t.transform.SetParent(parent);
            }
            else
            {
                t = GameObject.Instantiate(prefab, parent);
            }
            t.gameObject.SetActive(true);

            return t;
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
            var go = GetGameObject(parent);
            var tran = go.transform;
            var index = tran.GetSiblingIndex();
            if (tran.parent.childCount - 1 != index)
                tran.SetAsLastSibling();
            owner.InitWidget(t, go);
            return t;
        }
        public void Set(T t)
        {
            classes.Enqueue(t);
            SetGameobject(t.gameObject);
            t.ClearFields();
        }

    }
}
