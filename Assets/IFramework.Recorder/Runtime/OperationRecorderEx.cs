namespace IFramework
{
    public static class OperationRecorderEx
    {
        public static CommandState AllocateCommand(this Recorder t) => t.Allocate<CommandState>();
        public static CommandGroupState AllocateCommandGroup(this Recorder t) => t.Allocate<CommandGroupState>();
        public static T SetCommand<T>(this T t, IRecorderActor redo, IRecorderActor undo) where T : CommandState => t.SetValue(redo, undo) as T;
        public static T SetGroupCommand<T>(this T t, IRecorderActor redo, IRecorderActor undo) where T : CommandGroupState => t.SetValue(redo, undo) as T;
        public static T Subscribe<T>(this T t, bool redo = true) where T : BaseState
        {
            t.recorder.Subscribe(t, redo);
            return t;
        }
    }

}