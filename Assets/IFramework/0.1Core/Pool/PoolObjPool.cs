/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-04-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework
{
    public class PoolObjPool : CachePool<IPoolObject>,IDisposable
    {
        public void AddCreater(PoolObjCreaterDel del) { (sleepPool as PoolObjCollecter).AddCreaterDel(del); }
        public PoolObjPool() : base(new PoolObjCollecter(), new PoolObjCollecter(), true, 16) { }
        public PoolObjPool(PoolObjCollecter runningPoool, PoolObjCollecter sleepPool) : this(runningPoool, sleepPool, true, 16) { }
        public PoolObjPool(PoolObjCollecter runningPoool, PoolObjCollecter sleepPool, bool autoClear, int cacheCapcity):base(runningPoool,sleepPool,autoClear,cacheCapcity)
        {
        }

        public delegate IPoolObject PoolObjCreaterDel(Type type, IEventArgs arg, params object[] param);

        public class PoolObjCollecter : CachePoolInnerPool<IPoolObject>
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

            public PoolObjCollecter() { CreaterDels = new List<PoolObjCreaterDel>(); }
        }
    }

    public class PoolObjPool<T> : CachePool<T>, IDisposable where T :IPoolObject
    {
        public void AddCreater(PoolObjCreaterDel<T> del) { (sleepPool as PoolObjCollecter<T>).AddCreaterDel(del); }
        public PoolObjPool() : base(new PoolObjCollecter<T>(), new PoolObjCollecter<T>(), true, 16) { }
        public PoolObjPool(PoolObjCollecter<T> runningPoool, PoolObjCollecter<T> sleepPool) : this(runningPoool, sleepPool, true, 16) { }
        public PoolObjPool(PoolObjCollecter<T> runningPoool, PoolObjCollecter<T> sleepPool, bool autoClear, int cacheCapcity) : base(runningPoool, sleepPool, autoClear, cacheCapcity)
        {
        }

        public delegate Obj PoolObjCreaterDel<Obj>(Type type, IEventArgs arg, params object[] param);
        public class PoolObjCollecter<Obj> : CachePoolInnerPool<Obj> where Obj : IPoolObject
        {
            private List<PoolObjCreaterDel<Obj>> CreaterDels;
            public PoolObjCollecter() { CreaterDels = new List<PoolObjCreaterDel<Obj>>(); }
            public void AddCreaterDel(PoolObjCreaterDel<Obj> createrDel)
            {
                CreaterDels.Add(createrDel);
            }

            protected override void OnClear(Obj t, IEventArgs arg, params object[] param)
            {
                base.OnClear(t, arg, param);
                if (IsSleepPool)
                    t.OnClear(arg, param);
            }
            protected override void OnGet(Obj t, IEventArgs arg, params object[] param)
            {
                base.OnGet(t, arg, param);
                if (IsSleepPool)
                    t.OnGet(arg, param);
            }
            protected override bool OnSet(Obj t, IEventArgs arg, params object[] param)
            {
                base.OnSet(t, arg, param);
                if (IsSleepPool)
                    t.OnSet(arg, param);
                return true;
            }
            protected override void OnCreate(Obj t, IEventArgs arg, params object[] param)
            {
                base.OnCreate(t, arg, param);
                if (IsSleepPool)
                    t.OnCreate(arg, param);
            }
            protected override Obj CreatNew(IEventArgs arg, params object[] param)
            {
                if (!IsSleepPool)
                    return default(Obj);
                Obj t = default(Obj);
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
                if (!have) t = (Obj)Activator.CreateInstance(type);
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
