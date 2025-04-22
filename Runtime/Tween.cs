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
using System.Linq;
using UnityEngine;

namespace IFramework
{


    partial class Tween
    {
        class TweenContextBox
        {
            private List<ITweenContext> contexts = new List<ITweenContext>();
            public void AddTween(ITweenContext context)
            {
                if (contexts.Contains(context)) return;
                contexts.Add(context);
                context.OnComplete(RemoveTween);
                context.OnCancel(RemoveTween);
            }

            private void RemoveTween(ITweenContext context)
            {
                if (contexts.Contains(context))
                {
                    contexts.Remove(context);
                }
            }
            public ITweenContext FindTween(ITweenContext context)
            {
                if (!contexts.Contains(context)) return null;
                return context;
            }

            public IReadOnlyList<ITweenContext> FindTweens()
            {
                return contexts;
            }

            static SimpleObjectPool<TweenContextBox> boxes = new SimpleObjectPool<TweenContextBox>();
            public static TweenContextBox New()
            {
                var box = boxes.Get();
                return box;
            }
            public static void Recycle(TweenContextBox box)
            {
                box.contexts.Clear();
                boxes.Set(box);
            }
        }


        private static Dictionary<object, TweenContextBox> boxes = new Dictionary<object, TweenContextBox>();
        private static TweenContextBox FindBox(object obj)
        {
            TweenContextBox box;
            boxes.TryGetValue(obj, out box);
            return box;
        }
        private static TweenContextBox GetBox(object obj)
        {
            TweenContextBox box;
            if (!boxes.TryGetValue(obj, out box))
            {
                box = TweenContextBox.New();
                boxes.Add(obj, box);
            }
            return box;
        }
        public static void RemoveTweenBox(this object obj)
        {
            TweenContextBox box;
            if (boxes.Remove(obj, out box))
            {
                TweenContextBox.Recycle(box);
            }
        }

        public static ITweenContext FindTween(this object obj, ITweenContext context)
        {
            var box = FindBox(obj);
            if (box == null) return null;
            return box.FindTween(context);
        }
        public static IReadOnlyList<ITweenContext> FindTweens(this object obj)
        {
            var box = FindBox(obj);
            if (box == null) return null;
            return box.FindTweens();
        }
        public static void CancelTweenContexts(this object obj)
        {
            var list = obj.FindTweens();
            if (list == null) return;
            for (int i = 0; i < list.Count; i++)
            {
                var context = list[i];
                context.Cancel();
            }
        }
        public static T AddTo<T>(this T context, object view) where T : ITweenContext
        {
            GetBox(view).AddTween(context);
            return context;
        }


        struct ITweenContextAwaitor<T> : IAwaiter<T> where T : ITweenContext
        {
            private T op;
            private Queue<Action> actions;

            public T GetResult() => op;
            public ITweenContextAwaitor(T op)
            {
                this.op = op;
                actions = new Queue<Action>();
                op.OnComplete(OnCompleted);
            }

            private void OnCompleted(ITweenContext context)
            {
                while (actions.Count > 0)
                {
                    actions.Dequeue()?.Invoke();
                }
            }

            public bool IsCompleted => op.isDone;

            public void OnCompleted(Action continuation)
            {
                actions?.Enqueue(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                OnCompleted(continuation);
            }


        }
        public static IAwaiter<T> GetAwaiter<T>(this T context) where T : ITweenContext => new ITweenContextAwaitor<T>(context);

    }

    public static partial class Tween
    {





#if UNITY_EDITOR
        private static TweenScheduler editorScheduler;
        private static void OnModeChange(UnityEditor.PlayModeStateChange mode)
        {
            if (mode == UnityEditor.PlayModeStateChange.EnteredPlayMode)
                editorScheduler?.CancelAllTween();

        }

        [UnityEditor.InitializeOnLoadMethod]
        static void CreateEditorScheduler()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= OnModeChange;
            UnityEditor.EditorApplication.playModeStateChanged += OnModeChange;
            editorScheduler = new TweenScheduler();
            UnityEditor.EditorApplication.update += editorScheduler.Update;
        }


#endif
        internal static TweenScheduler GetScheduler()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return editorScheduler;
#endif
            return TweenScheduler_Runtime.Instance.scheduler;
        }
        public static void CancelAllTween() => GetScheduler().CancelAllTween();

