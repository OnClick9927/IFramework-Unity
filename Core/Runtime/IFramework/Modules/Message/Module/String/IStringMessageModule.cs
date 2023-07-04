namespace IFramework.Message
{
    /// <summary>
    /// 消息模块（String版本）
    /// </summary>
    public interface IStringMessageModule
    {
        /// <summary>
        /// 剩余消息数目
        /// </summary>
        int count { get; }
        /// <summary>
        /// 每一帧处理消息上限
        /// </summary>
        int processesPerFrame { get; set; }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        IMessage Publish(string type, IEventArgs args, int code = 0, MessageUrgencyType priority = MessageUrgencyType.Common);
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        IMessage PublishByNumber(string type, IEventArgs args, int code = 0, int priority = MessageUrgency.Common);
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        void Subscribe(string type, IMessageListener listener);
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        void Subscribe(string type, MessageListener listener);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        void UnSubscribe(string type, IMessageListener listener);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        void UnSubscribe(string type, MessageListener listener);
    }
}