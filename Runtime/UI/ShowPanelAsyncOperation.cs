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
    public class PanelAsyncOperation : IPoolObject, IAwaitable<UIAsyncOperationAwaitor>
    {

        public static PanelAsyncOperation Done = new PanelAsyncOperation()
        {
            _isDone = true,
        };


        public string path;

        public Action completed;
        public bool _isDone = false;
        public bool isDone { get { return _isDone; } }
        public void SetComplete()
        {
            _isDone = true;
            completed?.Invoke();
            completed = null;
        }
        bool IPoolObject.valid { get; set; }

        void IPoolObject.OnGet()
        {
            Reset();
        }

        void IPoolObject.OnSet()
        {
        }
        protected virtual void Reset()
        {
            _isDone = false;
            completed = null;
            path = string.Empty;
        }

        public IAwaiter GetAwaiter() => new UIAsyncOperationAwaitor(this);
    }

    struct UIAsyncOperationAwaitor : IAwaiter
    {
        private PanelAsyncOperation op;
        private Queue<Action> actions;
        public UIAsyncOperationAwaitor(PanelAsyncOperation op)
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

        public void OnCompleted(Action continuation)
        {
            actions?.Enqueue(continuation);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            OnCompleted(continuation);
        }

        public void GetResult() { }
    }

    class ShowPanelAsyncOperation : PanelAsyncOperation { }
    class HidePanelAsyncOperation : PanelAsyncOperation
    {

    }
    class ClosePanelAsyncOperation : PanelAsyncOperation
    {

        //public string path;
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
            //path = string.Empty;
            parent = null;
        }
        public string path => show?.path;
        public RectTransform parent;
        internal ShowPanelAsyncOperation show;
    }




}
