using System;
using System.Collections;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 等待条件不成立
    /// </summary>
    public class WaitWhile : YieldInstruction
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="condition">等待不成立条件</param>
        public WaitWhile(Func<bool> condition)
        {
            _condition = condition;
        }

        private Func<bool> _condition { get; }
        /// <summary>
        /// override
        /// </summary>
        /// <returns></returns>
        protected override bool IsCompelete()
        {
            return !_condition.Invoke();
        }
    }
}
