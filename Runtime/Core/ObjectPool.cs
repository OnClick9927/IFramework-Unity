using System;
using System.Collections.Generic;

namespace IFramework
{
    /// <summary>
    /// 基础对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObjectPool<T> 
    {
        /// <summary>
        /// 数据容器
        /// </summary>
        protected Queue<T> pool { get { return _lazy.Value; } }
        private Lazy<Queue<T>> _lazy = new Lazy<Queue<T>>(() => { return new Queue<T>(); }, true);
        /// <summary>
        /// 自旋锁
        /// </summary>
        protected object para = new object();
        /// <summary>
        /// 存储数据类型
        /// </summary>
        public virtual Type type { get { return typeof(T); } }

        /// <summary>
        /// 池子数量
        /// </summary>
        public int count { get { return pool.Count; } }




        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 清除
        /// </summary>
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
        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="count"></param>
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
        /// <summary>
        /// 创建一个新对象
        /// </summary>
        /// <returns></returns>
        protected abstract T CreateNew();
        /// <summary>
        /// 数据被清除时
        /// </summary>
        /// <param name="t"></param>
        protected virtual void OnClear(T t) { }
        /// <summary>
        /// 数据被回收时，返回true可以回收
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected virtual bool OnSet(T t)
        {
            return true;
        }
        /// <summary>
        /// 数据被获取时
        /// </summary>
        /// <param name="t"></param>
        protected virtual void OnGet(T t) { }
        /// <summary>
        /// 数据被创建时
        /// </summary>
        /// <param name="t"></param>
        protected virtual void OnCreate(T t) { }
    }
}
