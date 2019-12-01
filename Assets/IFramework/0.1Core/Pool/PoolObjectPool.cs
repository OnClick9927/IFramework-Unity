/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-04-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework
{
    public class PoolObjectPool : CachePool<IPoolObject>,IDisposable
    {
        public PoolObjectPool() : base(new PoolObjectInnerPool(), new PoolObjectInnerPool(), true, 16) { }
        public PoolObjectPool(PoolObjectInnerPool runningPoool, PoolObjectInnerPool sleepPool) : this(runningPoool, sleepPool, true, 16) { }
        public PoolObjectPool(PoolObjectInnerPool runningPoool, PoolObjectInnerPool sleepPool, bool autoClear, int cacheCapcity):base(runningPoool,sleepPool,autoClear,cacheCapcity)
        {
        }

        public void AddCreater(PoolObjCreaterDel del) { (sleepPool as PoolObjectInnerPool).AddCreaterDel(del); }

        public delegate IPoolObject PoolObjCreaterDel(Type type, IEventArgs arg, params object[] param);

        public class PoolObjectInnerPool : CacheInnerPool<IPoolObject>
        {

            private List<PoolObjCreaterDel> CreaterDels;
            public void AddCreaterDel(PoolObjCreaterDel createrDel)
            {
                CreaterDels.Add(createrDel);
            }
            protected override void OnClear(IPoolObject t, IEventArgs arg, params object[] param)
            {
                base.OnClear(t, arg, param);
                if (IsSleepPool)
                    t.OnClear(arg, param);
            }
            protected override void OnCreate(IPoolObject t, IEventArgs arg, params object[] param)
            {
                base.OnCreate(t, arg, param);
                if (IsSleepPool)
                    t.OnCreate(arg, param);
            }
            protected override void OnGet(IPoolObject t, IEventArgs arg, params object[] param)
            {
                base.OnGet(t, arg, param);
                if (IsSleepPool)
                    t.OnGet(arg, param);
            }
            protected override bool OnSet(IPoolObject t, IEventArgs arg, params object[] param)
            {
                base.OnSet(t, arg, param);
                if (IsSleepPool)
                    t.OnSet(arg, param);
                return true;
            }
            protected override void OnDispose()
            {
                CreaterDels.Clear();
                CreaterDels = null;
                base.OnDispose();
            }
            protected override IPoolObject CreatNew(IEventArgs arg, params object[] param)
            {
                if (!IsSleepPool)
                    return default(IPoolObject);
                IPoolObject t = default(IPoolObject);
                bool have = false;
                for (int i = 0; i < CreaterDels.Count; i++)
                {
                    var o = CreaterDels[i].Invoke(type, arg, param);
                    if (o != null)
                    {
                        t = o;
                        have = true; break;
                    }
                }
                if (!have) t = Activator.CreateInstance(type) as IPoolObject;
                return t;

            }

            public PoolObjectInnerPool() { CreaterDels = new List<PoolObjCreaterDel>(); }
        }
    }

    public class PoolObjectPool<T> : CachePool<T>, IDisposable where T :IPoolObject
    {
        public PoolObjectPool() : base(new PoolObjectInnerPool<T>(), new PoolObjectInnerPool<T>(), true, 16) { }
        public PoolObjectPool(PoolObjectInnerPool<T> runningPoool, PoolObjectInnerPool<T> sleepPool) : this(runningPoool, sleepPool, true, 16) { }
        public PoolObjectPool(PoolObjectInnerPool<T> runningPoool, PoolObjectInnerPool<T> sleepPool, bool autoClear, int cacheCapcity) : base(runningPoool, sleepPool, autoClear, cacheCapcity)
        {
        }

        public void AddCreater(PoolObjCreaterDel<T> del) { (sleepPool as PoolObjectInnerPool<T>).AddCreaterDel(del); }


        public delegate Object PoolObjCreaterDel<Object>(Type type, IEventArgs arg, params object[] param);

        public class PoolObjectInnerPool<Object> : CacheInnerPool<Object> where Object : IPoolObject
        {
            private List<PoolObjCreaterDel<Object>> CreaterDels;
            public PoolObjectInnerPool() { CreaterDels = new List<PoolObjCreaterDel<Object>>(); }
            public void AddCreaterDel(PoolObjCreaterDel<Object> createrDel)
            {
                CreaterDels.Add(createrDel);
            }

            protected override void OnClear(Object t, IEventArgs arg, params object[] param)
            {
                base.OnClear(t, arg, param);
                if (IsSleepPool)
                    t.OnClear(arg, param);
            }
            protected override void OnGet(Object t, IEventArgs arg, params object[] param)
            {
                base.OnGet(t, arg, param);
                if (IsSleepPool)
                    t.OnGet(arg, param);
            }
            protected override bool OnSet(Object t, IEventArgs arg, params object[] param)
            {
                base.OnSet(t, arg, param);
                if (IsSleepPool)
                    t.OnSet(arg, param);
                return true;
            }
            protected override void OnCreate(Object t, IEventArgs arg, params object[] param)
            {
                base.OnCreate(t, arg, param);
                if (IsSleepPool)
                    t.OnCreate(arg, param);
            }
            protected override Object CreatNew(IEventArgs arg, params object[] param)
            {
                if (!IsSleepPool)
                    return default(Object);
                Object t = default(Object);
                bool have = false;
                for (int i = 0; i < CreaterDels.Count; i++)
                {
                    var o = CreaterDels[i].Invoke(type, arg, param);
                    if (o != null)
                    {
                        t = o;
                        have = true; break;
                    }
                }
                if (!have) t = (Object)Activator.CreateInstance(type);
                return t;
            }
            protected override void OnDispose()
            {
                CreaterDels.Clear();
                CreaterDels = null;
                base.OnDispose();
            }
        }
    }

}
