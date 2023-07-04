namespace IFramework.Message
{
    class StringHandlerQueue : HandlerQueue<string>
    {
        public override bool Publish(IMessage message)
        {
            bool success = false;
            var type = message.stringSubject;
            if (subjectMap.ContainsKey(type))
            {
                success |= subjects[subjectMap[type]].Publish(message);
            }
            return success;
        }
    }
}
