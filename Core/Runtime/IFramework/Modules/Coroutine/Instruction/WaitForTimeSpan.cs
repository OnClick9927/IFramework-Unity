using System;
using System.Collections;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 等待时间
    /// </summary>
    public class WaitForTimeSpan : YieldInstruction
    {
        private DateTime _setTime;
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="span"> 等待时间</param>
        public WaitForTimeSpan(TimeSpan span) : base()
        {
            _setTime = DateTime.Now + span;
        }
        /// <summary>
        /// override
        /// </summary>
        /// <returns></returns>
        protected override bool IsCompelete()
        {
            return DateTime.Now >= _setTime;
        }
    }
}
