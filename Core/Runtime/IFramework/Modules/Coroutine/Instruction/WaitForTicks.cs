using System;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 等待ticks(100微秒)
    /// </summary>
    public class WaitForTicks : WaitForTimeSpan
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="ticks">等待的tick数</param>
        public WaitForTicks(long ticks) : base(TimeSpan.FromTicks(ticks))
        {
        }
    }
}
