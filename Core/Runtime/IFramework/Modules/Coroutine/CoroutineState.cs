namespace IFramework.Coroutine
{
    /// <summary>
    /// 携程的状态
    /// </summary>
    public enum CoroutineState
    {
        /// <summary>
        /// 干活中
        /// </summary>
        Working,
        /// <summary>
        /// 挂起中，刚创建/主动挂起了
        /// </summary>
        Yied,
        /// <summary>
        /// 干完了，第一帧，处于等待被回收
        /// 休息中
        /// </summary>
        Rest,
    }
}
