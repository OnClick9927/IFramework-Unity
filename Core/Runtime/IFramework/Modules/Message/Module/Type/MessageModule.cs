using System;
namespace IFramework.Message
{
    /// <summary>
    /// 消息模块
    /// </summary>
    public class MessageModule : UpdateModule, IMessageModule
    {
        private TypeMessageQueue messages;
        private TypeHandlerQueue handlers;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override ModulePriority OnGetDefautPriority()
        {
            return ModulePriority.Message;
        }
        protected override void OnDispose()
        {
            handlers.Dispose();
            messages.Dispose();
        }
        protected override void Awake()
        {
            handlers = new TypeHandlerQueue();
            messages = new TypeMessageQueue(handlers);
        }


        protected override void OnUpdate()
        {
            handlers.Update();
            messages.Update();
        }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        /// <summary>
        /// 剩余消息数目
        /// </summary>
        public int count
        {
            get
            {
                return messages.count;
            }
        }
        /// <summary>
        /// 适配子类型
        /// </summary>
        public bool fitSubType { get { return handlers.fitSubType; } set { handlers.fitSubType = value; } }
        /// <summary>
        /// 每帧处理消息个数
        /// </summary>
        public int processesPerFrame { get { return messages.processesPerFrame; } set { messages.processesPerFrame = value; } }




        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void Subscribe(Type type, IMessageListener listener)
        {
            Subscribe(type, listener.Listen);
        }
        /// <summary>
        /// 注册监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void Subscribe<T>(IMessageListener listener)
        {
            Subscribe(typeof(T), listener);
        }
        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void UnSubscribe(Type type, IMessageListener listener)
        {
            UnSubscribe(type, listener.Listen);
        }
        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void UnSubscribe<T>(IMessageListener listener)
        {
            UnSubscribe(typeof(T), listener);
        }



        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void Subscribe(Type type, MessageListener listener)
        {
            handlers.Subscribe(type, listener);
        }
        /// <summary>
        /// 注册监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void Subscribe<T>(MessageListener listener)
        {
            Subscribe(typeof(T), listener);
        }
        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void UnSubscribe(Type type, MessageListener listener)
        {
            handlers.UnSubscribe(type, listener);
        }
        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void UnSubscribe<T>(MessageListener listener)
        {
            UnSubscribe(typeof(T), listener);
        }


        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage Publish<T>(IEventArgs args, int code = 0, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return Publish(typeof(T), args, code, priority);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage Publish<T>(T t, IEventArgs args, int code = 0, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return Publish(typeof(T), args, code = 0, priority);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage PublishByNumber<T>(IEventArgs args, int code = 0, int priority = MessageUrgency.Common)
        {
            return PublishByNumber(typeof(T), args, code, priority);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage PublishByNumber<T>(T t, IEventArgs args, int code = 0, int priority = MessageUrgency.Common)
        {
            return PublishByNumber(typeof(T), args, code, priority);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage Publish(Type type, IEventArgs args, int code = 0, MessageUrgencyType priority = MessageUrgencyType.Common)
        {
            return PublishByNumber(type, args, code, (int)priority);
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority">越大处理越晚</param>
        /// <returns></returns>
        public IMessage PublishByNumber(Type type, IEventArgs args, int code = 0, int priority = MessageUrgency.Common)
        {
            return messages.PublishByNumber(type, args, code, priority);
        }
    }

}
