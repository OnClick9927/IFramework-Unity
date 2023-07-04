using System;

namespace IFramework.Recorder
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class OperationRecorderEx
    {
        /// <summary>
        /// 分配命令
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static CommandState AllocateCommand(this IOperationRecorderModule t)
        {
            return t.Allocate<CommandState>();
        }
        /// <summary>
        /// 分配回调
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ActionState AllocateAction(this IOperationRecorderModule t)
        {
            return t.Allocate<ActionState>();
        }
        /// <summary>
        /// 分配命令组
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static CommandGroupState AllocateCommandGroup(this IOperationRecorderModule t)
        {
            return t.Allocate<CommandGroupState>();
        }
        /// <summary>
        /// 分配回调组
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ActionGroupState AllocateActionGroup(this IOperationRecorderModule t)
        {
            return t.Allocate<ActionGroupState>();
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <returns></returns>
        public static T SetCommand<T>(this T t, ICommand redo, ICommand undo) where T : CommandState
        {
            t.SetValue(redo, undo);
            return t;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <returns></returns>
        public static T SetGroupCommand<T>(this T t, ICommand redo, ICommand undo) where T : CommandGroupState
        {
            t.SetValue(redo, undo);
            return t;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <returns></returns>
        public static T SetCommand<T>(this T t, Action redo, Action undo) where T : ActionState
        {
            t.SetValue(redo, undo);
            return t;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <returns></returns>
        public static T SetGroupCommand<T>(this T t, Action redo, Action undo) where T : ActionGroupState
        {
            t.SetValue(redo, undo);
            return t;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <returns></returns>
        public static T Subscribe<T>(this T t, bool redo = true) where T : BaseState
        {
            t.recorder.Subscribe(t, redo);
            return t;
        }
    }
}