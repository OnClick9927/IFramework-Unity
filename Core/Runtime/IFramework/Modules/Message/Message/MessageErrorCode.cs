namespace IFramework.Message
{
    /// <summary>
    /// 消息发送结果
    /// </summary>
    public enum MessageErrorCode
    {
        /// <summary>
        /// 无状态,即不在使用
        /// </summary>
        None,
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 无人监听
        /// </summary>
        NoneListen,

    }
}
