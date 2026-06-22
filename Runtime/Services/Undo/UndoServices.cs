using System.Collections.Generic;

namespace IFramework
{
    class UndoServices : IFramework.ServiceBase
    {

        protected override void OnUse(IServiceCollection services)
        {
            _head = new HeadState();
            _head.SetName("head");
            _current = _head;
        }
        protected override void OnQuit(IServiceCollection services)
        {
            Clear();
        }

        private class HeadState : BaseUndoRecord
        {
            protected override void OnRedo() { }
            protected override void OnUndo() { }
            //public override object Clone() => null;
        }

        private HeadState _head;
        private BaseUndoRecord _current;

        private void Cycle(BaseUndoRecord record)
        {
            if (record == null) return;
            if (record.next != null)
                Cycle(record.next);
            StaticPool.SetByRealType(record);
        }
        public void Subscribe(BaseUndoRecord state, bool redo = true)
        {
            if (_current == null) _current = _head;
            Cycle(_current.next);

            _current.next = state;


            state.front = _current;
            _current = state;
            if (redo) state.Redo();
        }
        public List<string> GetRecordNames(out int index)
        {
            index = 0;
            List<string> names = new List<string>();
            BaseUndoRecord baseState = _head;
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
        public BaseUndoRecord GetCurrent()
        {
            return _current;
        }
        public void Clear()
        {
            if (_head.next != null)
                Cycle(_head.next);
            _current = _head;
        }
    }

}