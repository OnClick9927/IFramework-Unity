using System.Collections.Generic;
using UnityEngine;

namespace IFramework
{
    class GameObjectPool : ServiceBase, IGameObjectPool
    {
        private class Pool : ObjectPool<GameObject>
        {
            public Transform parent;
            public GameObject prefab;
            protected override void OnGet(GameObject t)
            {
                base.OnGet(t);
                t.SetActive(true);
            }
            protected override bool OnSet(GameObject t)
            {
                t.SetActive(false);
                if (t.transform.parent != parent)
                {
                    t.transform.SetParent(parent);
                }
                return base.OnSet(t);
            }
            protected override GameObject CreateNew()
            {
                var go = GameObject.Instantiate(prefab, parent);
                go.hideFlags = HideFlags.None;
                return go;
            }

            internal void DestroyAll()
            {
                pool.Clear();
                GameObject.Destroy(parent.gameObject);
                parent = null;
                prefab = null;
            }
        }

        private UnityEngine.Transform root;
        private IGameObjectPoolAsset asset;

        public GameObjectPool(IGameObjectPoolAsset asset, Transform root)
        {
            this.asset = asset;
            this.root = root;
        }

        private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();
        private Pool GetPool(string key) => pools.TryGetValue(key, out var pool) ? pool : null;



        public void ClearAll()
        {
            foreach (var item in pools)
            {
                var pool = item.Value;
                var key = item.Key;
                asset.ReleaseAsset(key, pool.prefab);
                pool.DestroyAll();
                StaticPool.Set(pool);
            }
            pools.Clear();
        }
        public async AsyncTask<bool> Prepare(string key)
        {
            var pool = GetPool(key);
            if (pool != null) return true;
            if (asset == null) return false;
            var prefab = await asset.LoadAsset(key);
            if (prefab == null) return false;

            var parent = new GameObject(key);
            prefab = GameObject.Instantiate(prefab, parent.transform);
            prefab.gameObject.SetActive(false);
            prefab.hideFlags = HideFlags.HideInHierarchy;

            pool = StaticPool.Get<Pool>();
            parent.transform.SetParent(this.root);

            pool.parent = parent.transform;
            pool.prefab = prefab;

            pools[key] = pool;
            return true;
        }
        public void Clear(string key)
        {
            if (!pools.Remove(key, out var pool)) return;
            asset.ReleaseAsset(key, pool.prefab);
            pool.DestroyAll();
            StaticPool.Set(pool);
        }
        public async AsyncTask<T> Get<T>(string key) where T : GameObjectView, IPoolAbleGameObjectView, new()
        {
            var pool = GetPool(key);
            if (pool == null)
            {
                if (!await Prepare(key)) return null;
                pool = GetPool(key);
            }


            var view = StaticPool.Get<T>();
            view.PoolKey = key;
            view.SetGameObject(pool.Get());
            return view;
        }
        public void Set(IPoolAbleGameObjectView view)
        {
            var key = view.PoolKey;
            var objPool = GetPool(key);
            if (objPool != null)
                objPool.Set(view.gameObject);
            StaticPool.SetByRealType(view);
        }

        protected override void OnUse(IServiceCollection services)
        {

        }
        protected override void OnEnter(IServiceCollection services)
        {
            
        }
        protected override void OnQuit(IServiceCollection services)
        {

        }
    }

}
