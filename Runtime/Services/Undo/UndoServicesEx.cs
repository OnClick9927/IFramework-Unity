using System;
using System.Collections.Generic;

namespace IFramework
{
    public static class UndoServicesEx
    {
        public static IServiceCollection UseUndo(this IServiceCollection services, string name = "")
        {
            UndoServices recorder = services.GetService<UndoServices>(name);
            if (recorder != null)
            {
                recorder.Clear();
            }
            else
            {
                recorder = new UndoServices();
                services.UseService(recorder, name);
            }

            return services;
        }
        public static void ClearUndo(this IServiceCollection services, string name = "")
        {
            UndoServices recorder = services.GetService<UndoServices>(name);
            recorder.Clear();
        }

        public static T SubscribeUndo<T>(this IServiceCollection services, Action<T> init, bool redo = true, string name = "") where T : BaseUndoRecord, new()
        {
            UndoServices recorder = services.GetService<UndoServices>(name);
            T result = StaticPool.Get<T>();
            init?.Invoke(result);
            recorder.Subscribe(result, redo);
            return result;
        }
        public static IReadOnlyList<string> GetUndoNames(this IServiceCollection services, out int index, string name = "")
        {
            UndoServices recorder = services.GetService<UndoServices>(name);
            return recorder.GetRecordNames(out index);
        }

        public static bool CouldUndo(this IServiceCollection services, string name = "")
        {
            UndoServices recorder = services.GetService<UndoServices>(name);
            return recorder.CouldUndo();
        }
        public static bool CouldRedo(this IServiceCollection services, string name = "")
        {
            UndoServices recorder = services.GetService<UndoServices>(name);
            return recorder.CouldRedo();
        }
        public static bool DoUndo(this IServiceCollection services, string name = "")
        {
            UndoServices recorder = services.GetService<UndoServices>(name);
            return recorder.Undo();
        }
        public static bool DoRedo(this IServiceCollection services, string name = "")
        {
            UndoServices recorder = services.GetService<UndoServices>(name);
            return recorder.Redo();
        }
        public static BaseUndoRecord GetCurrentUndoState(this IServiceCollection services, string name = "")
        {
            UndoServices recorder = services.GetService<UndoServices>(name);
            return recorder.GetCurrent();
        }
    }

}