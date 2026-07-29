using System;

namespace IFramework
{
    public class UndoRecord : BaseUndoRecord
    {
        protected override void OnReset()
        {
            base.OnReset();
            redo = null;
            undo = null;
        }
        public UndoRecord SetValue(Action redo, Action undo)
        {
            this.redo = redo;
            this.undo = undo;
            return this;
        }
        protected override void OnRedo() => redo.Invoke();
        protected override void OnUndo() => undo.Invoke();
        private Action redo;
        private Action undo;
    }

}
