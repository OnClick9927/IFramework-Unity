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
    struct UIAsyncOperationAwaitor : IAwaiter<ShowPanelAsyncOperation>
    {
        private ShowPanelAsyncOperation op;
        private Queue<Action> actions;
        public UIAsyncOperationAwaitor(ShowPanelAsyncOperation op)
        {
            this.op = op;
            actions = StaticPool<Queue<Action>>.Get();
            op.completed += OnCompleted;
        }

        private void OnCompleted()
        {
            while (actions.Count > 0)
            {
                actions.Dequeue()?.Invoke();
            }
            StaticPool<Queue<Action>>.Set(actions);
        }

        public bool IsCompleted => op.isDone;

        public ShowPanelAsyncOperation GetResult() => op;
        public void OnCompleted(Action continuation)
        {
            actions?.Enqueue(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            OnCompleted(continuation);
        }


    }
    public class ShowPanelAsyncOperation : IAwaitable<UIAsyncOperationAwaitor, ShowPanelAsyncOperation>, IPoolObject
    {
        public Action completed;
        public bool _isDone = false;

        public bool isDone { get { return _isDone; } }

        bool IPoolObject.valid { get; set; }

        internal void Reset()
        {
            _isDone = false;
            completed = null;

        }
        public void SetComplete()
        {
            _isDone = true;
            completed?.Invoke();
            completed = null;
        }

        public IAwaiter<ShowPanelAsyncOperation> GetAwaiter()
        {
            return new UIAsyncOperationAwaitor(this);
        }

        void IPoolObject.OnGet()
        {
            Reset();
        }

        void IPoolObject.OnSet()
        {
        }
    }

    public class LoadPanelAsyncOperation : IPoolObject
    {
        internal UIPanel value;
        private bool _isDone;

        public bool isDone { get { return _isDone; } }

        bool IPoolObject.valid { get; set; }

        public void SetValue(UIPanel value)
        {
            this.value = value;
            _isDone = true;
        }

        void IPoolObject.OnGet()
        {
            Reset();
        }

        void IPoolObject.OnSet()
        {
        }

        internal void Reset()
        {
            _isDone = false;
            value = null;
            path = string.Empty;
            parent = null;
        }
        public string path;
        public RectTransform parent;
        internal ShowPanelAsyncOperation show;
    }
}
