using System;
using System.Collections.Generic;

namespace IFramework
{
    public interface IRecorderActor
    {
        void Execute();
    }
    public class Recorder
    {
        private class HeadState : BaseState
        {
            protected override void OnRedo() { }
            protected override void OnUndo() { }
            public override object Clone() => null;
        }

        private HeadState _head;
        private BaseState _current;

        public Recorder()
        {
            _head = Allocate<HeadState>();
            _head.SetName("head");
            _current = _head;
        }
        public T Allocate<T>() where T : BaseState, new()
        {
            var state = new T();
            state.recorder = this;
            state.SetName(null);
            return state;
        }

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
        public bool Undo()
        {
            if (_current == _head) return false;
            _current.Undo();
            _current = _current.front;
            return true;
        }
        public bool Redo()
        {
            if (_current.next == null) return false;
            _current = _current.next;
            _current.Redo();
            return true;
        }
        public string GetCurrentRecordName()
        {
            return _current.name;
        }
    }
    public abstract class BaseState : ICloneable
    {
        internal BaseState front;
        internal BaseState next;
        internal Recorder recorder;
        protected Guid _id = Guid.NewGuid();
        public Guid guid => _id; 
        public string name { get; private set; }
        internal void Redo() => OnRedo();
        internal void Undo() => OnUndo();
        protected abstract void OnRedo();
        protected abstract void OnUndo();
        public abstract object Clone();
        public void SetName(string name) => this.name = name;
    }
    public class CommandGroupState : BaseState
    {
        internal CommandGroupState SetValue(IRecorderActor redo, IRecorderActor undo)
        {
            this.redo.Add(redo);
            this.undo.Add(undo);
            return this;
        }
     
        protected override void OnRedo()
        {
            for (int i = 0; i < redo.Count; i++)
            {
                redo[i].Execute();
            }
        }

        protected override void OnUndo()
        {
            for (int i = redo.Count - 1; i >= 0; i--)
            {
                undo[i].Execute();
            }
        }

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
        internal CommandState SetValue(IRecorderActor redo, IRecorderActor undo)
        {
            this.redo = redo;
            this.undo = undo;
            return this;
        }
        protected override void OnRedo() => redo.Execute();
        protected override void OnUndo() => undo.Execute();
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
    public static class OperationRecorderEx
    {
        public static CommandState AllocateCommand(this Recorder t) => t.Allocate<CommandState>();
        public static CommandGroupState AllocateCommandGroup(this Recorder t) => t.Allocate<CommandGroupState>();
        public static T SetCommand<T>(this T t, IRecorderActor redo, IRecorderActor undo) where T : CommandState => t.SetValue(redo, undo) as T;
        public static T SetGroupCommand<T>(this T t, IRecorderActor redo, IRecorderActor undo) where T : CommandGroupState => t.SetValue(redo, undo) as T;
        public static T Subscribe<T>(this T t, bool redo = true) where T : BaseState
        {
            t.recorder.Subscribe(t, redo);
            return t;
        }
    }

}