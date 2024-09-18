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
}