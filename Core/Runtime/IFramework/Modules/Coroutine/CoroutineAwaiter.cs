using System;
using System.Collections.Generic;

namespace IFramework.Coroutine
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public struct CoroutineAwaiter : IAwaiter
    {
        private Coroutine cor;
        private Queue<Action> calls;

        public CoroutineAwaiter(ICoroutine cor)
        {
            this.cor = cor as Coroutine;
            calls = new Queue<Action>();
            this.cor.onCompelete += Task_completed;
        }

        private void Task_completed()
        {
            while (calls.Count != 0)
            {
                calls.Dequeue()?.Invoke();
            }
        }

        public bool IsCompleted { get { return cor.isDone; } }

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
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
