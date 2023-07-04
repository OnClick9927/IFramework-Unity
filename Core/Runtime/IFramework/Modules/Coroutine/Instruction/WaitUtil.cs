using System;
using System.Collections;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 等待条件成立
    /// </summary>
    public class WaitUtil : YieldInstruction
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="condition">等待成立条件</param>
        public WaitUtil(Func<bool> condition)
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
            return _condition.Invoke();
        }
    }
}
