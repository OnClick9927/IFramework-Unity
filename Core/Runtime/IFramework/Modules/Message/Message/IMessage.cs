using System;

namespace IFramework.Message
{
    /// <summary>
    /// 消息
    /// </summary>
    public interface IMessage:IAwaitable<MessageAwaiter>
    {
        /// <summary>
        /// 发送消息类型
        /// </summary>
        Type subject { get; }

        /// <summary>
        /// 发送消息类型
        /// </summary>
        string stringSubject { get; }
        /// <summary>
        /// 承载消息内容
        /// </summary>
        IEventArgs args { get; }
        /// <summary>
        /// code,帮助区分 args
        /// </summary>
        int code { get; }

        /// <summary>
        /// 消息状态
        /// </summary>
        MessageState state { get; }

        /// <summary>
        /// 消息发送结果
        /// </summary>
        MessageErrorCode errorCode { get; }
        /// <summary>
        /// 消息主题类型
        /// </summary>
        SubjectType type { get; }

        #region 仅在 state 为 MessageState.Wait时有效

        /// <summary>
        /// 设置Code，
        /// 仅在 state 为 MessageState.Wait时有效
        /// </summary>
        /// <param name="code"></param>
        IMessage SetCode(int code);


        /// <summary>
        /// 仅在 state 为 MessageState.Wait时有效
        /// 消息发布完成时的引用
        /// </summary>
        /// <param name="action"></param>
        IMessage OnCompelete(Action<IMessage> action);
        #endregion
    }
}
