using System;
using System.Collections.Generic;

namespace IFramework.Timer
{
    /// <summary>
    /// 时间模块
    /// </summary>
    public class TimerModule : UpdateModule, ITimerModule
    {
        /// <summary>
        /// 优先级
        /// </summary>
        /// <returns></returns>
        protected override ModulePriority OnGetDefautPriority()
        {
            return ModulePriority.Timer;
        }

        private List<TimerEntity> _entities;
        DateTime _lastTime; //保存上一次的时间，便于计算DeltaTime;
        /// <summary>
        /// 清除方法
        /// </summary>
        public void Clear()
        {
            _entities.Clear();
        }
        /// <summary>
        /// 注册定时方法
        /// </summary>
        /// <param name="entity"></param>
        public void Subscribe(ITimerEntity entity)
        {
             
            if (entity.state != EntityState.NotStart)
            {
                Log.E("Don't Subscribe an Used entity");
            }

            if ((entity as TimerEntity).timer == null)
                Log.E("Don't Create new entity ,Use Allocate");

            if (_entities.Contains((entity as TimerEntity)))
            {
                throw new InvalidOperationException("This item has been already subscribed");
            }
            _entities.Add((entity as TimerEntity));
            entity.Start();
        }
        /// <summary>
        /// awake
        /// </summary>
        protected override void Awake()
        {
            _entities = new List<TimerEntity>();
            _lastTime = DateTime.Now;
        }

        /// <summary>
        /// dispose
        /// </summary>
        protected override void OnDispose()
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Reset();
            }
            _entities.Clear();
        }

        /// <summary>
        /// update
        /// </summary>
        protected override void OnUpdate()
        {
            if (_entities.Count <= 0) return;

            var deltaTime = (float)(DateTime.Now - _lastTime).TotalMilliseconds;

            _lastTime = DateTime.Now;
            
            for (int i = _entities.Count - 1; i >= 0; i--)
            {
                var entity = _entities[i];
                if (entity.state == EntityState.Done)
                {
                    _entities.Remove(entity);
                    entity.Reset();
                    continue;
                }

                entity.Update(deltaTime);
            }
        }

        /// <summary>
        ///  全局分配
        /// </summary>
        /// <param name="action">调用的方法</param>
        /// <param name="repeatDelay">延迟时间</param>
        /// <param name="repeat">执行次数</param>
        /// <param name="delay">开始定时器的等待时间</param>
        /// <param name="timeScale">时间比例</param>
        /// <returns>获取的TimerEntity</returns>
        public ITimerEntity Allocate(Action action, float repeatDelay, int repeat = 1,float delay = 0, float timeScale = 1f)
        {
            var entity = new TimerEntity();
            entity.timer = this;
            entity._action = action;
            entity._repeatDelay = repeatDelay;
            entity._repeat = repeat;
            entity._timeScale = timeScale;
            entity._delay = delay;
            entity._state = EntityState.NotStart;
            return entity;
        }
    }
}
