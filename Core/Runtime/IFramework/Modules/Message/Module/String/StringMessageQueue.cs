namespace IFramework.Message
{
    class StringMessageQueue : MessageQueue<string>
    {
        public StringMessageQueue(HandlerQueue<string> handler) : base(handler)
        {
        }

        protected override void SetMessage(Message message, string type)
        {
            message.SetType(type);
        }
    }
}
