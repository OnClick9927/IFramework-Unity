/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.UI
{
    struct UIAsyncOperationAwaitor : IAwaiter<UIAsyncOperation>
    {
        private UIAsyncOperation op;
        private Queue<Action> actions;
        public UIAsyncOperationAwaitor(UIAsyncOperation op)
        {
            this.op = op;
            actions = new Queue<Action>();
            op.completed += OnCompleted;
        }

        private void OnCompleted()
        {
            while (actions.Count > 0)
            {
                actions.Dequeue()?.Invoke();
            }
        }

        public bool IsCompleted => op.isDone;

        public UIAsyncOperation GetResult() => op;
        public void OnCompleted(Action continuation)
        {
            actions?.Enqueue(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            OnCompleted(continuation);
        }


    }
    public class UIAsyncOperation : IAwaitable<UIAsyncOperationAwaitor, UIAsyncOperation>
    {
        public Action completed;
        public bool _isDone = false;

        public bool isDone { get { return _isDone; } }

        public bool IsCompleted => _isDone;


        public void SetComplete()
        {
            _isDone = true;
            completed?.Invoke();
            completed = null;
        }

        public IAwaiter<UIAsyncOperation> GetAwaiter()
        {
            return new UIAsyncOperationAwaitor(this);
        }


    }
    public class UIAsyncOperation<T> : UIAsyncOperation
    {
        public T value;

        public void SetValue(T value)
        {
            this.value = value;
            base.SetComplete();
        }
    }
    public class ShowPanelAsyncOperation : UIAsyncOperation { }
    public class LoadPanelAsyncOperation : UIAsyncOperation<UIPanel>
    {
        public string path;
        public RectTransform parent;
        internal ShowPanelAsyncOperation show;
    }
    public class LoadItemAsyncOperation : UIAsyncOperation<GameObject>
    {
        public string path;
    }
}
