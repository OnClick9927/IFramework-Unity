using System.Collections;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 所有等待类的基类
    /// </summary>
    public abstract class YieldInstruction
    {

        /// <summary>
        /// 是否结束
        /// </summary>
        public virtual bool isDone
        {
            get
            {
                return IsCompelete();
            }
        }

        /// <summary>
        /// 是否结束
        /// </summary>
        /// <returns></returns>
        protected abstract bool IsCompelete();

        internal IEnumerator AsEnumerator()
        {
            while (!isDone)
            {
                yield return false;
            }
            yield return true;
        }
    }
}
