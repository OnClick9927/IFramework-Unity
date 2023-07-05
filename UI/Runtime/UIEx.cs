/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using static IFramework.UI.UIModule;

namespace IFramework.UI
{
    public static class UIEx
    {
        public struct UIItemViewAwaiter : IAwaiter<UIItemView>, ICriticalNotifyCompletion 
        {
            private UIItemView task;
            private Queue<Action> calls;
            public UIItemViewAwaiter(UIItemView task)
            {
                if (task == null) throw new ArgumentNullException("task");
                this.task = task;
                calls = new Queue<Action>();
                this.task.completed += Task_completed;
            }

            private void Task_completed()
            {
                while (calls.Count != 0)
                {
                    calls.Dequeue()?.Invoke();
                }
            }

            public bool IsCompleted => task.isDone;

            public UIItemView GetResult()
            {
                if (!IsCompleted)
                    throw new Exception("The task is not finished yet");
                return task;
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

        public struct UIAsyncOperationAwaiter<T> : IAwaiter<T>, ICriticalNotifyCompletion where T : UIAsyncOperation
        {
            private T task;
            private Queue<Action> calls;
            public UIAsyncOperationAwaiter(T task)
            {
                if (task == null) throw new ArgumentNullException("task");
                this.task = task;
                calls = new Queue<Action>();
                this.task.completed += Task_completed;
            }

            private void Task_completed()
            {
                while (calls.Count != 0)
                {
                    calls.Dequeue()?.Invoke();
                }
            }

            public bool IsCompleted => task.isDone;

            public T GetResult()
            {
                if (!IsCompleted)
                    throw new Exception("The task is not finished yet");
                return task;
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
        public static IAwaiter<ShowPanelAsyncOperation> GetAwaiter(this ShowPanelAsyncOperation target)
        {
            return new UIAsyncOperationAwaiter<ShowPanelAsyncOperation>(target);
        }
        public static IAwaiter<LoadItemAsyncOperation> GetAwaiter(this LoadItemAsyncOperation target)
        {
            return new UIAsyncOperationAwaiter<LoadItemAsyncOperation>(target);
        }
        public static IAwaiter<ItemPool> GetAwaiter(this ItemPool target)
        {
            return new UIAsyncOperationAwaiter<ItemPool>(target);
        }
        public static IAwaiter<UIItemOperation> GetAwaiter(this UIItemOperation target)
        {
            return new UIAsyncOperationAwaiter<UIItemOperation>(target);
        }
        public static UIItemViewAwaiter GetAwaiter(this UIItemView target)
        {
            return new UIItemViewAwaiter(target);
        }
    }


}
