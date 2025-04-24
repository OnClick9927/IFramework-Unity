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
        private List<TimerContext> timers;

        private Dictionary<Type, ISimpleObjectPool> contextPools;

        public TimerScheduler()
        {
            contextPools = new Dictionary<Type, ISimpleObjectPool>();
            timers = new List<TimerContext>();
        }

        public ITimerGroup NewTimerSequence() => _NewTimerContext<TimerSequence>();
        public ITimerGroup NewTimerParallel() => _NewTimerContext<TimerParallel>();
        public T _NewTimerContext<T>() where T : TimerContextBase, new()
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
        public T NewTimerContext<T>() where T : TimerContext, new() => _NewTimerContext<T>();
        public void Cycle(ITimerContext context)
        {
            var type = context.GetType();
            ISimpleObjectPool _pool = null;
            if (contextPools.TryGetValue(type, out _pool))
                _pool.SetObject(context);
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
