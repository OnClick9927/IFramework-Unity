using System;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 等待日子
    /// </summary>
    public class WaitForDays : WaitForTimeSpan
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="days">天数</param>
        public WaitForDays(double days) : base(TimeSpan.FromDays(days))
        {
        }
    }
}
