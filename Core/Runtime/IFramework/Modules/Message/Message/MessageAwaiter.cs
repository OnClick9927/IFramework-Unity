using System;
using System.Collections.Generic;

namespace IFramework.Message
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public struct MessageAwaiter : IAwaiter
    {
        private IMessage cor;
        private Queue<Action> calls;

        public MessageAwaiter(IMessage cor)
        {
            this.cor = cor;
            calls = new Queue<Action>();
            this.cor.OnCompelete(Task_completed);
        }

        private void Task_completed(IMessage obj)
        {
            while (calls.Count != 0)
            {
                calls.Dequeue()?.Invoke();
            }
        }

        public bool IsCompleted { get { return cor.state == MessageState.Rest; } }

        public void GetResult()
        {
            if (!IsCompleted)
                throw new Exception("The task is not finished yet");
        }

        public void OnCompleted(Action continuation)
        {
            UnsafeOnCompleted(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");
            calls.Enqueue(continuation);
        }
    }
}
