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
    public enum TweenType
    {
        Normal, Shake, Punch, Jump, Bezier
    }
    public enum LoopType
    {
        Restart,
        PingPong,
        Add
    }
    public enum Ease
    {
        Linear,
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc
    }
    public interface IValueEvaluator
    {
        float Evaluate(float percent, float time, float duration);
    }

    public interface ITweenContext
    {
        bool isDone { get; }
        bool autoCycle { get; }
        bool paused { get; }
    }
    public interface ITweenGroup : ITweenContext
    {
        ITweenGroup NewContext(Func<ITweenContext> func);
    }

    public interface ITweenContext<T, Target> : ITweenContext { }

    public interface ITweenContextBox
    {
        void AddTween(ITweenContext context);
        void CancelTween(ITweenContext context);
        void CancelTweenContexts();
        void CompleteTween(ITweenContext context);
        void CompleteTweenContexts();
    }
    class TweenContextBox : ITweenContextBox
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

        public void CancelTween(ITweenContext context)
        {
            if (!contexts.Contains(context)) return;
            context.Cancel();
            RemoveTween(context);
        }

        public void CancelTweenContexts()
        {
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.Cancel();
            }
            contexts.Clear();
        }

        public void CompleteTween(ITweenContext context)
        {
            if (!contexts.Contains(context)) return;
            context.Complete(true);
            RemoveTween(context);
        }
        public void CompleteTweenContexts()
        {
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.Complete(true);
            }
            contexts.Clear();
        }
    }


    abstract class TweenContextBase : ITweenContext, IPoolObject
    {

        public void Recycle()
        {
            if (!valid) return;
            Tween.GetScheduler().CycleContext(this);
        }
        protected void TryRecycle()
        {
            if (!autoCycle) return;
            Recycle();
        }
        public bool autoCycle { get; private set; }
        public bool isDone { get; private set; }
        public bool canceled { get; private set; }
        public bool valid { get; set; }
        public bool paused { get; private set; }
        protected float timeScale { get; private set; }



        private Action<ITweenContext> onBegin;

        private Action<ITweenContext> onComplete;
        private Action<ITweenContext> onCancel;
        private Action<ITweenContext, float, float> onTick;



        public void OnBegin(Action<ITweenContext> action) => onBegin += action;
        public void OnComplete(Action<ITweenContext> action) => onComplete += action;
        public void OnCancel(Action<ITweenContext> action) => onCancel += action;
        public void OnTick(Action<ITweenContext, float, float> action) => onTick += action;
        protected virtual void Reset()
        {
            paused = false;
            isDone = false;
            canceled = false;
            onCancel = null;
            onBegin = null;
            onComplete = null;
            onTick = null;
            autoCycle = true;
            timeScale = 1f;
        }
        protected void InvokeCancel()
        {
            if (canceled) return;
            canceled = true;
            onCancel?.Invoke(this);
        }
        protected void SetCancel()
        {
            if (canceled) return;
            canceled = true;
        }
        protected void InvokeComplete()
        {
            if (isDone) return;
            isDone = true;
            onComplete?.Invoke(this);
        }
        protected void InvokeTick(float time, float tick)
        {
            onTick?.Invoke(this, time, tick);
        }


        void IPoolObject.OnGet() => Reset();

        void IPoolObject.OnSet() => Reset();

        public virtual ITweenContext SetTimeScale(float timeScale)
        {
            if (!valid) return this;
            this.timeScale = timeScale;
            return this;
        }

        public abstract void Complete(bool callComplete);

        public virtual void Pause()
        {
            if (!valid || paused) return;
            paused = true;
        }
        public virtual void UnPause()
        {
            if (!valid || !paused) return;
            paused = false;
        }



        public virtual void Run()
        {
            paused = false;
            canceled = false;
            isDone = false;
            onBegin?.Invoke(this);
        }

        public ITweenContext SetAutoCycle(bool cycle)
        {
            this.autoCycle = cycle;
            return this;
        }



        public abstract void Stop();

    }


    class TweenSequence : TweenContextBase, ITweenGroup
    {
        private List<Func<ITweenContext>> list = new List<Func<ITweenContext>>();
        private Queue<Func<ITweenContext>> _queue = new Queue<Func<ITweenContext>>();
        public ITweenGroup NewContext(Func<ITweenContext> func)
        {
            if (func == null) return this;
            list.Add(func);
            return this;
        }
        private ITweenContext inner;
        public override void Stop()
        {
            inner?.Stop();
            inner = null;
            TryRecycle();
        }
        private void Cancel()
        {
            if (canceled) return;
            InvokeCancel();
            inner?.Cancel();
            TryRecycle();

        }
        private void Complete()
        {
            if (isDone) return;
            InvokeComplete();
            TryRecycle();
        }
        protected override void Reset()
        {
            base.Reset();
            inner = null;
            list.Clear();
            _queue.Clear();
        }

        public override void Complete(bool callComplete)
        {
            if (callComplete)
                Complete();
            else
                Cancel();
        }





        private void RunNext(ITweenContext context)
        {
            if (canceled || isDone) return;
            if (_queue.Count > 0)
            {
                inner = _queue.Dequeue().Invoke();
                if (inner != null)
                {
                    inner.OnTick(_OnTick);
                    inner.OnCancel(RunNext);
                    inner.OnComplete(RunNext);
                    inner?.SetTimeScale(this.timeScale);
                }
                else
                {
                    RunNext(context);
                }
            }
            else
            {
                Complete();
            }
        }

        private void _OnTick(ITweenContext context, float time, float delta)
        {
            InvokeTick(time, delta);
        }

        public override void Run()
        {
            base.Run();
            _queue.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                var func = list[i];
                _queue.Enqueue(func);
            }
            RunNext(null);
        }

        public override ITweenContext SetTimeScale(float timeScale)
        {
            if (!valid) return this;
            base.SetTimeScale(timeScale);
            inner?.SetTimeScale(timeScale);
            return this;
        }

        public override void Pause()
        {
            if (!valid || paused) return;
            base.Pause();
            inner?.Pause();
        }

        public override void UnPause()
        {
            if (!valid || !paused) return;
            base.UnPause();
            inner?.UnPause();
        }



    }

    class TweenParallel : TweenContextBase, ITweenGroup
    {
        private List<Func<ITweenContext>> list = new List<Func<ITweenContext>>();
        private List<ITweenContext> contexts = new List<ITweenContext>();

        public ITweenGroup NewContext(Func<ITweenContext> func)
        {
            if (func == null) return this;
            list.Add(func);
            return this;
        }

        public override void Stop()
        {
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.Stop();
            }
            TryRecycle();
        }


        private void Cancel()
        {
            if (canceled) return;
            InvokeCancel();
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.Cancel();
            }
            TryRecycle();

        }
        private void Complete()
        {
            if (isDone) return;
            InvokeComplete();
            TryRecycle();
        }
        protected override void Reset()
        {
            base.Reset();
            this._time = this._delta = -1;
            list.Clear();
            contexts.Clear();
        }

        public override void Complete(bool callComplete)
        {
            if (callComplete)
                Complete();
            else
                Cancel();
        }
        private void OnContextEnd(ITweenContext context)
        {

            if (canceled || isDone) return;
            if (contexts.Count > 0)
                contexts.Remove(context);
            if (contexts.Count == 0)
                Complete();
        }
        private float _time, _delta;
        private void _OnTick(ITweenContext context, float time, float delta)
        {
            if (_time != time || _delta != delta)
            {
                this._time = time;
                this._delta = delta;
                InvokeTick(time, delta);
            }
        }

        public override void Run()
        {
            base.Run();
            contexts.Clear();
            if (list.Count > 0)
                for (int i = 0; i < list.Count; i++)
                {
                    var func = list[i];
                    var context = func.Invoke();
                    context.OnCancel(OnContextEnd);
                    context.OnComplete(OnContextEnd);
                    context.OnTick(_OnTick);
                    context.SetTimeScale(timeScale);
                    contexts.Add(context);
                }
            else
                Complete();
        }

        public override ITweenContext SetTimeScale(float timeScale)
        {
            if (!valid) return this;
            base.SetTimeScale(timeScale);
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.SetTimeScale(timeScale);
            }
            return this;
        }

        public override void Pause()
        {
            if (!valid || paused) return;
            base.Pause();
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.Pause();
            }
        }

        public override void UnPause()
        {
            if (!valid || !paused) return;
            base.UnPause();
            for (int i = 0; i < contexts.Count; i++)
            {
                var context = contexts[i];
                context.UnPause();
            }
        }
    }



















    abstract class TweenContext : TweenContextBase, ITweenContext
    {
        protected float time { get; private set; }
        protected void SetTime(float value) => time = value;
        internal void Update(float deltaTime)
        {
            if (paused || isDone) return;
            if (!MoveNext(deltaTime))
            {
                InvokeComplete();
                TryRecycle();
            }
            else
            {
                InvokeTick(this.time, deltaTime);
            }
        }
        protected abstract bool MoveNext(float delta);
        public override void Run()
        {
            this.SetTime(0);
            base.Run();
            OnRun();
        }
        protected abstract void OnRun();
        public override void Complete(bool callComplete)
        {
            if (isDone) return;
            if (callComplete)
                InvokeComplete();
            else
                InvokeCancel();
            TryRecycle();
        }

        public override void Stop()
        {
            SetCancel();
            TryRecycle();
        }

    }

    class TweenContext<T, Target> : TweenContext, ITweenContext<T, Target>
    {
        private Target target;
        private static ValueCalculator<T> _calc;

        private T start;
        private T end;
        private bool snap;
        private float delay;
        private float duration;
        private float sourceDelta;
        private IValueEvaluator evaluator;
        private int loops;
        private LoopType loopType;
        private Func<Target, T> getter;
        private Action<Target, T> setter;
        private T _start;
        private T _end;

        private bool _wait_delay_flag;
        private int _loop;


        private TweenType _mode;
        private T[] points;
        private int jumpCount;
        private float jumpDamping;

        private static ValueCalculator<T> calc
        {
            get
            {
                if (_calc == null)
                    _calc = Tween.GetValueCalculator<T>();
                return _calc;
            }
        }
        protected override void Reset()
        {
            base.Reset();
            OnRun();
            this.SetEvaluator(EaseEvaluator.Default);
            this.SetSourceDelta(0);
            this.SetDelay(0);
            this.SetLoop(LoopType.Restart, 1);
            _set2Start_called = false;
        }
        protected override void OnRun()
        {
            _wait_delay_flag = true;
            _loop = 0;
            _start = start;
            _end = end;
            _set2Start_called = false;
        }

        private bool _set2Start_called = false;
        private int frequency;
        private float dampingRatio;
        private T strength;


        protected override bool MoveNext(float delta)
        {
            var targetTime = this.time + delta * timeScale;
            if (!_set2Start_called && getter != null)
            {

                CalculateView(0);
                _set2Start_called = true;
            }
            if (_wait_delay_flag)
            {
                SetTime(targetTime);
                if (this.time >= delay)
                {
                    targetTime = 0;
                    SetTime(targetTime);
                    _wait_delay_flag = false;
                }
            }
            if (!_wait_delay_flag)
                if (targetTime >= duration)
                {
                    SetTime(duration);
                    CalculateView(duration);
                    OnLoopEnd();
                    _loop++;
                    SetTime(0);
                }
                else
                {
                    SetTime(targetTime);
                    CalculateView(targetTime);
                }
            bool result = loops == -1 || _loop < loops;
            return result;
        }
        private void CalculateView(float _time)
        {
            //var _time = this.time;
            var _dur = this.duration;
            var _percent = Mathf.Clamp01(_time / _dur);
            var _convertPercent = evaluator.Evaluate(_percent, _time, _dur);
            var _deltaPercent = (1 - sourceDelta) + sourceDelta * _percent;

            var src = getter.Invoke(target);
            T _cur = calc.Calculate(_mode, _start, _end, _convertPercent, src, _deltaPercent, snap, strength, frequency, dampingRatio, jumpCount, jumpDamping, points);
            if (!src.Equals(_cur))
                setter?.Invoke(target, _cur);
        }




        private void OnLoopEnd()
        {
            if (loopType == LoopType.PingPong)
            {
                var temp = _start;
                _start = _end;
                _end = temp;
            }
            else if (loopType == LoopType.Add)
            {
                var tmp = calc.CalculatorEnd(_start, _end);
                _start = _end;
                _end = tmp;
            }
        }


        public TweenContext<T, Target> Config(Target target, T start, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, bool snap)
        {
            this.target = target;
            _mode = TweenType.Normal;
            this._start = this.start = start;
            this._end = this.end = end;
            this.duration = duration;
            this.getter = getter;
            this.setter = setter;
            this.snap = snap;
            return this;
        }
        public TweenContext<T, Target> ShakeConfig(Target target, T start, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, bool snap, T strength,
            int frequency, float dampingRatio)
        {
            this.Config(target, start, end, duration, getter, setter, snap);
            _mode = TweenType.Shake;
            this.frequency = frequency;
            this.dampingRatio = dampingRatio;
            this.strength = strength;

            return this;
        }
        public TweenContext<T, Target> PunchConfig(Target target, T start, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, bool snap, T strength,
       int frequency, float dampingRatio)
        {
            this.Config(target, start, end, duration, getter, setter, snap);
            _mode = TweenType.Punch;
            this.frequency = frequency;
            this.dampingRatio = dampingRatio;
            this.strength = strength;

            return this;
        }
        public TweenContext<T, Target> JumpConfig(Target target, T start, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, bool snap, T strength,
  int jumpCount, float jumpDamping)
        {
            this.Config(target, start, end, duration, getter, setter, snap);
            _mode = TweenType.Jump;
            this.jumpCount = jumpCount;
            this.jumpDamping = jumpDamping;
            this.strength = strength;

            return this;
        }


        public TweenContext<T, Target> BezierConfig(Target target, float duration, Func<Target, T> getter, Action<Target, T> setter, bool snap, T[] points)
        {
            if (points==null || points.Length<=2)
            {
                Log.FE("At Least 3 point");
            }
            this.Config(target, default, default, duration, getter, setter, snap);
            _mode = TweenType.Bezier;
            this.points = points;

            return this;
        }

        public void SetSourceDelta(float delta) => sourceDelta = delta;
        public void SetEvaluator(IValueEvaluator evaluator) => this.evaluator = evaluator;
        public void SetSnap(bool value) => snap = value;



        public void SetDelay(float value)
        {
            if (value <= 0.00002f)
                value = 0.0002f;
            delay = value;
        }
        public void SetLoop(LoopType type, int loops)
        {
            this.loops = loops;
            loopType = type;
        }

        public void SetDuration(float duration) => this.duration = duration;
        public void SetFrequency(int duration) => this.frequency = duration;
        public void SetStrength(T duration) => this.strength = duration;
        public void SetDampingRatio(float duration) => this.dampingRatio = duration;
        public void SetJumpDamping(float jumpDamping) => this.jumpDamping = jumpDamping;
        public void SetJumpCount(int jumpCount) => this.jumpCount = jumpCount;

    }



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
    [MonoSingletonPath(nameof(IFramework.Tween))]
    class TweenScheduler_Runtime : MonoSingleton<TweenScheduler_Runtime>
    {
        public TweenScheduler scheduler;

        protected override void OnSingletonInit()
        {
            base.OnSingletonInit();
            scheduler = new TweenScheduler();
        }
        private void Update()
        {
            scheduler.Update();
        }
        protected override void OnDestroy()
        {
            scheduler.CancelAllTween();
            base.OnDestroy();
        }


    }




    public static class Tween
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
            { typeof(int), new ValueCalculator_Int() },
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
        public static IAwaiter<T> GetAwaiter<T>(this T context) where T : ITweenContext => new ITweenContextAwaitor<T>(context);
        public static ITweenContext<T, Target> DoGoto<T, Target>(Target target, T start, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, bool snap, bool autoRun = true)
        {
            var context = GetScheduler().AllocateContext<T, Target>(autoRun);
            context.AsInstance().Config(target, start, end, duration, getter, setter, snap);
            return context;
        }
        public static ITweenContext<T, Target> DoGoto<T, Target>(Target target, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, bool snap, bool autoRun = true)
            => DoGoto(target, getter.Invoke(target), end, duration, getter, setter, snap, autoRun);

        public static ITweenContext<T, Target> DoShake<T, Target>(Target target, T start, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, T strength,
            int frequency = 10, float dampingRatio = 1, bool snap = false, bool autoRun = true)
        {
            var context = GetScheduler().AllocateContext<T, Target>(autoRun);
            context.AsInstance().ShakeConfig(target, start, end, duration, getter, setter, snap, strength, frequency, dampingRatio);
            return context;
        }

        public static ITweenContext<T, Target> DoShake<T, Target>(Target target, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, T strength,
          int frequency = 10, float dampingRatio = 1, bool snap = false, bool autoRun = true)
        {
            return DoShake<T, Target>(target, getter.Invoke(target), end, duration, getter, setter, strength, frequency, dampingRatio, snap, autoRun);
        }
        public static ITweenContext<T, Target> DoPunch<T, Target>(Target target, T start, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, T strength,
      int frequency = 10, float dampingRatio = 1, bool snap = false, bool autoRun = true)
        {
            var context = GetScheduler().AllocateContext<T, Target>(autoRun);
            context.AsInstance().PunchConfig(target, start, end, duration, getter, setter, snap, strength, frequency, dampingRatio);
            return context;
        }
        public static ITweenContext<T, Target> DoPunch<T, Target>(Target target, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, T strength,
       int frequency = 10, float dampingRatio = 1, bool snap = false, bool autoRun = true)
        {
            return DoPunch<T, Target>(target, getter.Invoke(target), end, duration, getter, setter, strength, frequency, dampingRatio, snap, autoRun);
        }
        public static ITweenContext<T, Target> DoJump<T, Target>(Target target, T start, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, T strength,
   int jumpCount = 5, float jumpDamping = 2f, bool snap = false, bool autoRun = true)
        {
            var context = GetScheduler().AllocateContext<T, Target>(autoRun);
            context.AsInstance().JumpConfig(target, start, end, duration, getter, setter, snap, strength, jumpCount, jumpDamping);
            return context;
        }
        public static ITweenContext<T, Target> DoJump<T, Target>(Target target, T end, float duration, Func<Target, T> getter, Action<Target, T> setter, T strength,
int jumpCount = 5, float jumpDamping = 2f, bool snap = false, bool autoRun = true)
        {
            return DoJump<T, Target>(target, getter.Invoke(target), end, duration, getter, setter, strength, jumpCount, jumpDamping, snap, autoRun);

        }


        public static ITweenContext<T, Target> DoArray<T, Target>(Target target, float duration, Func<Target, T> getter, Action<Target, T> setter, T[] points, bool snap = false, bool autoRun = true)
        {
            var context = GetScheduler().AllocateContext<T, Target>(autoRun);
            context.AsInstance().BezierConfig(target, duration, getter, setter, snap, points);
            return context;
        }
 
        public static ITweenGroup Sequence() => GetScheduler().AllocateSequence();
        public static ITweenGroup Parallel() => GetScheduler().AllocateParallel();



        public static T AddTo<T>(this T context, ITweenContextBox box) where T : ITweenContext
        {
            box.AddTween(context);
            return context;
        }




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

    struct AnimationCurveEvaluator : IValueEvaluator
    {
        private AnimationCurve _curve;

        public AnimationCurveEvaluator(AnimationCurve curve) => _curve = curve;
        float IValueEvaluator.Evaluate(float percent, float time, float duration) => _curve.Evaluate(percent);
    }
    struct EaseEvaluator : IValueEvaluator
    {
        public static EaseEvaluator Default = new EaseEvaluator(Ease.Linear);
        private Ease _ease;
        public EaseEvaluator(Ease ease) => _ease = ease;
        private static float Evaluate(Ease easeType, float time, float duration)
        {
            float percent = Mathf.Clamp01((time / duration));
            switch (easeType)
            {
                case Ease.Linear:
                    return percent;
                case Ease.InSine:
                    return -(float)Math.Cos((double)(percent * 1.5707964f)) + 1f;
                case Ease.OutSine:
                    return (float)Math.Sin((double)(percent * 1.5707964f));
                case Ease.InOutSine:
                    return -0.5f * ((float)Math.Cos((double)(3.1415927f * percent)) - 1f);
                case Ease.InQuad:
                    return percent * percent;
                case Ease.OutQuad:
                    return -(time /= duration) * (time - 2f);
                case Ease.InOutQuad:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * time * time;
                    }
                    return -0.5f * ((time -= 1f) * (time - 2f) - 1f);
                case Ease.InCubic:
                    return (time /= duration) * time * time;
                case Ease.OutCubic:
                    return (time = percent - 1f) * time * time + 1f;
                case Ease.InOutCubic:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * time * time * time;
                    }
                    return 0.5f * ((time -= 2f) * time * time + 2f);
                case Ease.InQuart:
                    return (time /= duration) * time * time * time;
                case Ease.OutQuart:
                    return -((time = percent - 1f) * time * time * time - 1f);
                case Ease.InOutQuart:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * time * time * time * time;
                    }
                    return -0.5f * ((time -= 2f) * time * time * time - 2f);
                case Ease.InQuint:
                    return (time /= duration) * time * time * time * time;
                case Ease.OutQuint:
                    return (time = percent - 1f) * time * time * time * time + 1f;
                case Ease.InOutQuint:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * time * time * time * time * time;
                    }
                    return 0.5f * ((time -= 2f) * time * time * time * time + 2f);
                case Ease.InExpo:
                    if (time != 0f)
                    {
                        return (float)Math.Pow(2.0, (double)(10f * (percent - 1f)));
                    }
                    return 0f;
                case Ease.OutExpo:
                    if (time == duration)
                    {
                        return 1f;
                    }
                    return -(float)Math.Pow(2.0, (double)(-10f * percent)) + 1f;
                case Ease.InOutExpo:
                    if (time == 0f)
                    {
                        return 0f;
                    }
                    if (time == duration)
                    {
                        return 1f;
                    }
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * (float)Math.Pow(2.0, (double)(10f * (time - 1f)));
                    }
                    return 0.5f * (-(float)Math.Pow(2.0, (double)(-10f * (time -= 1f))) + 2f);
                case Ease.InCirc:
                    return -((float)Math.Sqrt((double)(1f - (time /= duration) * time)) - 1f);
                case Ease.OutCirc:
                    return (float)Math.Sqrt((double)(1f - (time = percent - 1f) * time));
                case Ease.InOutCirc:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return -0.5f * ((float)Math.Sqrt((double)(1f - time * time)) - 1f);
                    }
                    return 0.5f * ((float)Math.Sqrt((double)(1f - (time -= 2f) * time)) + 1f);

                default:
                    return -(time /= duration) * (time - 2f);
            }
        }
        float IValueEvaluator.Evaluate(float percent, float time, float duration) => Evaluate(_ease, time, duration);
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


    abstract class ValueCalculator<T>
    {
        public const float E = 2.71828175F;
        public const float PI = 3.14159274F;
        protected static void EvaluateJump(int jumpCount, float percent, out int devCount, out float jumpAdd)
        {
            if (percent == 0 || percent == 1)
            {
                jumpAdd = 0;
                devCount = 0;
                return;
            }

            var gap = 1f / jumpCount;
            var _percent = (percent % gap) * jumpCount;
            devCount = Mathf.FloorToInt(percent / gap);
            jumpAdd = (1 - (4 * Mathf.Pow(_percent - 0.5f, 2)));
        }
        protected static float EvaluateStrength(int frequency, float dampingRatio, float t)
        {
            if (t == 1f || t == 0f)
            {
                return 0;
            }
            float angularFrequency = (frequency - 0.5f) * PI;
            float dampingFactor = dampingRatio * frequency / (2f * PI);
            return Mathf.Cos(angularFrequency * t) * Mathf.Pow(E, -dampingFactor * t);
        }
        public abstract T Calculate(TweenType mode, T start, T end, float percent, T srcValue,
            float srcPercent, bool snap, T strength, int frequency, float dampingRatio, int jumpCount, float jumpDamping, T[] points);
        public abstract T CalculatorEnd(T start, T end);



        protected static float Range(float min, float max) => UnityEngine.Random.Range(min, max);

        protected static float Range1() => Range(-1, 1);



        private static Dictionary<int, float[]> arrays_Bezier = new Dictionary<int, float[]>();
        private static float[] AllocateBezierArray(int length)
        {
            var min = 16;
            while (min < length)
                min *= 2;
            float[] result = null;
            if (arrays_Bezier.TryGetValue(min, out result))
            {
                return result;
            }
            return new float[min];
        }
        protected static void CycleBezierArray(float[] arr)
        {
            arrays_Bezier[arr.Length] = arr;
        }
        protected static float[] EvaluateBezier(float percent, int length)
        {
            float u = 1f - percent;

            // 使用Bernstein多项式递推关系优化计算
            float[] tempCoefficients = AllocateBezierArray(length);
            tempCoefficients[0] = Mathf.Pow(u, length - 1);

            for (int i = 1; i < length; i++)
                tempCoefficients[i] = tempCoefficients[i - 1] * (percent / u) * (length - i) / i;

            return tempCoefficients;
        }
    }
    class ValueCalculator_Float : ValueCalculator<float>
    {
        public override float Calculate(TweenType mode, float start, float end, float percent, float srcValue, float srcPercent, bool snap, float strength, int frequency, float dampingRatio, int jumpCount, float jumpDamping, float[] points)
        {

            float dest = 0;
            if (mode != TweenType.Bezier)
                dest = Mathf.Lerp(start, end, percent);
            else
            {
                if (percent == 1)
                    dest = points[points.Length - 1];
                else
                {
                    int length = points.Length;
                    float[] tempCoefficients = EvaluateBezier(percent, length);
                    for (int i = 0; i < length; i++)
                    {
                        dest += tempCoefficients[i] * points[i];
                    }
                    CycleBezierArray(tempCoefficients);
                }

            }


            dest = Mathf.Lerp(srcValue, dest, srcPercent);
            if (mode == TweenType.Shake || mode == TweenType.Punch)
            {
                strength = strength * end;

                var s = EvaluateStrength(frequency, dampingRatio, percent) * strength;
                if (mode == TweenType.Punch)
                    dest += s;

                else
                {
                    s *= percent;
                    dest += s * Range1();
                }
            }
            else if (mode == TweenType.Jump)
            {
                int devCount;
                float jumpAdd;
                EvaluateJump(jumpCount, percent, out devCount, out jumpAdd);
                for (int i = 0; i < devCount; i++)
                    strength /= jumpDamping;
                dest += strength * jumpAdd;

            }
            if (snap)
                return Mathf.RoundToInt(dest);
            return dest;
        }

        public override float CalculatorEnd(float start, float end) => end + end - start;


    }
    class ValueCalculator_Int : ValueCalculator<int>
    {


        public override int Calculate(TweenType mode, int start, int end, float percent, int srcValue, float srcPercent, bool snap, int strength, int frequency, float dampingRatio, int jumpCount, float jumpDamping, int[] points)
        {
            float dest = 0;
            if (mode != TweenType.Bezier)
                dest = Mathf.Lerp(start, end, percent);
            else
            {
                if (percent == 1)
                    dest = points[points.Length - 1];
                else
                {
                    int length = points.Length;
                    float[] tempCoefficients = EvaluateBezier(percent, length);
                    for (int i = 0; i < length; i++)
                    {
                        dest += tempCoefficients[i] * points[i];
                    }
                    CycleBezierArray(tempCoefficients);
                }

            }

            dest = Mathf.Lerp(srcValue, dest, srcPercent);
            if (mode == TweenType.Shake || mode == TweenType.Punch)
            {
                strength = strength * end;

                var s = EvaluateStrength(frequency, dampingRatio, percent) * strength;
                if (mode == TweenType.Punch)
                    dest += s;

                else
                {
                    s *= percent;
                    dest += s * Range1();
                }
            }
            else if (mode == TweenType.Jump)
            {
                int devCount;
                float jumpAdd;
                EvaluateJump(jumpCount, percent, out devCount, out jumpAdd);
                float _s = (float)strength;
                for (int i = 0; i < devCount; i++)
                    _s /= jumpDamping;
                dest += _s * jumpAdd;

            }
            return Mathf.RoundToInt(dest);
        }

        public override int CalculatorEnd(int start, int end) => end + end - start;
    }



    class ValueCalculator_Color : ValueCalculator<Color>
    {
        public override Color Calculate(TweenType mode, Color start, Color end, float percent, Color srcValue, float srcPercent, bool snap, Color strength, int frequency, float dampingRatio, int jumpCount, float jumpDamping, Color[] points)
        {


            Color dest = Color.clear;
            if (mode != TweenType.Bezier)
                dest = Color.Lerp(start, end, percent);
            else
            {
                if (percent == 1)
                    dest = points[points.Length - 1];
                else
                {
                    int length = points.Length;
                    float[] tempCoefficients = EvaluateBezier(percent, length);
                    for (int i = 0; i < length; i++)
                    {
                        dest += tempCoefficients[i] * points[i];
                    }
                    CycleBezierArray(tempCoefficients);
                }

            }


            dest = Color.Lerp(srcValue, dest, srcPercent);

            if (mode == TweenType.Shake || mode == TweenType.Punch)
            {
                strength = new Color(strength.r * end.r, strength.g * end.g, strength.b * strength.b, strength.a * end.a);

                var s = EvaluateStrength(frequency, dampingRatio, percent) * strength;
                if (mode == TweenType.Punch)
                    dest += s;

                else
                {
                    s *= percent;
                    dest.r += s.r * Range1();
                    dest.g += s.g * Range1();
                    dest.b += s.b * Range1();
                    dest.a += s.a * Range1();

                }
            }
            else if (mode == TweenType.Jump)
            {
                int devCount;
                float jumpAdd;
                EvaluateJump(jumpCount, percent, out devCount, out jumpAdd);
                for (int i = 0; i < devCount; i++)
                    strength /= jumpDamping;
                dest += strength * jumpAdd;

            }

            if (snap)
            {
                dest.a = Mathf.RoundToInt(dest.a);
                dest.r = Mathf.RoundToInt(dest.r);
                dest.g = Mathf.RoundToInt(dest.g);
                dest.b = Mathf.RoundToInt(dest.b);
            }

            return dest;
        }

        public override Color CalculatorEnd(Color start, Color end) => end + end - start;
    }

    class ValueCalculator_Rect : ValueCalculator<Rect>
    {
        private static Rect Lerp(Rect r, Rect a, Rect b, float t)
        {
            r.x = Mathf.Lerp(a.x, b.x, t);
            r.y = Mathf.Lerp(a.y, b.y, t);
            r.width = Mathf.Lerp(a.width, b.width, t);
            r.height = Mathf.Lerp(a.height, b.height, t);
            return r;
        }
        public override Rect Calculate(TweenType mode, Rect start, Rect end, float percent, Rect srcValue, float srcPercent, bool snap, Rect strength, int frequency, float dampingRatio, int jumpCount, float jumpDamping, Rect[] points)
        {


            Rect dest = Rect.zero;
            if (mode != TweenType.Bezier)
                dest = Lerp(start, start, end, percent);
            else
            {
                if (percent == 1)
                    dest = points[points.Length - 1];
                else
                {
                    int length = points.Length;
                    float[] tempCoefficients = EvaluateBezier(percent, length);
                    for (int i = 0; i < length; i++)
                    {
                        var value = tempCoefficients[i];
                        var point = points[i];
                        dest.x += point.x * value;
                        dest.y += point.y * value;
                        dest.width += point.width * value;
                        dest.height += point.height * value;

                    }
                    CycleBezierArray(tempCoefficients);
                }

            }




            dest = Lerp(start, srcValue, dest, srcPercent);
            if (mode == TweenType.Shake || mode == TweenType.Punch)
            {
                Vector4 _strength = new Vector4(strength.x * end.x, strength.y * end.y, strength.width * strength.width, strength.height * end.height);

                var s = EvaluateStrength(frequency, dampingRatio, percent) * _strength;
                if (mode == TweenType.Punch)
                {
                    dest.x += s.x;
                    dest.y += s.y;
                    dest.width += s.z;
                    dest.height += s.w;
                }

                else
                {
                    s *= percent;
                    dest.x += s.x * Range1();
                    dest.y += s.y * Range1();
                    dest.width += s.z * Range1();
                    dest.height += s.w * Range1();

                }
            }
            else if (mode == TweenType.Jump)
            {
                int devCount;
                float jumpAdd;
                EvaluateJump(jumpCount, percent, out devCount, out jumpAdd);
                Vector4 _s = new Vector4(strength.x, strength.y, strength.width, strength.height);
                for (int i = 0; i < devCount; i++)
                    _s /= jumpDamping;
                _s *= jumpAdd;
                dest.x += _s.x;
                dest.y += _s.y;
                dest.width += _s.z;
                dest.height += _s.w;
            }
            if (snap)
            {
                dest.x = Mathf.RoundToInt(dest.x);
                dest.y = Mathf.RoundToInt(dest.y);
                dest.width = Mathf.RoundToInt(dest.width);
                dest.height = Mathf.RoundToInt(dest.height);
            }
            return dest;
        }

        public override Rect CalculatorEnd(Rect start, Rect end)
        {
            end.x *= 2;
            end.y *= 2;
            end.width *= 2;
            end.height *= 2;
            end.x -= start.x;
            end.y -= start.y;
            end.width -= start.width;
            end.height -= start.height;
            return end;
        }
    }
    class ValueCalculator_Vector2 : ValueCalculator<Vector2>
    {
        public override Vector2 Calculate(TweenType mode, Vector2 start, Vector2 end, float percent, Vector2 srcValue, float srcPercent, bool snap, Vector2 strength, int frequency, float dampingRatio, int jumpCount, float jumpDamping, Vector2[] points)
        {
            Vector2 dest = Vector2.zero;
            if (mode != TweenType.Bezier)
                dest = Vector2.Lerp(start, end, percent);
            else
            {
                if (percent == 1)
                    dest = points[points.Length - 1];
                else
                {
                    int length = points.Length;
                    float[] tempCoefficients = EvaluateBezier(percent, length);
                    for (int i = 0; i < length; i++)
                    {
                        dest += tempCoefficients[i] * points[i];
                    }
                    CycleBezierArray(tempCoefficients);
                }

            }

            dest = Vector3.Lerp(srcValue, dest, srcPercent);
            if (mode == TweenType.Shake || mode == TweenType.Punch)
            {
                strength = new Vector2(strength.x * end.x, strength.y * end.y);

                var s = EvaluateStrength(frequency, dampingRatio, percent) * strength;
                if (mode == TweenType.Punch)
                    dest += s;

                else
                {
                    s *= percent;
                    dest.x += s.x * Range1();
                    dest.y += s.y * Range1();

                }
            }
            else if (mode == TweenType.Jump)
            {
                int devCount;
                float jumpAdd;
                EvaluateJump(jumpCount, percent, out devCount, out jumpAdd);
                for (int i = 0; i < devCount; i++)
                    strength /= jumpDamping;
                dest += strength * jumpAdd;

            }
            if (snap)
            {
                dest.x = Mathf.RoundToInt(dest.x);
                dest.y = Mathf.RoundToInt(dest.y);
            }
            return dest;
        }

        public override Vector2 CalculatorEnd(Vector2 start, Vector2 end) => end + end - start;
    }
    class ValueCalculator_Vector3 : ValueCalculator<Vector3>
    {


        public override Vector3 Calculate(TweenType mode, Vector3 start, Vector3 end, float percent, Vector3 srcValue, float srcPercent, bool snap, Vector3 strength, int frequency, float dampingRatio, int jumpCount, float jumpDamping, Vector3[] points)
        {
            Vector3 dest = Vector3.zero;
            if (mode != TweenType.Bezier)
                dest = Vector3.Lerp(start, end, percent);
            else
            {
                if (percent == 1)
                    dest = points[points.Length - 1];
                else
                {
                    int length = points.Length;
                    float[] tempCoefficients = EvaluateBezier(percent, length);
                    for (int i = 0; i < length; i++)
                    {
                        dest += tempCoefficients[i] * points[i];
                    }
                    CycleBezierArray(tempCoefficients);
                }

            }

            dest = Vector3.Lerp(srcValue, dest, srcPercent);
            if (mode == TweenType.Shake || mode == TweenType.Punch)
            {
                strength = new Vector3(strength.x * end.x, strength.y * end.y, strength.z * strength.z);

                var s = EvaluateStrength(frequency, dampingRatio, percent) * strength;
                if (mode == TweenType.Punch)
                {
                    dest += s;
                }
                else
                {
                    s *= percent;
                    dest.x += s.x * Range1();
                    dest.y += s.y * Range1();
                    dest.z += s.z * Range1();

                }
            }
            else if (mode == TweenType.Jump)
            {
                int devCount;
                float jumpAdd;
                EvaluateJump(jumpCount, percent, out devCount, out jumpAdd);
                for (int i = 0; i < devCount; i++)
                    strength /= jumpDamping;
                dest += strength * jumpAdd;

            }

            if (snap)
            {
                dest.x = Mathf.RoundToInt(dest.x);
                dest.y = Mathf.RoundToInt(dest.y);
                dest.z = Mathf.RoundToInt(dest.z);
            }
            return dest;
        }

        public override Vector3 CalculatorEnd(Vector3 start, Vector3 end) => end + end - start;
    }
    class ValueCalculator_Vector4 : ValueCalculator<Vector4>
    {



        public override Vector4 Calculate(TweenType mode, Vector4 start, Vector4 end, float percent, Vector4 srcValue, float srcPercent, bool snap, Vector4 strength, int frequency, float dampingRatio, int jumpCount, float jumpDamping, Vector4[] points)
        {
            Vector4 dest = Vector4.zero;
            if (mode != TweenType.Bezier)
                dest = Vector4.Lerp(start, end, percent);
            else
            {
                if (percent == 1)
                    dest = points[points.Length - 1];
                else
                {
                    int length = points.Length;
                    float[] tempCoefficients = EvaluateBezier(percent, length);
                    for (int i = 0; i < length; i++)
                    {
                        dest += tempCoefficients[i] * points[i];
                    }
                    CycleBezierArray(tempCoefficients);
                }

            }

            dest = Vector3.Lerp(srcValue, dest, srcPercent);
            if (mode == TweenType.Shake || mode == TweenType.Punch)
            {
                strength = new Vector4(strength.x * end.x, strength.y * end.y, strength.z * strength.z, strength.z * end.z);

                var s = EvaluateStrength(frequency, dampingRatio, percent) * strength;
                if (mode == TweenType.Punch)
                    dest += s;

                else
                {
                    s *= percent;
                    dest.x += s.x * Range1();
                    dest.y += s.y * Range1();
                    dest.z += s.z * Range1();
                    dest.w += s.w * Range1();

                }
            }
            else if (mode == TweenType.Jump)
            {
                int devCount;
                float jumpAdd;
                EvaluateJump(jumpCount, percent, out devCount, out jumpAdd);
                for (int i = 0; i < devCount; i++)
                    strength /= jumpDamping;
                dest += strength * jumpAdd;

            }
            if (snap)
            {
                dest.x = Mathf.RoundToInt(dest.x);
                dest.y = Mathf.RoundToInt(dest.y);
                dest.z = Mathf.RoundToInt(dest.z);
                dest.w = Mathf.RoundToInt(dest.w);

            }
            return dest;
        }

        public override Vector4 CalculatorEnd(Vector4 start, Vector4 end) => end + end - start;
    }




}