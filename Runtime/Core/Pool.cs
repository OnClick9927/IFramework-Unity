using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
            if (t is null)
                return false;
            if (!pool.Contains(t))
            {
                if (!OnSet(t)) return false;
                if (t is IPoolObject)
                {
                    IPoolObject obj = t as IPoolObject;
                    obj.valid = false;
                    obj.OnSet();
                }
                pool.Enqueue(t);
                return true;
            }
            else
            {
                Log.FE("Set Err: Exist " + type);
                return false;
            }
        }


        public virtual void Clear()
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
                Log.FE($"{nameof(context)} is not {typeof(T)} is {(context == null ? "null" : context.GetType().ToString())}");
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
            if (toRelease is null) return;
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
        private static readonly bool clearOnRelease = RuntimeHelpers.IsReferenceOrContainsReferences<T>();
        private int length;

        protected override T[] CreateNew() => new T[length];

        public void SetLength(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            this.length = length;
        }
        public override T[] Get()
        {
            T[] t;
            int poolCount = pool.Count;
            t = null;
            for (int i = 0; i < poolCount; i++)
            {
                int itemLength = _lengthqueue.Dequeue();
                var item = pool.Dequeue();
                if (itemLength == length)
                {
                    t = item;
                    break;
                }
                _lengthqueue.Enqueue(itemLength);
                pool.Enqueue(item);
            }
            if (t == null)
            {
                t = CreateNew();
                OnCreate(t);
            }
            OnGet(t);
            return t;
        }

        public override bool Set(T[] t)
        {
            if (t == null)
                return false;
            if (!pool.Contains(t))
            {
                if (!OnSet(t)) return false;
                if (clearOnRelease)
                    Array.Clear(t, 0, t.Length);
                _lengthqueue.Enqueue(t.Length);
                pool.Enqueue(t);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Clear()
        {
            base.Clear();
            _lengthqueue.Clear();
        }
    }

}
