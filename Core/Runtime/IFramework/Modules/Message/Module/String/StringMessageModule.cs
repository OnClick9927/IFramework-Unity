namespace IFramework.Message
{
    /// <summary>
    /// 消息模块（String版本）
    /// </summary>
    public class StringMessageModule : UpdateModule, IStringMessageModule
    {
        private StringMessageQueue messages;
        private StringHandlerQueue handlers;
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
            handlers = new StringHandlerQueue();
            messages = new StringMessageQueue(handlers);
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
        /// 每帧处理消息个数
        /// </summary>
        public int processesPerFrame { get { return messages.processesPerFrame; } set { messages.processesPerFrame = value; } }




        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void Subscribe(string type, IMessageListener listener)
        {
            Subscribe(type, listener.Listen);
        }

        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void UnSubscribe(string type, IMessageListener listener)
        {
            UnSubscribe(type, listener.Listen);
        }

        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void Subscribe(string type, MessageListener listener)
        {
            handlers.Subscribe(type, listener);
        }

        /// <summary>
        /// 解除注册监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public void UnSubscribe(string type, MessageListener listener)
        {
            handlers.UnSubscribe(type, listener);
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="code"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public IMessage Publish(string type, IEventArgs args, int code = 0, MessageUrgencyType priority = MessageUrgencyType.Common)
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
        public IMessage PublishByNumber(string type, IEventArgs args, int code = 0, int priority = MessageUrgency.Common)
        {
            return messages.PublishByNumber(type, args, code, priority);
        }
    }

}
