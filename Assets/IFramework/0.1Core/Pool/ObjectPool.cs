/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
namespace IFramework
{
    public delegate void ObjPoolEventDel<T>(T t, IEventArgs arg, params object[] param);

    public abstract class ObjectPool<T> : IDisposable
    {
        public event ObjPoolEventDel<T> OnClearObject;
        public event ObjPoolEventDel<T> OnGetObject;
        public event ObjPoolEventDel<T> OnSetObject;
        public event ObjPoolEventDel<T> OnCreateObject;

        public Type type { get { return typeof(T); } }
        protected int capcity;
        protected List<T> pool;
        protected LockParam lockParam;
        public int Capcity { get { return capcity; } set { capcity = value; } }
        public int Count { get { return pool.Count; } }
        public ObjectPool() { pool = new List<T>(); lockParam = new LockParam(); }
        public ObjectPool(int capcity) : this() { this.capcity = capcity; }
        public void Dispose()
        {

            OnDispose();
            pool = null;
            lockParam = null;
            OnClearObject = null;
            OnGetObject = null;
            OnSetObject = null;
            OnCreateObject = null;
        }



        public bool Contains(T t)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                return pool.Contains(t);
            }
        }
        public int IndexOf(T t)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                return pool.IndexOf(t);
            }
        }
        public void ForEach(Action<T> action)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                pool.ForEach(action);
            }
        }
        public bool Contains(Predicate<T> p)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                var list = pool.FindAll(p);
                return list != null && list.Count > 0;
            }
        }
        public T Peek()
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                T t = default(T);
                if (pool.Count > 0)
                {
                    t = pool.QueuePeek();
                }
                return t;
            }
        }

        public T Get(IEventArgs arg = null, params object[] param)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                T t;
                if (pool.Count > 0)
                {
                    t = pool.Dequeue();
                }
                else
                {
                    t = CreatNew(arg, param);
                    OnCreate(t, arg, param);
                }
                OnGet(t, arg, param);
                return t;
            }
        }
        public T Get(Predicate<T> p, IEventArgs arg = null, params object[] param)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                var ts = pool.FindAll(p);
                if (ts.Count < 0)
                {
                    T t = ts[0];
                    pool.Remove(t);
                    OnGet(t, arg, param);
                    return t;
                }
                return default(T);
            }
        }

        public bool Set(T t, IEventArgs arg = null, params object[] param)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                if (!pool.Contains(t))
                {
                    if (OnSet(t, arg, param))
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
        public bool Clear(T t, IEventArgs arg = null, params object[] param)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                if (pool.Contains(t))
                {
                    pool.Remove(t);
                    OnClear(t, arg, param);
                    return true;
                }
                else
                {
                    Log.E("Clear Err: Not Exist " + type);
                    return false;
                }
            }
        }
        public void Clear(IEventArgs arg = null, params object[] param)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                while (pool.Count > 0)
                {
                    var t = pool.Dequeue();
                    OnClear(t, arg, param);
                }
            }
        }
        public void Clear(int count, IEventArgs arg = null, params object[] param)
        {
            using (LockWait wait = new LockWait(ref lockParam))
            {
                count = count > pool.Count ? 0 : pool.Count - count;
                while (pool.Count > count)
                {
                    var t = pool.Dequeue();
                    OnClear(t, arg, param);
                }
            }
        }

        protected abstract T CreatNew(IEventArgs arg, params object[] param);
        protected virtual void OnClear(T t, IEventArgs arg, params object[] param)
        {
            if (OnClearObject != null) OnClearObject(t, arg, param);

        }
        protected virtual bool OnSet(T t, IEventArgs arg, params object[] param)
        {
            if (OnSetObject != null) OnSetObject(t, arg, param);
            return true;
        }
        protected virtual void OnGet(T t, IEventArgs arg, params object[] param)
        {
            if (OnGetObject != null) OnGetObject(t, arg, param);

        }
        protected virtual void OnCreate(T t, IEventArgs arg, params object[] param)
        {
            if (OnCreateObject != null) OnCreateObject(t, arg, param);
        }
        protected virtual void OnDispose()
        {
            Clear();
        }
    }
}
