using System;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework
{

    public interface IPoolObject
    {
        bool valid { get; set; }
        void OnGet();
        void OnSet();
    }
    public abstract class ObjectPool<T>
    {

        protected Queue<T> pool { get { return _lazy.Value; } }
        private Lazy<Queue<T>> _lazy = new Lazy<Queue<T>>(() => { return new Queue<T>(); }, true);

        public virtual Type type { get { return typeof(T); } }


        public int count { get { return pool.Count; } }




        public virtual T Get()
        {
            T t;
            if (pool.Count > 0)
            {
                t = pool.Dequeue();
            }
            else
            {
                t = CreateNew();
                OnCreate(t);
            }
            if (t is IPoolObject)
            {
                IPoolObject obj = t as IPoolObject;
                obj.valid = true;
                obj.OnGet();
            }
            OnGet(t);

            return t;
        }

        public virtual bool Set(T t)
        {
            if (!pool.Contains(t))
            {
                if (OnSet(t))
                {
                    if (t is IPoolObject)
                    {
                        IPoolObject obj = t as IPoolObject;
                        obj.valid = false;
                        obj.OnSet();
                    }
                    pool.Enqueue(t);
                }
                return true;
            }
            else
            {
                Log.FE("Set Err: Exist " + type);
                return false;
            }
        }


        public void Clear()
        {
            while (pool.Count > 0)
            {
                var t = pool.Dequeue();
                OnClear(t);
                IDisposable dispose = t as IDisposable;
                if (dispose != null)
                    dispose.Dispose();
            }
        }

        protected abstract T CreateNew();

        protected virtual void OnClear(T t) { }

        protected virtual bool OnSet(T t)
        {
            return true;
        }

        protected virtual void OnGet(T t) { }

        protected virtual void OnCreate(T t) { }
    }
    public interface ISimpleObjectPool
    {
        void SetObject(object context);
    }
    public sealed class SimpleObjectPool<T> : ObjectPool<T>, ISimpleObjectPool where T : class, new()
    {
        public void SetObject(object context)
        {
            if (!(context is T))
            {
                Log.FE($"{nameof(context)} is not {typeof(T)} is {context.GetType()}");
                return;
            }
            base.Set(context as T);
        }

        protected override T CreateNew()
        {
            return new T();
        }
    }


    public class StaticPool
    {
        private static Dictionary<Type, ISimpleObjectPool> map = new Dictionary<Type, ISimpleObjectPool>();
        class Pool<T> where T : class, new()
        {
            internal static readonly SimpleObjectPool<T> s_Pool = new SimpleObjectPool<T>();
            static Pool()
            {
                map[typeof(T)] = s_Pool;
            }
            public static T Get() => s_Pool.Get();

            public static void Set(T toRelease) => s_Pool.SetObject(toRelease);

        }
        class ArrPool<T>
        {
            internal static readonly ArrayPool<T> s_array_Pool = new ArrayPool<T>();
            public static T[] Get(int length)
            {
                s_array_Pool.SetLength(length);
                return s_array_Pool.Get();
            }
            public static void Set(T[] toRelease) => s_array_Pool.Set(toRelease);

        }

        public interface IDisposableValue<T> : IDisposable
        {
            T value { get; }

        }

        struct StaticPoolValue<T> : IDisposableValue<T> where T : class, new()
        {
            public T value { get; private set; }


            public StaticPoolValue(bool ignore = true) => value = StaticPool.Get<T>();

            public void Dispose() => StaticPool.Set(value);
        }
        struct StaticPoolArray<T> : IDisposableValue<T[]>
        {
            public T[] value { get; private set; }
            public StaticPoolArray(int length) => value = StaticPool.GetArray<T>(length);

            public void Dispose() => StaticPool.Set(value);
        }
        public static T[] GetArray<T>(int length) => ArrPool<T>.Get(length);

        public static void Set<T>(T[] toRelease) => ArrPool<T>.Set(toRelease);

        public static T Get<T>() where T : class, new() => Pool<T>.Get();

        public static void Set<T>(T toRelease) where T : class, new() => Pool<T>.Set(toRelease);

        //public static T Get<T>() where T : class, new() => Pool<T>.Get();

        public static void SetByRealType<T>(T toRelease)
        {
            var type = toRelease.GetType();
            if (map.TryGetValue(type, out var result))
            {
                result.SetObject(toRelease);
            }
        }

        public static IDisposableValue<T> CreateDisposable<T>() where T : class, new() => new StaticPoolValue<T>(true);
        public static IDisposableValue<T[]> CreateDisposableArray<T>(int length) => new StaticPoolArray<T>(length);



    }

    public class ArrayPool<T> : ObjectPool<T[]>
    {
        private Queue<int> _lengthqueue = new Queue<int>();
        private int length;

        protected override T[] CreateNew() => new T[length];
        Queue<T[]> queue = new Queue<T[]>();

        public void SetLength(int length)
        {
            this.length = length;
        }
        public override T[] Get()
        {
            T[] t;
            if (pool.Count > 0 && _lengthqueue.Contains(length))
            {
                while (_lengthqueue.Peek() != length)
                {
                    _lengthqueue.Dequeue();
                    queue.Enqueue(pool.Dequeue());
                }
                t = pool.Dequeue();
                while (pool.Count != 0) queue.Enqueue(pool.Dequeue());
                int _count = queue.Count;
                for (int i = 0; i < _count; i++)
                {
                    var tmp = queue.Dequeue();
                    int _len = tmp.Length;
                    _lengthqueue.Enqueue(_len);
                    pool.Enqueue(tmp);
                }
            }
            else
            {
                t = CreateNew();
                OnCreate(t);
            }
            OnGet(t);
            return t;
        }

        public override bool Set(T[] t)
        {
            if (!pool.Contains(t))
            {
                if (OnSet(t))
                {
                    int _len = t.Length;
                    _lengthqueue.Enqueue(_len);
                    pool.Enqueue(t);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public interface IPoolAbleGameObjectView
    {
        string PoolKey { get; set; }
        GameObject gameObject { get; }
    }
    public interface IGameObjectPoolAsset
    {
        AsyncTask<GameObject> LoadAsset(string key);
        void ReleaseAsset(string key, GameObject asset);
    }
    [AddComponentMenu("")]
    [DynamicMonoSingleton]
    public class GameObjectPool : MonoSingleton<GameObjectPool>
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


        private IGameObjectPoolAsset asset;
        private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();
        private Pool GetPool(string key) => pools.TryGetValue(key, out var pool) ? pool : null;
        public void SetAsset(IGameObjectPoolAsset asset)
        {
            this.asset = asset;
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
            parent.transform.SetParent(this.transform);

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
    }

}
