using System;
using System.Collections.Generic;

namespace IFramework
{
    class LoomModule : UpdateModule
    {
        public LoomModule() { }
        private interface IQueue
        {
            void RunFirst();

        }
        private class MyQueue<T> : IQueue
        {
            public LoomModule loom;
            private Queue<T> queue = new Queue<T>();
            public event Action<T> action;
            public void Enqueue(T t)
            {
                queue.Enqueue(t);
            }
            public void RunFirst()
            {
                T t = queue.Dequeue();
                action?.Invoke(t);
            }
        }
        private Dictionary<Type, IQueue> queues = new Dictionary<Type, IQueue>();

        private Queue<IQueue> _delay = new Queue<IQueue>();



        private MyQueue<T> GetQueue<T>()
        {
            var type = typeof(T);
            if (!queues.ContainsKey(type))
            {
                var queue = new MyQueue<T>();
                queue.loom = this;
                queues.Add(type, queue);
            }
            return queues[type] as MyQueue<T>;
        }


        public void RunDelay<T>(T t)
        {
            lock (_delay)
            {
                MyQueue<T> queue = GetQueue<T>();
                queue.Enqueue(t);
                _delay.Enqueue(queue);
            }
        }
        public void AddDelayHandler<T>(Action<T> action)
        {
            MyQueue<T> queue = GetQueue<T>();
            queue.action += action;
        }
        public void RemoveDelayHandler<T>(Action<T> action)
        {
            MyQueue<T> queue = GetQueue<T>();
            queue.action -= action;
        }
        protected override ModulePriority OnGetDefaultPriority()
        {
            return ModulePriority.Loom;
        }

        protected override void OnUpdate()
        {
            lock (_delay)
            {
                int count = _delay.Count;
                if (count <= 0) return;
                for (int i = 0; i < count; i++)
                {
                    _delay.Dequeue().RunFirst();
                }
            }
        
        }
        protected override void OnDispose()
        {
            _delay.Clear();
            _delay = null;
        }
        protected override void Awake()
        {

        }
    }
}
