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
    public class UIItemViewCollection
    {
        private readonly UIModule ui;

        public UIItemViewCollection(UIModule ui)
        {
            this.ui = ui;
        }
        public UIItemView Get(Type type,string path, Transform parent)
        {
            var op = ui.GetItem(path);
            var ins = Activator.CreateInstance(type) as UIItemView;
            ins.Load(path, op, parent);
            return ins;
        }
        public T Get<T>(string path, Transform parent) where T : UIItemView, new()
        {
            var op = ui.GetItem(path);
            var ins = new T();
            ins.Load(path, op, parent);
            return ins;
        }
        public void Set(UIItemView view)
        {
            this.ui.SetItem(view.path, view.gameObject);
            view.SetActive(false);
            view.OnSet();
        }
    }
}
