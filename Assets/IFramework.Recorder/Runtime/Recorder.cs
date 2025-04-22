using System;
using System.Collections.Generic;

namespace IFramework
{
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

}