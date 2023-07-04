namespace IFramework.Timer
{
    /// <summary>
    /// 当前时间的状态
    /// </summary>
    public enum EntityState
    {
        /// <summary>
        /// 未开始
        /// </summary>
        NotStart,
        /// <summary>
        /// 正在等待
        /// </summary>
        Waiting,
        /// <summary>
        /// 正在执行
        /// </summary>
        Running,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause,
        /// <summary>
        /// 完成
        /// </summary>
        Done,
        /// <summary>
        /// 无状态(默认状态)
        /// </summary>
        None
    }
}
