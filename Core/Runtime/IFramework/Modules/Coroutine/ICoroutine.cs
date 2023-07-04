namespace IFramework.Coroutine
{
    /// <summary>
    /// 协程实体
    /// </summary>
    public interface ICoroutine : IAwaitable<CoroutineAwaiter>
    {
        /// <summary>
        /// 目前状态
        /// </summary>
        CoroutineState state { get; }
        /// <summary>
        /// 手动结束
        /// </summary>
        void Compelete();
        /// <summary>
        /// 挂起
        /// </summary>
        void Pause();
        /// <summary>
        /// 恢复运行
        /// </summary>
        void Resume();
    }
}