/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using System;

namespace IFramework
{
    class TimerScheduler : ITimerScheduler
    {
        private SimpleObjectPool<TimerSequence> seuqencesPool;
        private SimpleObjectPool<TimerParallel> parallelPool;

        //private SimpleObjectPool<Timer> pool;
        private List<TimerContext> timers;

        private Dictionary<Type, ISimpleObjectPool> contextPools;

        public TimerScheduler()
        {
            parallelPool = new SimpleObjectPool<TimerParallel>();
            seuqencesPool = new SimpleObjectPool<TimerSequence>();
            contextPools = new Dictionary<Type, ISimpleObjectPool>();
            //pool = new SimpleObjectPool<Timer>();
            timers = new List<TimerContext>();
        }

        public ITimerGroup NewTimerSequence()
        {
            var seq = seuqencesPool.Get();
            seq.scheduler = this;
            return seq;
        }
        public ITimerGroup NewTimerParallel()
        {
            var seq = parallelPool.Get();
            seq.scheduler = this;
            return seq;
        }
        public void Cycle(TimerSequence seq) => seuqencesPool.Set(seq);
        private void Cycle(TimerContext context)
        {
            var type = context.GetType();
            ISimpleObjectPool _pool = null;
            if (contextPools.TryGetValue(type, out _pool))
                _pool.SetObject(context);
            //pool.Set(timer);
        }
        public void Cycle(TimerParallel parallel) => parallelPool.Set(parallel);
        public T NewTimerContext<T>() where T : TimerContext, new()
        {
            Type type = typeof(T);
            ISimpleObjectPool pool = null;
            if (!contextPools.TryGetValue(type, out pool))
            {
                pool = new SimpleObjectPool<T>();
                contextPools.Add(type, pool);
            }
            var simple = pool as SimpleObjectPool<T>;
            var cls = simple.Get();
            cls.scheduler = this;
            return cls;
        }

        public ITimerContext RunTimerContext(TimerContext context)
        {
            context.UnPause();
            timers.Add(context);
            return context;
        }



        public void ClearTimers()
        {
            timers.Clear();
        }




        internal void Update()
        {
            float deltaTime = Launcher.GetDeltaTime();
            for (int i = timers.Count - 1; i >= 0; i--)
            {


                var timer = timers[i];
                if (timer.canceled)
                {
                    timers.RemoveAt(i);
                    Cycle(timer);
                    continue;
                }

                timer.Update(deltaTime);
                if (timer.isDone)
                {
                    timers.RemoveAt(i);
                    Cycle(timer);
                }
            }


        }


    }

}
