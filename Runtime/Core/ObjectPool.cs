using System;
using System.Collections.Generic;

namespace IFramework
{

    public abstract class ObjectPool<T> 
    {

        protected Queue<T> pool { get { return _lazy.Value; } }
        private Lazy<Queue<T>> _lazy = new Lazy<Queue<T>>(() => { return new Queue<T>(); }, true);
 
        protected object para = new object();

        public virtual Type type { get { return typeof(T); } }


        public int count { get { return pool.Count; } }




        public virtual T Get()
        {
            lock (para)
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
                OnGet(t);
                return t;
            }
        }

        public virtual bool Set(T t)
        {
            lock (para)
            {
                if (!pool.Contains(t))
                {
                    if (OnSet(t))
                    {
                        pool.Enqueue(t);
                    }
                    return true;
                }
                else
                {
                    Log.E("Set Err: Exist " + type);
                    return false;
                }
            }
        }


        public void Clear()
        {
            lock (para)
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
        }

        public void Clear(int count)
        {
            lock (para)
            {
                count = count > pool.Count ? 0 : pool.Count - count;
                while (pool.Count > count)
                {
                    var t = pool.Dequeue();
                    OnClear(t);
                }
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
}
