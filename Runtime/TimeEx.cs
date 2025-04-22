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
    partial class TimeEx
    {
        class TimerContextBox
        {
            private List<ITimerContext> contexts = new List<ITimerContext>();
            public void AddTween(ITimerContext context)
            {
                if (contexts.Contains(context)) return;
                contexts.Add(context);
                context.OnComplete(Remove);
                context.OnCancel(Remove);
            }

            private void Remove(ITimerContext context)
            {
                if (contexts.Contains(context))
                {
                    contexts.Remove(context);
                }
            }
            public ITimerContext Find(ITimerContext context)
            {
                if (!contexts.Contains(context)) return null;
                return context;
            }

            public IReadOnlyList<ITimerContext> Find()
            {
                return contexts;
            }

            static SimpleObjectPool<TimerContextBox> boxes = new SimpleObjectPool<TimerContextBox>();
            public static TimerContextBox New()
            {
                var box = boxes.Get();
                return box;
            }
            public static void Recycle(TimerContextBox box)
            {
                box.contexts.Clear();
                boxes.Set(box);
            }
        }


        private static Dictionary<object, TimerContextBox> boxes = new Dictionary<object, TimerContextBox>();
        private static TimerContextBox FindBox(object obj)
        {
            TimerContextBox box;
            boxes.TryGetValue(obj, out box);
            return box;
        }
        private static TimerContextBox GetBox(object obj)
        {
            TimerContextBox box;
            if (!boxes.TryGetValue(obj, out box))
            {
                box = TimerContextBox.New();
                boxes.Add(obj, box);
            }
            return box;
        }
        public static void RemoveTimerBox(this object obj)
        {
            TimerContextBox box;
            if (boxes.Remove(obj, out box))
            {
                TimerContextBox.Recycle(box);
            }
        }

        public static ITimerContext FindTimer(this object obj, ITimerContext context)
        {
            var box = FindBox(obj);
            if (box == null) return null;
            return box.Find(context);
        }
        public static IReadOnlyList<ITimerContext> FindTimers(this object obj)
        {
            var box = FindBox(obj);
            if (box == null) return null;
            return box.Find();
        }
        public static void CancelTimerContexts(this object obj)
        {
            var list = obj.FindTimers();
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                var context = list[i];
                context.Cancel();
            }
        }
        public static T AddTo<T>(this T context, object view) where T : ITimerContext
        {
            GetBox(view).AddTween(context);
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

    }
    public static partial class TimeEx
    {

#if UNITY_EDITOR
        private static TimerScheduler editorScheduler;
        private static void OnModeChange(UnityEditor.PlayModeStateChange mode)
        {
            if (mode == UnityEditor.PlayModeStateChange.EnteredPlayMode)
                editorScheduler?.ClearTimers();
           
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
        internal static ITimerScheduler GetScheduler()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return editorScheduler;
#endif
            return TweenScheduler_Runtime.Instance.scheduler;
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
        public static T Complete<T>(this T t) where T : ITimerContext
        {
            t.AsContextBase().Complete();
            return t;
        }
        public static T Cancel<T>(this T t) where T : ITimerContext
        {
            t.AsContextBase().Cancel();
            return t;
        }
        private static TimerContextBase AsContextBase(this ITimerContext t) => t as TimerContextBase;

        public static ITimerScheduler Scheduler => GetScheduler();


        public static ITimerContext Until(this ITimerScheduler scheduler, TimerFunc condition, float interval = min2Delta)
        {
            var cls = scheduler.NewTimerContext<ConditionTimerContext>();
            bool succ = cls.Condition(condition, interval, true, false);
            return succ ? scheduler.RunTimerContext(cls) : null;
        }
        public static ITimerContext While(this ITimerScheduler scheduler, TimerFunc condition, float interval = min2Delta)
        {
            var cls = scheduler.NewTimerContext<ConditionTimerContext>();
            bool succ = cls.Condition(condition, interval, false, false);
            return succ ? scheduler.RunTimerContext(cls) : null;
        }
        public static ITimerContext DoWhile(this ITimerScheduler scheduler, TimerFunc condition, float interval = min2Delta)
        {
            var cls = scheduler.NewTimerContext<ConditionTimerContext>();

            bool succ = cls.Condition(condition, interval, false, true);
            return succ ? scheduler.RunTimerContext(cls) : null;
        }
        public static ITimerContext DoUntil(this ITimerScheduler scheduler, TimerFunc condition, float interval = min2Delta)
        {
            var cls = scheduler.NewTimerContext<ConditionTimerContext>();
            bool succ = cls.Condition(condition, interval, true, true);
            return succ ? scheduler.RunTimerContext(cls) : null;
        }
        public static ITimerContext Delay(this ITimerScheduler scheduler, float delay, TimerAction action = null)
        {
            var cls = scheduler.NewTimerContext<TickTimerContext>();
            bool succ = cls.Delay(delay, action);
            return succ ? scheduler.RunTimerContext(cls) : null;
        }
        public static ITimerContext Frame(this ITimerScheduler scheduler) => scheduler.Delay(min2Delta);
        public static ITimerContext Tick(this ITimerScheduler scheduler, float interval, int times, TimerAction action)
        {
            var cls = scheduler.NewTimerContext<TickTimerContext>();
            bool succ = cls.Tick(interval, times, action);
            return succ ? scheduler.RunTimerContext(cls) : null;

        }
        public static ITimerContext DelayAndTick(this ITimerScheduler scheduler, float delay, TimerAction delayCall, float interval, int times, TimerAction action)
        {
            var cls = scheduler.NewTimerContext<TickTimerContext>();
            bool succ = cls.DelayAndTick(delay, delayCall, interval, times, action);
            return succ ? scheduler.RunTimerContext(cls) : null;

        }



    }

}
