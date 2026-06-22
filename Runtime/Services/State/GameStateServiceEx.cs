/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework
{
    public static class GameStateServiceEx
    {
        public static IServiceCollection UseState(this IServiceCollection services, IReadOnlyList<IGameState> states, IGameState first)
        {

            var service = new GameStateService();

            services.UseService(service);
            if (states == null) return services;
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                services.RegisterValue(state.GetType(), state, string.Empty);
            }
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                services.InjectFields(state);
                state.Init();
            }
            if (first != null)
                SwitchState(services, first);
            return services;
        }
        public static bool SwitchState(this IServiceCollection services, Type type)
        {
            var _state = FindState(services, type);
            if (_state == null) return false;
            var service = services.GetService<GameStateService>();
            service.state = _state;
            return true;
        }
        public static bool SwitchState<T>(this IServiceCollection services) => SwitchState(services, typeof(T));
        public static bool SwitchState(this IServiceCollection services, IGameState state) => SwitchState(services, state.GetType());
        public static IGameState FindState(this IServiceCollection services, Type type) => services.GetValue(type, string.Empty) as IGameState;
        public static IGameState FindState<T>(this IServiceCollection services) where T : IGameState => FindState(services, typeof(T));
        public static IGameState GetCurrentState(this IServiceCollection services)
        {
            var service = services.GetService<GameStateService>();
            return service?.state;
        }
        public static void ListenStateChange(this IServiceCollection services, GameStateChange call)
        {
            var service = services.GetService<GameStateService>();
            if (service != null)
            {
                service.OnGameStateChange += call;
            }
        }

    }
}
