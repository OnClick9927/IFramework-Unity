﻿using System;
using System.Collections.Generic;

namespace IFramework.Record
{
    /// <summary>
    /// 操作记录
    /// </summary>
    public class Recorder
    {
        private class HeadState : BaseState
        {
            protected override void OnRedo() { }

            protected override void OnUndo() { }

            protected override void OnReset() { }

            public override object Clone()
            {
                return null;
            }
        }

        private HeadState _head;
        private BaseState _current;

        public Recorder()
        {
            _head = Allocate<HeadState>();
            _head.SetName("head");
            _current = _head;
        }


#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /// <summary>
        /// 分配状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Allocate<T>() where T : BaseState, new()
        {
            var state = new T();
            state.recorder = this;
            state.SetName(null);
            return state;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="state"></param>
        /// <param name="redo"></param>
        public void Subscribe(BaseState state, bool redo = true)
        {
            if (state.recorder == null)
                Log.FE("Don't Create new State ,Use Allocate");
            if (_current == null) _current = _head;
            if (state.guid == _current.guid)
            {
                state = state.Clone() as BaseState;
                state.SetName($"{_current.name}#Clone");
            }
            //if (_current.next != null)
            //{
            //    Recyle(_current.next);
            //}
            _current.next = state;
            state.front = _current;
            _current = state;
            if (redo) state.Redo();
        }
        /// <summary>
        /// 获取记录列表
        /// </summary>
        /// <param name="index">当前记录的位置</param>
        /// <returns></returns>
        public List<string> GetRecordNames(out int index)
        {
            index = 0;
            List<string> names = new List<string>();
            BaseState baseState = _head;
            while (baseState != null)
            {
                if (_current == baseState)
                {
                    index = names.Count;
                }
                names.Add(baseState.name);
                baseState = baseState.next;
            }
            return names;
        }

        public bool CouldUndo()
        {
            if (_current == _head) return false;
            return true;
        }
        public bool CouldRedo()
        {
            if (_current.next == null) return false;
            return true;
        }
        /// <summary>
        /// 撤回
        /// </summary>
        /// <returns></returns>
        public bool Undo()
        {
            if (_current == _head) return false;
            _current.Undo();
            _current = _current.front;
            return true;
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public bool Redo()
        {
            if (_current.next == null) return false;
            _current = _current.next;
            _current.Redo();
            return true;
        }

        /// <summary>
        /// 获取当前节点的名字
        /// </summary>
        /// <returns></returns>
        public string GetCurrentRecordName()
        {
            return _current.name;
        }
    }
}