using System;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 等待小时
    /// </summary>
    public class WaitForHours : WaitForTimeSpan
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="hours">小时</param>
        public WaitForHours(double hours) : base(TimeSpan.FromHours(hours))
        {
        }
    }
}
