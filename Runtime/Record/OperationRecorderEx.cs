namespace IFramework.Record
{
    public static class OperationRecorderEx
    {
        /// <summary>
        /// 分配命令
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static CommandState AllocateCommand(this Recorder t)
        {
            return t.Allocate<CommandState>();
        }

        /// <summary>
        /// 分配命令组
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static CommandGroupState AllocateCommandGroup(this Recorder t)
        {
            return t.Allocate<CommandGroupState>();
        }


        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="redo"></param>
        /// <param name="undo"></param>
        /// <returns></returns>
        public static T SetCommand<T>(this T t, IRecorderActor redo, IRecorderActor undo) where T : CommandState
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
        public static T SetGroupCommand<T>(this T t, IRecorderActor redo, IRecorderActor undo) where T : CommandGroupState
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