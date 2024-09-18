/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
namespace IFramework.UI
{
    class ItemsPool
    {
        private Dictionary<string, ItemPool> pools = new Dictionary<string, ItemPool>();
        private UIModule module;
        private Dictionary<string, List<UIItemOperation>> works = new Dictionary<string, List<UIItemOperation>>();
        public ItemsPool(UIModule module)
        {
            this.module = module;
        }
        public UIItemOperation Get(string path)
        {
            UIItemOperation item;
            if (pools.ContainsKey(path))
                item = pools[path].Get();
            else
                item = LoadPrefab(path);
            if (!works.ContainsKey(path))
                works.Add(path, new List<UIItemOperation>());
            if (!works[path].Contains(item))
                works[path].Add(item);
            return item;
        }
        private UIItemOperation LoadPrefab(string path)
        {
            var _loader = module._asset;
            LoadItemAsyncOperation op = new LoadItemAsyncOperation();
            op.path = path;
            ItemPool pool = new ItemPool(op, module);
            pools.Add(path, pool);
            if (_loader.LoadItemAsync(op))
            {
                return pool.Get();
            }
            throw new Exception($"can not load {path}");
        }
        public void Set(string path, UIItemOperation item)
        {
            if (works.ContainsKey(path))
            {
                works[path].Remove(item);
            }
            pools[path].Set(item);
        }
        public void Set(string path, GameObject go)
        {
            if (works.ContainsKey(path))
            {
                var item = works[path].Find((_item) => { return _item.gameObject == go; });
                Set(path, item);
            }
        }
        public void Clear()
        {
            foreach (var item in pools.Values)
            {
                item.Clear();
            }
            works.Clear();
            foreach (var item in pools.Keys)
            {
                module._asset.ReleaseItemAsset(pools[item].prefab);
            }
        }


        Queue<ItemPool> clear = new Queue<ItemPool>();
        public void ClearUseless()
        {
            foreach (var item in pools.Keys)
            {
                if (!works.ContainsKey(item))
                {
                    clear.Enqueue(pools[item]);
                }
            }
            while (clear.Count != 0)
            {
                var pool = clear.Dequeue();
                pools.Remove(pool.path);
                pool.Clear();
                module._asset.ReleaseItemAsset(pool.prefab);
            }
        }
    }
}
