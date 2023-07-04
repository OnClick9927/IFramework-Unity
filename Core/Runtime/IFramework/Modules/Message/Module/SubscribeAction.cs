using System;
namespace IFramework.Message
{
    struct SubscribeAction<T>
    {
        public readonly bool subscribe;
        public readonly T type;
        public readonly MessageListener value;

        public SubscribeAction(T type, MessageListener value, bool subscribe)
        {
            this.subscribe = subscribe;
            this.type = type;
            this.value = value;
        }

    }
}
