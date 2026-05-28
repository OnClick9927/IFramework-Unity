using System.Collections.Generic;

namespace IFramework
{
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

}