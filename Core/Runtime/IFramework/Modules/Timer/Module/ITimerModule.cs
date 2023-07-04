using System;

namespace IFramework.Timer
{
    /// <summary>
    /// 计时器
    /// </summary>
    public interface ITimerModule
    {
        /// <summary>
        /// 注册方法
        /// </summary>
        /// <param name="actionItem"></param>
        void Subscribe(ITimerEntity actionItem);
        /// <summary>
        /// 清除所有定时方法
        /// </summary>
        void Clear();
        /// <summary>
        /// 分配
        /// </summary>
        /// <returns></returns>
        ITimerEntity Allocate(Action action, float repeatDelay, int repeat = 1, float delay = 0, float timeScale = 1f);
    }
}