        static Dictionary<Type, object> value_calcs = new Dictionary<Type, object>()
        {
            { typeof(float), new ValueCalculator_Float() },
            { typeof(Vector2), new ValueCalculator_Vector2() },
            { typeof(Vector3), new ValueCalculator_Vector3() },
            { typeof(Vector4), new ValueCalculator_Vector4() },
            { typeof(Color), new ValueCalculator_Color() },
            { typeof(Rect), new ValueCalculator_Rect() },
        };
        public static List<Type> GetSupportTypes() => value_calcs.Keys.ToList();
        internal static ValueCalculator<T> GetValueCalculator<T>()
        {
            var type = typeof(T);
            object obj = null;
            if (!value_calcs.TryGetValue(type, out obj))
            {
                Log.FE($"Tween Not support Type: {type}");
            }
            return obj as ValueCalculator<T>;
        }



        //public static ITweenContext<T> As<T>(this ITweenContext t) => t as ITweenContext<T>;
        private static TweenContext<T, Target> AsInstance<T, Target>(this ITweenContext<T, Target> context) => context as TweenContext<T, Target>;
        public static ITweenContext<T, Target> DoGoto<T, Target>(Target target, T start, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, bool snap, bool autoRun = true)
        {
            var context = GetScheduler().AllocateContext<T, Target>(autoRun);
            context.AsInstance().Config(target, start, end, duration, getter, setter, snap);
            return context;
        }
        public static ITweenContext<T, Target> DoShake<T, Target>(Target target, T start, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, T strength,
            int frequency = 10, float dampingRatio = 1, bool snap = false, bool autoRun = true)
        {
            var context = GetScheduler().AllocateContext<T, Target>(autoRun);
            context.AsInstance().ShakeConfig(target, start, end, duration, getter, setter, snap, strength, frequency, dampingRatio);
            return context;
        }
        public static ITweenContext<T, Target> DoPunch<T, Target>(Target target, T start, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, T strength,
      int frequency = 10, float dampingRatio = 1, bool snap = false, bool autoRun = true)
        {
            var context = GetScheduler().AllocateContext<T, Target>(autoRun);
            context.AsInstance().PunchConfig(target, start, end, duration, getter, setter, snap, strength, frequency, dampingRatio);
            return context;
        }  
        public static ITweenContext<T, Target> DoJump<T, Target>(Target target, T start, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, T strength,
   int jumpCount = 5, float jumpDamping = 2f, bool snap = false, bool autoRun = true)
        {
            var context = GetScheduler().AllocateContext<T, Target>(autoRun);
            context.AsInstance().JumpConfig(target, start, end, duration, getter, setter, snap, strength, jumpCount, jumpDamping);
            return context;
        }
        public static ITweenContext<T, Target> DoArray<T, Target>(Target target, float duration, Func<Target, T> getter, Action<Target, T> setter, T[] points, bool snap = false, bool autoRun = true)
        {
            var context = GetScheduler().AllocateContext<T, Target>(autoRun);
            context.AsInstance().ArrayConfig(target, duration, getter, setter, snap, points);
            return context;
        }
        public static ITweenContext<T, Target> DoBezier<T, Target>(Target target, float duration, Func<Target, T> getter, Action<Target, T> setter, T[] points, bool snap = false, bool autoRun = true)
        {
            var context = GetScheduler().AllocateContext<T, Target>(autoRun);
            context.AsInstance().BezierConfig(target, duration, getter, setter, snap, points);
            return context;
        }





        public static ITweenGroup Sequence() => GetScheduler().AllocateSequence();
        public static ITweenGroup Parallel() => GetScheduler().AllocateParallel();


        public static T ReStart<T>(this T context) where T : ITweenContext
        {
            if (!(context as IPoolObject).valid)
            {
                Log.FE($"The {context} have be in pool,{nameof(context.autoCycle)}:{context.autoCycle}");
            }
            else
            {

                context.Stop();
                context.Run();
            }
            return context;
        }


