using IFramework.Queue;
using System;
using System.Collections.Generic;
namespace IFramework.Message
{


    abstract class MessageQueue<T> : IDisposable
    {
        public int count
        {
            get
            {
                lock (_lock)
                {
                    return _priorityQueue.count;
                }
            }
        }

        protected readonly HandlerQueue<T> handler;
        private int _processesPerFrame = -1;
        private object _lock = new object();


        private StablePriorityQueue<StablePriorityQueueNode> _priorityQueue;
        private List<StablePriorityQueueNode> _updatelist;
        private Dictionary<StablePriorityQueueNode, Message> excuteMap;
        public int processesPerFrame { get { return _processesPerFrame; } set { _processesPerFrame = value; } }

        public MessageQueue(HandlerQueue<T> handler)
        {
            excuteMap = new Dictionary<StablePriorityQueueNode, Message>();
            _priorityQueue = new StablePriorityQueue<StablePriorityQueueNode>();
            _updatelist = new List<StablePriorityQueueNode>();
            this.handler = handler;
        }
        protected abstract void SetMessage(Message message, T type);
        public IMessage PublishByNumber(T type, IEventArgs args, int code, int priority = MessageUrgency.Common)
        {
            var message = new Message();
            message.Begin();
            message.SetCode(code);
            message.SetArgs(args);
            SetMessage(message, type);
            if (priority < 0)
            {
                HandleMessage(message);
            }
            else
            {
                lock (_lock)
                {
                    if (_priorityQueue.count == _priorityQueue.capcity)
                    {
                        _priorityQueue.Resize(_priorityQueue.capcity * 2);
                    }
                    StablePriorityQueueNode node = new StablePriorityQueueNode();
                    _priorityQueue.Enqueue(node, priority);
                    excuteMap.Add(node, message);
                    _updatelist.Add(node);
                }
            }
            return message;
        }

        private void HandleMessage(Message message)
        {
            message.Lock();
            bool sucess = false;
            sucess |= handler.Publish(message);
            message.SetErrorCode(sucess ? MessageErrorCode.Success : MessageErrorCode.NoneListen);
            message.End();
        }
        Queue<Message> _tmp = new Queue<Message>();
        public void Update()
        {
            int count = 0;
            lock (_lock)
            {
                count = processesPerFrame == -1 ? _priorityQueue.count : Math.Min(processesPerFrame, _priorityQueue.count);
                if (count == 0) return;
                for (int i = 0; i < count; i++)
                {
                    StablePriorityQueueNode node = _priorityQueue.Dequeue();
                    Message message;
                    if (excuteMap.TryGetValue(node, out message))
                    {
                        _tmp.Enqueue(message);
                        excuteMap.Remove(node);
                    }
                    _updatelist.Remove(node);
                }
                if (_updatelist.Count > 0)
                {
                    for (int i = _updatelist.Count - 1; i >= 0; i--)
                    {
                        _priorityQueue.UpdatePriority(_updatelist[i], _updatelist[i].priority - 1);
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                var message = _tmp.Dequeue();
                HandleMessage(message);
            }
        }


        public void Dispose()
        {
  
            _updatelist.Clear();
            excuteMap.Clear();
            _priorityQueue = null;
            _updatelist = null;
            excuteMap = null;
        }
    }
}
