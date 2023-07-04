using System.Collections;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 等待帧数
    /// </summary>
    public class WaitForFrames : YieldInstruction
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="count">帧数 </param>
        public WaitForFrames(int count)
        {
            _curCount = 0;
            this._count = count;
        }
        private int _curCount;
        private int _count { get; }

        /// <summary>
        /// over
        /// </summary>
        /// <returns></returns>
        protected override bool IsCompelete()
        {
            _curCount++;
            return _curCount >= _count;
        }
    }
}