        private static TweenContextBase AsContextBase(this ITweenContext t) => t as TweenContextBase;
        public static void Cancel<T>(this T context) where T : ITweenContext => context.Complete(false);
        public static T Pause<T>(this T t) where T : ITweenContext
        {
            t.AsContextBase().Pause();
            return t;
        }
        public static T UnPause<T>(this T t) where T : ITweenContext
        {
            t.AsContextBase().UnPause();
            return t;
        }
        public static T Complete<T>(this T t, bool callComplete) where T : ITweenContext
        {
            t.AsContextBase().Complete(callComplete);
            return t;
        }
        public static T Recycle<T>(this T t) where T : ITweenContext
        {
            t.AsContextBase().Recycle();
            return t;
        }
        public static T Stop<T>(this T t) where T : ITweenContext
        {
            t.AsContextBase().Stop();
            return t;
        }
        public static T SetTimeScale<T>(this T t, float value) where T : ITweenContext
        {
            t.AsContextBase().SetTimeScale(value);
            return t;
        }
        public static T SetAutoCycle<T>(this T t, bool value) where T : ITweenContext
        {
            t.AsContextBase().SetAutoCycle(value);
            return t;
        }
        public static T Run<T>(this T t) where T : ITweenContext
        {
            t.AsContextBase().Run();
            if (!(t is ITweenGroup))
                Tween.GetScheduler().AddToRun(t);
            return t;
        }
        public static T OnComplete<T>(this T t, Action<ITweenContext> action) where T : ITweenContext
        {
            t.AsContextBase().OnComplete(action);
            return t;
        }
        public static T OnBegin<T>(this T t, Action<ITweenContext> action) where T : ITweenContext
        {
            t.AsContextBase().OnBegin(action);
            return t;
        }
        public static T OnCancel<T>(this T t, Action<ITweenContext> action) where T : ITweenContext
        {
            t.AsContextBase().OnCancel(action);
            return t;
        }
        public static T OnTick<T>(this T t, Action<ITweenContext, float, float> action) where T : ITweenContext
        {
            t.AsContextBase().OnTick(action);
            return t;
        }





        public static ITweenContext<T, Target> SetLoop<T, Target>(this ITweenContext<T, Target> context, LoopType type, int loops)
        {
            context.AsInstance().SetLoop(type, loops);
            return context;
        }

        public static ITweenContext<T, Target> SetDelay<T, Target>(this ITweenContext<T, Target> t, float value)
        {
            t.AsInstance().SetDelay(value);
            return t;
        }
        public static ITweenContext<T, Target> SetSourceDelta<T, Target>(this ITweenContext<T, Target> t, float delta)
        {
            t.AsInstance().SetSourceDelta(delta);
            return t;
        }
        public static ITweenContext<T, Target> SetEvaluator<T, Target>(this ITweenContext<T, Target> t, IValueEvaluator evaluator)
        {
            t.AsInstance().SetEvaluator(evaluator);
            return t;
        }
        public static ITweenContext<T, Target> SetEase<T, Target>(this ITweenContext<T, Target> t, Ease ease) => t.SetEvaluator(new EaseEvaluator(ease));
        public static ITweenContext<T, Target> SetAnimationCurve<T, Target>(this ITweenContext<T, Target> t, AnimationCurve curve) => t.SetEvaluator(new AnimationCurveEvaluator(curve));
        public static ITweenContext<T, Target> SetSnap<T, Target>(this ITweenContext<T, Target> t, bool value)
        {
            t.AsInstance().SetSnap(value);
            return t;
        }
        public static ITweenContext<T, Target> SetDuration<T, Target>(this ITweenContext<T, Target> t, float value)
        {
            t.AsInstance().SetDuration(value);
            return t;
        }
        public static ITweenContext<T, Target> SetFrequency<T, Target>(this ITweenContext<T, Target> t, int value)
        {
            t.AsInstance().SetFrequency(value);
            return t;
        }
        public static ITweenContext<T, Target> SetDampingRatio<T, Target>(this ITweenContext<T, Target> t, float value)
        {
            t.AsInstance().SetDampingRatio(value);
            return t;
        }
        public static ITweenContext<T, Target> SetJumpCount<T, Target>(this ITweenContext<T, Target> t, int value)
        {
            t.AsInstance().SetJumpCount(value);
            return t;
        }
        public static ITweenContext<T, Target> SetJumpDamping<T, Target>(this ITweenContext<T, Target> t, float value)
        {
            t.AsInstance().SetJumpDamping(value);
            return t;
        }
        public static ITweenContext<T, Target> SetStrength<T, Target>(this ITweenContext<T, Target> t, T value)
        {
            t.AsInstance().SetStrength(value);
            return t;
        }
        //public static ITweenContext<T> SetEnd<T>(this ITweenContext<T> t, T value)
        //{
        //    t.AsInstance().SetEnd(value);
        //    return t;
        //}
        //public static ITweenContext<T> SetStart<T>(this ITweenContext<T> t, T value)
        //{
        //    t.AsInstance().SetStart(value);
        //    return t;
        //}


    }





}