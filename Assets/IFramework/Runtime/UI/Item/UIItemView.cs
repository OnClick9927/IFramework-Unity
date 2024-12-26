/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2020.3.3f1c1
 *Date:           2022-08-03
 *Description:    Description
 *History:        2022-08-03--
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace IFramework.UI
{
    public abstract class UIItemView : GameObjectView
    {
        public Action completed;
        public bool _isDone = false;
        public string path { get; private set; }

        public bool isDone { get { return _isDone; } }

        private void Compelete()
        {
            _isDone = true;
            completed?.Invoke();
            completed = null;
        }
        internal async void Load(string path, UIItemOperation op, Transform parent)
        {
            await op;
            var go = op.gameObject;
            if (parent != null)
            {
                go.transform.SetParent(parent.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
            }
            this.path = path;
            this.SetGameObject(go);
            //go.SetActive(true);
            this.OnGet();
            Compelete();
        }
        public virtual void OnSet()
        {
            this.DisposeEvents();
            this.DisposeUIEvents();

        }

        protected abstract void OnGet();

    }


    public static class UIItemAsync
    {
        struct UIItemViewAwaiter<T> : IAwaiter<T>, ICriticalNotifyCompletion where T : UIItemView
        {
            private T task;
            private Queue<Action> calls;
            public UIItemViewAwaiter(T task)
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

        public static IAwaiter<T> GetAwaiter<T>(this T target) where T : UIItemView
        {
            return new UIItemViewAwaiter<T>(target);
        }
    }
}
