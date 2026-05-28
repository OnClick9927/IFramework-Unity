namespace IFramework
{
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

}