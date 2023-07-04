using System;
using System.Collections.Generic;
namespace IFramework.Message
{
    abstract class HandlerQueue<T> : IDisposable
    {
        private Queue<SubscribeAction<T>> _subscribeQueue = new Queue<SubscribeAction<T>>();
        protected List<Subject<T>> subjects = new List<Subject<T>>();
        protected Dictionary<T, int> subjectMap = new Dictionary<T, int>();
        private object _lock = new object();


        private void CreateSubscribeAction(T type, MessageListener listener, bool subscribe)
        {
            lock (_lock)
            {
                _subscribeQueue.Enqueue(new SubscribeAction<T>(type, listener, subscribe));
            }
        }
        public void Subscribe(T type, MessageListener listener)
        {
            CreateSubscribeAction(type, listener, true);
        }
        public void UnSubscribe(T type, MessageListener listener)
        {
            CreateSubscribeAction(type, listener, false);
        }
        public abstract bool Publish(IMessage message);
        public void Update()
        {
            int count = 0;

            lock (_lock)
            {
                int index = 0;
                T type = default;
                count = _subscribeQueue.Count;
                for (int i = 0; i < count; i++)
                {
                    var value = _subscribeQueue.Dequeue();
                    type = value.type;
                    if (!value.subscribe)
                    {
                        if (subjectMap.TryGetValue(type, out index))
                        {
                            subjects[subjectMap[type]].UnSubscribe(value.value);
                        }
                    }
                    else
                    {
                        Subject<T> en;
                        if (!subjectMap.TryGetValue(type, out index))
                        {
                            index = subjects.Count;
                            en = new Subject<T>(type);
                            subjectMap.Add(type, index);
                            subjects.Add(en);
                        }
                        else
                        {
                            en = subjects[subjectMap[type]];
                        }
                        en.Subscribe(value.value);
                    }
                }
            }

        }

        public void Dispose()
        {
            for (int i = 0; i < subjects.Count; i++) subjects[i].Dispose();
            subjects.Clear();
            subjectMap.Clear();
            _subscribeQueue.Clear();
            _subscribeQueue = null;
            subjects = null;
            subjectMap = null;
        }
    }
}
