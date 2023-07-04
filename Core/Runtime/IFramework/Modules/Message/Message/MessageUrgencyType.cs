namespace IFramework.Message
{
    /// <summary>
    /// 消息紧急程度
    /// </summary>
    public enum MessageUrgencyType
    {
        /// <summary>
        /// 立刻
        /// </summary>
        Immediately= MessageUrgency.Immediately,
        /// <summary>
        /// 非常紧急
        /// </summary>
        VeryUrgent = MessageUrgency.VeryUrgent,
        /// <summary>
        /// 紧急
        /// </summary>
        Urgent = MessageUrgency.Urgent,
        /// <summary>
        /// 重要的
        /// </summary>
        Important = MessageUrgency.Important,
        /// <summary>
        /// 普通的
        /// </summary>
        Common = MessageUrgency.Common,
        /// <summary>
        /// 不重要的
        /// </summary>
        Unimportant = MessageUrgency.Unimportant,
        /// <summary>
        /// 可有可无的
        /// </summary>
        Dispensable= MessageUrgency.Dispensable,
    }
}
