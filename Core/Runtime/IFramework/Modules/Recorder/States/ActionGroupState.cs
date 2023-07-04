using System;
using System.Collections.Generic;

namespace IFramework.Recorder
{
    /// <summary>
    /// 回调组
    /// </summary>
    public class ActionGroupState : BaseState
    {
        internal void SetValue(Action redo, Action undo)
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
                redo[i]();
            }
        }
        /// <summary>
        /// 撤回
        /// </summary>
        protected override void OnUndo()
        {
            for (int i = redo.Count - 1; i >= 0; i--)
            {
                undo[i]();
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
        /// 赋值
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new ActionGroupState()
            {
                recorder = recorder,
                redo = new List<Action>(redo),
                undo = new List<Action>(undo),
                _id = _id
            };
        }

        private List<Action> redo = new List<Action>();
        private List<Action> undo = new List<Action>();
    }
}
