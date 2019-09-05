/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-12
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework
{
    public abstract class CachePoolInnerPool<V>:ObjectPool<V>
    {
        public bool IsSleepPool { get; set; }
    }
    public class CachePool<T> : IDisposable
    {
       
        public event ObjPoolEventDel<T> OnClearObject { add { SleepPool.OnClearObject += value; } remove { SleepPool.OnClearObject -= value; } }
        public event ObjPoolEventDel<T> OnGetObject { add { SleepPool.OnGetObject += value; } remove { SleepPool.OnGetObject -= value; } }
        public event ObjPoolEventDel<T> OnSetObject { add { SleepPool.OnSetObject += value; } remove { SleepPool.OnSetObject -= value; } }
        public event ObjPoolEventDel<T> OnCreateObject { add { SleepPool.OnCreateObject += value; } remove { SleepPool.OnCreateObject -= value; } }
        public CachePool(CachePoolInnerPool<T> runningPoool, CachePoolInnerPool<T> sleepPool) : this(runningPoool, sleepPool, true, 16) { }
        public CachePool(CachePoolInnerPool<T> runningPoool, CachePoolInnerPool<T> sleepPool, bool autoClear, int cacheCapcity)
        {
            this.autoClear = autoClear;
            this.sleepCapcity = cacheCapcity;
            this.RunningPoool = runningPoool;
            this.SleepPool = sleepPool;
        }
        public virtual void Dispose()
        {
            CycleRunningPool(null);
            SleepPool.Dispose();
            RunningPoool.Dispose();
        }

        public Type type { get { return typeof(T); } }
        private int sleepCapcity;
        private bool autoClear;
        public bool AutoClear { get { return autoClear; } set { autoClear = value; } }
        public int SleepCapcity { get { return sleepCapcity; } set { sleepCapcity = value; } }

        protected CachePoolInnerPool<T> sleepPool;
        protected CachePoolInnerPool<T> runningPoool;
        public virtual CachePoolInnerPool<T> RunningPoool { get { return runningPoool; }set { runningPoool = value;runningPoool.IsSleepPool = false; } }
        public virtual CachePoolInnerPool<T> SleepPool { get { return sleepPool; } set { sleepPool = value; sleepPool.IsSleepPool = true; } }

        public int SleepCount { get { return SleepPool.Count; } }
        public int RunningCount { get { return RunningPoool.Count; } }

        public bool IsRunning(T t) { return RunningPoool.Contains(t); }
        public bool IsSleep(T t) { return SleepPool.Contains(t); }
        public bool Contains(T t) { return IsRunning(t) || IsSleep(t); }


        public T Get()
        {
            T t = SleepPool.Get();
            RunningPoool.Set(t);
            return t;
        }
        public void Set(T t)
        {
            RunningPoool.Clear(t);
            SleepPool.Set(t);
            AutoClean();
        }
        public T Get(IEventArgs arg, params object[] param)
        {
            T t = SleepPool.Get(arg, param);
            RunningPoool.Set(t);
            return t;
        }
        public void Set(T t, IEventArgs arg, params object[] param)
        {
            RunningPoool.Clear(t);
            SleepPool.Set(t, arg, param);
            AutoClean();
        }
        private void AutoClean()
        {
            if (!autoClear) return;
            SleepPool.Clear(SleepPool.Count - SleepCapcity);
        }

        public void Clear(T t, bool ignoreRun = true)
        {
            if (IsRunning(t))
            {
                if (ignoreRun) return;
                Set(t);
            }
            SleepPool.Clear(t);
        }
        public void Clear(T[] ts, bool ignoreRun = true)
        {
            ts.ForEach((t) =>
            {
                Clear(t, ignoreRun);
            });
        }
        public void Clear(T t, IEventArgs arg, bool ignoreRun = true, params object[] param)
        {
            if (IsRunning(t))
            {
                if (ignoreRun) return;
                Set(t, arg, param);
            }
            SleepPool.Clear(t, arg, param);
        }
        public void Clear(T[] ts, IEventArgs arg, bool ignoreRun = true, params object[] param)
        {
            ts.ForEach((t) =>
            {
                Clear(t, arg, ignoreRun, param);
            });
        }


        public void CycleRunningPool()
        {
            while (RunningPoool.Count > 0)
            {
                T t = RunningPoool.Get();
                SleepPool.Set(t);
                AutoClean();
            }
        }
        public void ClearSleepPool()
        {
            SleepPool.Clear();
        }
        public void CycleRunningPool(IEventArgs arg, params object[] param)
        {
            while (RunningPoool.Count > 0)
            {
                T t = RunningPoool.Get();
                SleepPool.Set(t, arg, param);
                AutoClean();
            }
        }
        public void ClearSleepPool(IEventArgs arg, params object[] param)
        {
            SleepPool.Clear(arg, param);
        }
    }

}
