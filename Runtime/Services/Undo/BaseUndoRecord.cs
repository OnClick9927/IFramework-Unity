namespace IFramework
{
    public abstract class BaseUndoRecord : IPoolObject
    {
        internal BaseUndoRecord front;
        internal BaseUndoRecord next;
        public string name { get; private set; }
        bool IPoolObject.valid { get; set; }

        internal void Redo() => OnRedo();
        internal void Undo() => OnUndo();
        protected abstract void OnRedo();
        protected abstract void OnUndo();
        public void SetName(string name) => this.name = name;
        protected virtual void OnReset()
        {
            front = null;
            next = null;
            SetName(string.Empty);

        }
        void IPoolObject.OnGet()
        {
            OnReset();
        }

        void IPoolObject.OnSet()
        {
            //OnReset();
        }
    }

}