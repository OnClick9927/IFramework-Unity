using System;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 等待秒
    /// </summary>
    public class WaitForSeconds : WaitForTimeSpan
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="seconds">等待秒数</param>
        public WaitForSeconds(double seconds) : base(TimeSpan.FromSeconds(seconds))
        {
        }
    }
}
