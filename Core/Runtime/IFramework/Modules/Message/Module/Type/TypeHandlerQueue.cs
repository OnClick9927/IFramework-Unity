using System;
using System.Linq;

namespace IFramework.Message
{
    class TypeHandlerQueue : HandlerQueue<Type>
    {
        private bool _fitSubType = false;

        public bool fitSubType { get { return _fitSubType; } set { _fitSubType = value; } }

        public override bool Publish(IMessage message)
        {
            bool success = false;
            var type = message.subject;
            if (fitSubType)
            {
                foreach (var _listenType in subjectMap.Keys)
                {
                    
                    if (type.GetInterfaces().Contains(_listenType) || type.IsSubclassOf(_listenType) || type == _listenType)
                    {
                        success |= subjects[subjectMap[_listenType]].Publish(message);
                    }
                }
            }
            else
            {
                if (subjectMap.ContainsKey(type))
                {
                    success |= subjects[subjectMap[type]].Publish(message);
                }
            }
            return success;
        }
    }
}
