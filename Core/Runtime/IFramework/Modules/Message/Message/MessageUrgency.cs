namespace IFramework.Message
{
    /// <summary>
    /// 消息紧急程度
    /// </summary>
    public static class MessageUrgency
    {
        /// <summary>
        /// 立刻
        /// </summary>
        public const int Immediately = -1;
        /// <summary>
        /// 非常紧急
        /// </summary>
        public const int VeryUrgent =32;
        /// <summary>
        /// 紧急
        /// </summary>
        public const int Urgent = 128;
        /// <summary>
        /// 重要的
        /// </summary>
        public const int Important = 256;
        /// <summary>
        /// 普通的
        /// </summary>
        public const int Common = 512;
        /// <summary>
        /// 不重要的
        /// </summary>
        public const int Unimportant = 1024;
        /// <summary>
        /// 可有可无的
        /// </summary>
        public const int Dispensable = 4096;
    }

}
