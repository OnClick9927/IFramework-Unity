using System;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 等待分钟
    /// </summary>
    public class WaitForMinutes : WaitForTimeSpan
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="minutes">等待分钟数</param>
        public WaitForMinutes(double minutes) : base(TimeSpan.FromMinutes(minutes))
        {
        }
    }
}
