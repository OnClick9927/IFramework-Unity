namespace IFramework.Record
{
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
}