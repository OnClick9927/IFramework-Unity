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

    public static partial class TimeEx
    {
        public static void KillTimers(this object obj)
        {
            var scheduler = GetScheduler() as TimerScheduler;
            scheduler.KillTimers(obj);
        }
        public static void KillTimers()
        {
            var scheduler = GetScheduler() as TimerScheduler;
            scheduler.KillTimers();
        }

        public static T AddTo<T>(this T context, object view) where T : ITimerContext
        {
            context.AsContextBase().SetOwner(view);
            return context;
        }
        struct ITimerContextAwaitor<T> : IAwaiter<T> where T : ITimerContext
        {
            private T op;
            private Queue<Action> actions;
            public ITimerContextAwaitor(T op)
            {
                this.op = op;
                actions = new Queue<Action>();
                op.OnComplete(OnCompleted);
            }

            private void OnCompleted(ITimerContext context)
            {
                while (actions.Count > 0)
                {
                    actions.Dequeue()?.Invoke();
                }
            }

            public bool IsCompleted => op.isDone;

            public T GetResult() => op;
            public void OnCompleted(Action continuation)
            {
                actions?.Enqueue(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                OnCompleted(continuation);
            }
        }
        public static IAwaiter<T> GetAwaiter<T>(this T context) where T : ITimerContext => new ITimerContextAwaitor<T>(context);

#if UNITY_EDITOR
        private static TimerScheduler editorScheduler;
        private static void OnModeChange(UnityEditor.PlayModeStateChange mode)
        {
            if (mode == UnityEditor.PlayModeStateChange.ExitingEditMode)
                editorScheduler?.KillTimers();

        }

        [UnityEditor.InitializeOnLoadMethod]
        static void CreateEditorScheduler()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= OnModeChange;
            UnityEditor.EditorApplication.playModeStateChanged += OnModeChange;
            editorScheduler = new TimerScheduler();
            UnityEditor.EditorApplication.update += editorScheduler.Update;
        }
#endif
        internal static TimerScheduler GetScheduler()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return editorScheduler;
#endif
            return TimerScheduler_Runtime.Instance.scheduler;
        }



        const float min2Delta = 0.00001f;
        public static T OnComplete<T>(this T context, Action<ITimerContext> action) where T : ITimerContext
        {
            context.AsContextBase().OnComplete(action);
            return context;
        }
        public static T OnCancel<T>(this T context, Action<ITimerContext> action) where T : ITimerContext
        {
            context.AsContextBase().OnCancel(action);
            return context;
        }
        public static T OnBegin<T>(this T context, Action<ITimerContext> action) where T : ITimerContext
        {
            context.AsContextBase().OnBegin(action);
            return context;
        }
        public static T OnTick<T>(this T context, TimerAction action) where T : ITimerContext
        {
            context.AsContextBase().OnTick(action);
            return context;
        }

        public static T SetId<T>(this T t, string id) where T : ITimerContext
        {
            t.AsContextBase().SetId(id);
            return t;
        }
        public static T SetTimeScale<T>(this T t, float timeScale) where T : ITimerContext
        {
            t.AsContextBase().SetTimeScale(timeScale);
            return t;
        }
        public static T Pause<T>(this T t) where T : ITimerContext
        {
            t.AsContextBase().Pause();

            return t;
        }
        public static T UnPause<T>(this T t) where T : ITimerContext
        {
            t.AsContextBase().UnPause();
            return t;
        }
        public static T Cancel<T>(this T t) where T : ITimerContext
        {
            t.AsContextBase().Cancel();
            return t;
        }
        public static T Stop<T>(this T t) where T : ITimerContext
        {
            t.AsContextBase().Stop();
            return t;
        }
        private static TimerContextBase AsContextBase(this ITimerContext t) => t as TimerContextBase;

        internal static event Action<ITimerContext> onContextAllocate, onContextRecycle;

        internal static void Cycle(ITimerContext context)
        {
            GetScheduler().Cycle(context);
            onContextRecycle?.Invoke(context);
        }

        public static T NewContext<T>() where T : TimerContext, new()
        {
            var context = GetScheduler().NewContext<T>();
            onContextAllocate?.Invoke(context);
            return context;
        }

        public static ITimerGroup Sequence()
        {
            var context = GetScheduler().Sequence();
            onContextAllocate?.Invoke(context);
            return context;
        }

        public static ITimerGroup Parallel()
        {
            var context = GetScheduler().Parallel();
            onContextAllocate?.Invoke(context);

            return context;
        }

        public static ITimerContext RunTimerContext(TimerContext context) => GetScheduler().RunTimerContext(context);
        public static ITimerContext Until(TimerFunc condition, float interval = min2Delta)
        {
            var cls = NewContext<ConditionTimerContext>();
            bool succ = cls.Condition(condition, interval, true, false);
            return succ ? RunTimerContext(cls) : null;
        }
        public static ITimerContext While(TimerFunc condition, float interval = min2Delta)
        {
            var cls = NewContext<ConditionTimerContext>();
            bool succ = cls.Condition(condition, interval, false, false);
            return succ ? RunTimerContext(cls) : null;
        }
        public static ITimerContext DoWhile(TimerFunc condition, float interval = min2Delta)
        {
            var cls = NewContext<ConditionTimerContext>();

            bool succ = cls.Condition(condition, interval, false, true);
            return succ ? RunTimerContext(cls) : null;
        }
        public static ITimerContext DoUntil(TimerFunc condition, float interval = min2Delta)
        {
            var cls = NewContext<ConditionTimerContext>();
            bool succ = cls.Condition(condition, interval, true, true);
            return succ ? RunTimerContext(cls) : null;
        }
        public static ITimerContext Delay(float delay, TimerAction action = null)
        {
            var cls = NewContext<TickTimerContext>();
            bool succ = cls.Delay(delay, action);
            return succ ? RunTimerContext(cls) : null;
        }
        public static ITimerContext Frame() => Delay(min2Delta);
        public static ITimerContext Tick(float interval, int times, TimerAction action)
        {
            var cls = NewContext<TickTimerContext>();
            bool succ = cls.Tick(interval, times, action);
            return succ ? RunTimerContext(cls) : null;

        }
        public static ITimerContext DelayAndTick(float delay, TimerAction delayCall, float interval, int times, TimerAction action)
        {
            var cls = NewContext<TickTimerContext>();
            bool succ = cls.DelayAndTick(delay, delayCall, interval, times, action);
            return succ ? RunTimerContext(cls) : null;

        }
    }

}
