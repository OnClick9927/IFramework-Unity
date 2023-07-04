using System.Collections;

namespace IFramework.Coroutine
{
    /// <summary>
    /// 协程模块
    /// </summary>
    public interface ICoroutineModule
    {
        /// <summary>
        /// 创建一个携程不跑
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        ICoroutine CreateCoroutine(IEnumerator routine);
        /// <summary>
        /// 开启一个协程
        /// CreateCoroutine + ResumeCoroutine
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        ICoroutine StartCoroutine(IEnumerator routine);
        /// <summary>
        /// 挂起携程
        /// </summary>
        /// <param name="coroutine"></param>
        void PauseCoroutine(ICoroutine coroutine);
        /// <summary>
        /// 恢复运行
        /// </summary>
        /// <param name="coroutine"></param>
        void ResumeCoroutine(ICoroutine coroutine);

        /// <summary>
        /// 关闭一个携程
        /// </summary>
        /// <param name="coroutine"></param>
        void StopCoroutine(ICoroutine coroutine);
    }
}