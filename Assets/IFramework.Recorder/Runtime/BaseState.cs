using System;

namespace IFramework
{
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

}