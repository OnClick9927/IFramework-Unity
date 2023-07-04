using System;

namespace IFramework.Message
{
    /// <summary>
    /// 消息模块
    /// </summary>
    public interface IMessageModule
    {
        /// <summary>
        /// 剩余消息数目
        /// </summary>
        int count { get; }
        /// <summary>
        /// 适配子类型
        /// </summary>
        bool fitSubType { get; set; }
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
        IMessage Publish(Type type, IEventArgs args, int code = 0, MessageUrgencyType priority = MessageUrgencyType.Common);
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority"></param>
        IMessage Publish<T>(IEventArgs args, int code = 0, MessageUrgencyType priority = MessageUrgencyType.Common);
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority"></param>
        IMessage Publish<T>(T t, IEventArgs args, int code = 0, MessageUrgencyType priority = MessageUrgencyType.Common);
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority"></param>
        IMessage PublishByNumber<T>(T t, IEventArgs args, int code = 0, int priority = MessageUrgency.Common);
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority"></param>
        IMessage PublishByNumber<T>(IEventArgs args, int code = 0, int priority = MessageUrgency.Common);
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority"></param>
        IMessage PublishByNumber(Type type, IEventArgs args, int code = 0, int priority = MessageUrgency.Common);

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        void Subscribe(Type type, IMessageListener listener);
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        void Subscribe(Type type, MessageListener listener);
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        void Subscribe<T>(IMessageListener listener);
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        void Subscribe<T>(MessageListener listener);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        void UnSubscribe(Type type, IMessageListener listener);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        void UnSubscribe(Type type, MessageListener listener);
        /// <summary>
        /// 移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        void UnSubscribe<T>(IMessageListener listener);
        /// <summary>
        /// 移除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        void UnSubscribe<T>(MessageListener listener);
    }
}