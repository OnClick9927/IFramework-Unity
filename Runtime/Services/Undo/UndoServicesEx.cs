

using System;

namespace IFramework
{
    public static class UndoServicesEx
    {
        public static IServiceCollection UseUndo(this IServiceCollection services, string name = "")
        {
            IUndoService recorder = services.GetService<IUndoService>(name);
            if (recorder != null)
            {
                recorder.Clear();
            }
            else
            {
                recorder = new UndoService();
                services.Use<IUndoService>(recorder, name);
            }

            return services;
        }
        public static IUndoService Undo(this IServiceProvider services, string name = "") => services.GetRequiredService<IUndoService>(name);

        public static void Subscribe<T>(this IUndoService service, Action<T> init, bool redo = true) where T : BaseUndoRecord, new()
        {
            var state = StaticPool.Get<T>();
            init?.Invoke(state);
            service.Subscribe(state, redo);
        }

    }

}
