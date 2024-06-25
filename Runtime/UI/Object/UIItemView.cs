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

namespace IFramework.UI
{
    public abstract class UIItemView : GameObjectView
    {
        public Action completed;
        public bool _isDone = false;
        public string path { get; private set; }

        public bool isDone { get { return _isDone; } }

        private void Compelete()
        {
            _isDone = true;
            completed?.Invoke();
            completed = null;
        }
        public async void Load(string path, UIItemOperation op, Transform parent)
        {
            await op;
            var go = op.gameObject;
            if (parent != null)
            {
                go.transform.SetParent(parent.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
            }
            this.path = path;
            this.SetGameObject(go);
            go.SetActive(true);
            this.OnGet();
            Compelete();
        }
        public abstract void OnSet();

        protected abstract void OnGet();

    }
}
