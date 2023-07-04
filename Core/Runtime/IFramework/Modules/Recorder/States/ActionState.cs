using System;

namespace IFramework.Recorder
{
    /// <summary>
    /// 回调状态
    /// </summary>
    public class ActionState : BaseState
    {
        internal void SetValue(Action redo, Action undo)
        {
            this.redo = redo;
            this.undo = undo;
        }
        /// <summary>
        /// 执行
        /// </summary>
        protected override void OnRedo()
        {
            redo();
        }
        /// <summary>
        /// 撤回
        /// </summary>
        protected override void OnUndo()
        {
            undo();
        }
        /// <summary>
        /// 重置数据
        /// </summary>
        protected override void OnReset()
        {
            redo = null;
            undo = null;
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new ActionState()
            {
                recorder = recorder,
                redo = redo,
                undo = undo,
                _id = _id
            };
        }

        private Action redo;
        private Action undo;
    }
}
