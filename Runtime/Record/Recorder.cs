using System;
using System.Collections.Generic;

namespace IFramework.Record
{
    public class CommandGroupState : BaseState
    {
        internal void SetValue(IRecorderActor redo, IRecorderActor undo)
        {
            this.redo.Add(redo);
            this.undo.Add(undo);
        }
        /// <summary>
        /// 执行
        /// </summary>
        protected override void OnRedo()
        {
            for (int i = 0; i < redo.Count; i++)
            {
                redo[i].Execute();
            }
        }
        /// <summary>
        /// 撤回
        /// </summary>
        protected override void OnUndo()
        {
            for (int i = redo.Count - 1; i >= 0; i--)
            {
                undo[i].Execute();
            }
        }
        /// <summary>
        /// 重置数据
        /// </summary>
        protected override void OnReset()
        {
            redo.Clear();
            undo.Clear();
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new CommandGroupState()
            {
                recorder = recorder,
                redo = new List<IRecorderActor>(redo),
                undo = new List<IRecorderActor>(undo),
                _id = _id
            };
        }

        private List<IRecorderActor> redo = new List<IRecorderActor>();
        private List<IRecorderActor> undo = new List<IRecorderActor>();
    }

    public class CommandState : BaseState
    {
        internal void SetValue(IRecorderActor redo, IRecorderActor undo)
        {
            this.redo = redo;
            this.undo = undo;
        }
        /// <summary>
        /// 执行
        /// </summary>
        protected override void OnRedo()
        {
            redo.Execute();
        }
        /// <summary>
        /// 撤回
        /// </summary>
        protected override void OnUndo()
        {
            undo.Execute();
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        protected override void OnReset()
        {
            redo = null;
            undo = null;
        }
        //复制
        public override object Clone()
        {
            return new CommandState()
            {
                recorder = recorder,
                redo = redo,
                undo = undo,
                _id = _id
            };
        }

        private IRecorderActor redo;
        private IRecorderActor undo;
    }

    public abstract class BaseState : ICloneable
    {
        internal BaseState front;
        internal BaseState next;
        internal Recorder recorder;
        /// <summary>
        /// id
        /// </summary>
        protected Guid _id = Guid.NewGuid();

        /// <summary>
        /// id
        /// </summary>
        public Guid guid { get { return _id; } }
        /// <summary>
        /// 名字
        /// </summary>
        public string name { get; private set; }
        internal void Redo() { OnRedo(); }
        internal void Undo() { OnUndo(); }
        internal virtual void Reset()
        {
            front = null;
            next = null;
            recorder = null;
            OnReset();
        }


        /// <summary>
        /// 执行
        /// </summary>
        protected abstract void OnRedo();
        /// <summary>
        /// 撤回
        /// </summary>
        protected abstract void OnUndo();
        /// <summary>
        /// 重置数据
        /// </summary>
        protected abstract void OnReset();
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

        /// <summary>
        /// 设置名字
        /// </summary>
        /// <param name="name">名字</param>
        public void SetName(string name)
        {
            this.name = name;
        }
    }



    public static class OperationRecorderEx
    {
        /// <summary>
        /// 分配命令
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static CommandState AllocateCommand(this Recorder t)
        {
            return t.Allocate<CommandState>();
        }

        /// <summary>
        /// 分配命令组
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static CommandGroupState AllocateCommandGroup(this Recorder t)
        {
            return t.Allocate<CommandGroupState>();
        }


        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <returns></returns>
        public static T SetCommand<T>(this T t, IRecorderActor redo, IRecorderActor undo) where T : CommandState
        {
            t.SetValue(redo, undo);
            return t;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <returns></returns>
        public static T SetGroupCommand<T>(this T t, IRecorderActor redo, IRecorderActor undo) where T : CommandGroupState
        {
            t.SetValue(redo, undo);
            return t;
        }


        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <returns></returns>
        public static T Subscribe<T>(this T t, bool redo = true) where T : BaseState
        {
            t.recorder.Subscribe(t, redo);
            return t;
        }
    }

    public interface IRecorderActor
    {
        /// <summary>
        /// 处理
        /// </summary>
        void Execute();
    }
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
                Log.E("Don't Create new State ,Use Allocate");
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