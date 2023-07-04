using System;
namespace IFramework.Message
{
    class TypeMessageQueue : MessageQueue<Type>
    {
        public TypeMessageQueue(HandlerQueue<Type> handler) : base(handler)
        {
        }

        protected override void SetMessage(Message message, Type type)
        {
            message.SetType(type);
        }
    }
}
