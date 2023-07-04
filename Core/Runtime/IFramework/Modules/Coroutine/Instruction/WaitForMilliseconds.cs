using System;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 等待毫秒
    /// </summary>
    public class WaitForMilliseconds : WaitForTimeSpan
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="milliseconds">毫秒</param>
        public WaitForMilliseconds(double milliseconds) : base(TimeSpan.FromMilliseconds(milliseconds))
        {
        }
    }
}
