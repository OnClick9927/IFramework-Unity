using System;

namespace IFramework.Message
{
    /// <summary>
    /// 消息监听者
    /// </summary>
    public interface IMessageListener
    {
        /// <summary>
        /// 收到消息回调
        /// </summary>
        /// <param name="message"></param>
        void Listen(IMessage message);
    }
}
