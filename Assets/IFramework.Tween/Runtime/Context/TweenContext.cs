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
using UnityEngine;

namespace IFramework
{
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


        private static Dictionary<int, T[]> arrays_Bezier = new Dictionary<int, T[]>();
        public static T[] AllocateArray(int length)
        {
            var min = 16;
            while (min < length)
                min *= 2;
            T[] result = null;
            if (arrays_Bezier.TryGetValue(min, out result))
            {
                return result;
            }
            return new T[min];
        }
        public static void CycleArray(T[] arr)
        {
            arrays_Bezier[arr.Length] = arr;
        }


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
        private T[] _points;
        private int _points_length;

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

            if (this._points != null)
            {
                CycleArray(this._points);
                this._points = null;
            }
            if (_mode == TweenType.Bezier || _mode == TweenType.Array)
            {
                _points_length = this.points.Length;
                this._points = AllocateArray(_points_length);
                for (int i = 0; i < _points_length; i++)
                {
                    this._points[i] = this.points[i];
                }
            }

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
            T _cur = calc.Calculate(_mode, _start, _end, _convertPercent, src, _deltaPercent, snap, strength,
                frequency, dampingRatio, jumpCount, jumpDamping, _points, _points_length);
            if (!src.Equals(_cur))
                setter?.Invoke(target, _cur);
        }


        private void OnLoopEnd()
        {
            if (_mode == TweenType.Bezier || _mode == TweenType.Array)
            {
                if (loopType == LoopType.PingPong)
                {
                    for (int i = 0; i < _points_length; i++)
                    {
                        this._points[i] = this.points[_points_length - 1 - i];

                    }
                }
                else if (loopType == LoopType.Add)
                {
                    var gap = calc.Minus(this._points[_points_length - 1], this._points[0]);

                    for (int i = 0; i < _points_length; i++)
                    {
                        this._points[i] = calc.Add(this._points[i], gap);
                    }
                }
            }
            else
            {
                if (loopType == LoopType.PingPong)
                {
                    var temp = _start;
                    _start = _end;
                    _end = temp;
                }
                else if (loopType == LoopType.Add)
                {
                    var gap = calc.Minus(_end, _start);
                    var tmp = calc.Add(_end, gap);
                    _start = _end;
                    _end = tmp;
                }
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
            if (points == null || points.Length <= 2)
            {
                Log.FE("At Least 3 point");
            }
            this.Config(target, default, default, duration, getter, setter, snap);
            _mode = TweenType.Bezier;
            this.points = points;
            return this;
        }

        public TweenContext<T, Target> ArrayConfig(Target target, float duration, Func<Target, T> getter, Action<Target, T> setter, bool snap, T[] points)
        {
            if (points == null || points.Length <= 2)
            {
                Log.FE("At Least 3 point");
            }
            this.Config(target, default, default, duration, getter, setter, snap);
            _mode = TweenType.Array;
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



}