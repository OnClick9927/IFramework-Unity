/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework
{
    class TweenScheduler
    {
        private Dictionary<Type, ISimpleObjectPool> contextPools;

        public TweenScheduler()
        {
            contextPools = new Dictionary<Type, ISimpleObjectPool>();
        }

        public void Update()
        {
            float deltaTime = Launcher.GetDeltaTime();
            for (int i = 0; i < contexts_run.Count; i++)
            {
                var context = contexts_run[i];
                (context as TweenContext).Update(deltaTime);
            }
            for (int i = 0; i < contexts_wait_to_run.Count; i++)
            {
                var context = contexts_wait_to_run[i];
                context.Run();
            }
            contexts_wait_to_run.Clear();
        }

        private List<ITweenContext> contexts_run = new List<ITweenContext>();
        private List<ITweenContext> contexts_wait_to_run = new List<ITweenContext>();


        public ITweenContext<T, Target> AllocateContext<T, Target>(bool auto_run)
        {
            Type type = typeof(TweenContext<T, Target>);
            ISimpleObjectPool pool = null;
            if (!contextPools.TryGetValue(type, out pool))
            {
                pool = new SimpleObjectPool<TweenContext<T, Target>>();
                contextPools.Add(type, pool);
            }
            var simple = pool as SimpleObjectPool<TweenContext<T, Target>>;
            var context = simple.Get();
            if (auto_run)
                contexts_wait_to_run.Add(context);
            return context;
        }
        public ITweenGroup AllocateSequence()
        {
            Type type = typeof(TweenSequence);
            ISimpleObjectPool pool = null;
            if (!contextPools.TryGetValue(type, out pool))
            {
                pool = new SimpleObjectPool<TweenSequence>();
                contextPools.Add(type, pool);
            }
            var simple = pool as SimpleObjectPool<TweenSequence>;
            var context = simple.Get();
            //contexts_run.Add(context);
            return context;
        }
        public ITweenGroup AllocateParallel()
        {
            Type type = typeof(TweenParallel);
            ISimpleObjectPool pool = null;
            if (!contextPools.TryGetValue(type, out pool))
            {
                pool = new SimpleObjectPool<TweenParallel>();
                contextPools.Add(type, pool);
            }
            var simple = pool as SimpleObjectPool<TweenParallel>;
            var context = simple.Get();
            //contexts_run.Add(context);
            return context;
        }

        public void CycleContext(ITweenContext context)
        {
            var type = context.GetType();


            ISimpleObjectPool pool = null;
            if (!contextPools.TryGetValue(type, out pool)) return;
            contexts_run.Remove(context);
            contexts_wait_to_run.Remove(context);

            pool.SetObject(context);
        }

        public void CancelAllTween()
        {
            for (int i = contexts_run.Count - 1; i >= 0; i--)
            {
                var context = contexts_run[i];
                context.Cancel();
            }
        }

        internal void AddToRun(ITweenContext context)
        {
            if (context == null || contexts_run.Contains(context)) return;
            contexts_run.Add(context);
        }
    }





}