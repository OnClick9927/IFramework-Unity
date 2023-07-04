namespace IFramework.Message
{
    /// <summary>
    /// 消息状态
    /// </summary>
    public enum MessageState
    {
        /// <summary>
        /// 休息
        /// </summary>
        Rest,
        /// <summary>
        /// 等待被发布
        /// </summary>
        Wait,
        /// <summary>
        /// 正在发布中
        /// </summary>
        Lock,
    }
}
