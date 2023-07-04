using System;
using System.Collections.Generic;

namespace IFramework.Timer
{
    /// <summary>
    /// 定时方法元素
    /// </summary>
    public class TimerEntity : ITimerEntity
    {
        #region 内部定义
        internal int _repeat;
        internal float _repeatDelay;  //每次循环的
        internal float _delay; //开始调用前的等待时间
        internal float _timeScale;
        internal EntityState _state; //状态
        internal Action _action;
        internal TimerModule timer;
        private bool _isWaiting=true;  //是否处于等待状态

        private int _doneCount; //已经完成数量
        private float _doneTimeSinceLastTime; //距离上一次调用方法已经经过的时间

        private Action onStart;
        private Action onUpdate;
        private Action onComplete;

        //维护两个列表
        internal List<TimerEntity> _parallelTimers = new List<TimerEntity>(); //并行
        internal List<TimerEntity> _serialTimers = new List<TimerEntity>(); //串行
        /// <summary>
        /// 重置
        /// </summary>
        internal virtual void Reset()
        {
            _repeat = 0;
            _action = null;
            _state = EntityState.None;
            _repeatDelay = 0f;
            _delay = 0f;
            _timeScale = 1f;
            _isWaiting = true;
            _doneCount = 0;
            _doneTimeSinceLastTime = 0f;

            onStart = null;
            onUpdate = null;
            onComplete = null;

            //子项也要重置
            for (int i = 0; i < _parallelTimers.Count; i++)
            {
                _parallelTimers[i].Reset();
            }

            for (int i = 0; i < _serialTimers.Count; i++)
            {
                _serialTimers[i].Reset();
            }

            _parallelTimers.Clear();
            _serialTimers.Clear();
        }
        #endregion

        #region 对外属性器定义
        /// <summary>
        /// 循环等待时间
        /// </summary>
        public float repeatDelay { get { return _repeatDelay; } }
        /// <summary>
        /// 开始等待时间
        /// </summary>
        public float delay { get { return _delay; } }
        /// <summary>
        /// 执行次数
        /// </summary>
        public int repeat { get { return _repeat; } }
        /// <summary>
        /// 时间比例
        /// </summary>
        public float timeScale { get { return _timeScale; } }
        /// <summary>
        /// 当前状态
        /// </summary>
        public EntityState state { get { return _state; } }
        #endregion

        #region 回调方法注册
        /// <summary>
        /// 注册开始调用的回调方法
        /// </summary>
        /// <param name="startAction">回调方法</param>
        public void SubscribeStart(Action startAction)
        {
            onStart += startAction;
        }
        /// <summary>
        /// 解绑开始调用的回调方法
        /// </summary>
        /// <param name="startAction">回调方法</param>
        public void UnSubscribeStart(Action startAction)
        {
            onStart -= startAction;
        }
        /// <summary>
        /// 注册每帧的回调方法
        /// </summary>
        /// <param name="updateAction">回调方法</param>
        public void SubsribeUpdate(Action updateAction)
        {
            onUpdate += updateAction;
        }
        /// <summary>
        /// 解绑每帧的回调方法
        /// </summary>
        /// <param name="updateAction">回调方法</param>
        public void UnSubsribeUpdate(Action updateAction)
        {
            onUpdate -= updateAction;
        }
        /// <summary>
        /// 注册完成的回调方法
        /// </summary>
        /// <param name="completeAction">回调函数</param>
        public void SubscribeComplete(Action completeAction)
        {
            onComplete += completeAction;
        }
        /// <summary>
        /// 解绑完成的回调方法
        /// </summary>
        /// <param name="completeAction">回调函数</param>
        public void UnSubscribeComplete(Action completeAction)
        {
            onComplete -= completeAction;
        }
        #endregion

        #region 对外方法定义
        /// <summary>
        /// 取消
        /// </summary>
        public void Cancel(bool callComplete)
        {
            if (_state != EntityState.Running && _state != EntityState.Waiting)
            {
                //throw new InvalidOperationException("this task are not running");
                Log.E("this task are not running");
            }
            _state = EntityState.Done;
            if (callComplete)
            {
                Compelete();
            }

        }
        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            if (_state == EntityState.Done)
            {
                throw new InvalidOperationException("this task has been already done!");
            }
            _state = EntityState.Pause;
        }
        /// <summary>
        /// 继续/开始计时
        /// </summary>
        public void Start()
        {
            if (_state == EntityState.Done)
            {
                throw new InvalidOperationException("this task has been already done!");
            }
            _state = _isWaiting ? EntityState.Waiting : EntityState.Running;
            //onStart?.Invoke();
        }
        /// <summary>
        /// 调用一次
        /// </summary>
        private void Do()
        {
            _action?.Invoke();
            _doneCount++;
            if (_doneCount >= _repeat && _repeat != -1)
            {
                Compelete();
            }
        }

        private void Compelete()
        {
            _state = EntityState.Done;
            onComplete?.Invoke();
        }

        /// <summary>
        /// 刷新时间
        /// </summary>
        public void Update(float deltaTime)
        {
            //只有等待状态和运行状态可以调用
            if (_state != EntityState.Running && state != EntityState.Waiting) return;

            deltaTime *= _timeScale;
            _doneTimeSinceLastTime += deltaTime;

            //等待时间过了才能开始
            if (_state == EntityState.Waiting)
            {
                if (_doneTimeSinceLastTime >= _delay)
                {
                    _doneTimeSinceLastTime -= _delay;
                    _state = EntityState.Running;
                    onStart?.Invoke();
                }
                else
                {
                    return;
                }
            }

            if (_repeat == 0) Compelete();

            while (_doneTimeSinceLastTime >= _repeatDelay && _state == EntityState.Running)
            {
                _doneTimeSinceLastTime -= _repeatDelay;
                Do();
            }

            //并行全调用
            //_parallelTimers.ForEach(i => i.Update(deltaTime));
            for (int i = _parallelTimers.Count - 1; i >= 0; i--)
            {
                var entity = _parallelTimers[i];
                if (entity.state == EntityState.Done)
                {
                    _parallelTimers.Remove(entity);
                    entity.Reset();
                    continue;
                }
                else
                {
                    entity.Update(deltaTime);
                }
            }

            //串行调用一个
            if (_serialTimers.Count > 0)
            {
                var entity = _serialTimers[0];
                if (entity.state == EntityState.Done)
                {
                    _serialTimers.Remove(entity);
                    entity.Reset();
                }
            }
            if (_serialTimers.Count > 0)
            {
                _serialTimers[0].Update(deltaTime);
            }

            onUpdate?.Invoke();
        }
        #endregion


        /// <summary>
        /// 设置子定时器静态扩展
        /// </summary>
        /// <param name="timerEntity">子定时器</param>
        /// <param name="type">子定时器类型</param>
        public void SetInnerTimer(ITimerEntity timerEntity, InnerType type = InnerType.Parallel)
        {
            if (timerEntity == null)
                throw new ArgumentNullException("timer is null");

            timerEntity.Start();

            switch (type)
            {
                case InnerType.Parallel:
                    _parallelTimers.Add(timerEntity as TimerEntity);
                    break;
                case InnerType.Serial:
                    _serialTimers.Add(timerEntity as TimerEntity);
                    break;
            }
        }

        /// <summary>
        /// 注册静态扩展
        /// </summary>
        public void Subscribe()
        {
            timer.Subscribe(this);
        }
        /// <summary>
        /// 设置TimeScale
        /// </summary>
        public void SetTimeScale(float scale)
        {
            this._timeScale = scale;
        }

    }
}
