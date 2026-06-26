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
        public static IServiceCollection UseState(this IServiceCollection services, IReadOnlyList<IGameState> states)
        {

            var service = new GameStateService(states);
            services.Use<IGameStateService>(service);
            var values = service.Values();
            if (states == null) return services;
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                values.Register(state.GetType(), state, string.Empty);
            }
            return services;
        }

        public static IGameStateService State(this IServiceProvider services) => services.GetRequiredService<IGameStateService>();

        public static IServiceProvider EnterState<T>(this IServiceProvider services) where T : IGameState
        {
            services.EnterService<IGameStateService>();
            SwitchState<T>(services.State());
            return services;
        }



        public static bool SwitchState(this IGameStateService service, Type type)
        {
            var _state = FindState(service, type);
            if (_state == null) return false;
            service.state = _state;
            return true;
        }
        public static bool SwitchState<T>(this IGameStateService service) => SwitchState(service, typeof(T));
        public static bool SwitchState(this IGameStateService service, IGameState state) => SwitchState(service, state.GetType());
        public static IGameState FindState(this IGameStateService service, Type type) => service.Values().Get(type, string.Empty) as IGameState;
        public static IGameState FindState<T>(this IGameStateService service) where T : IGameState => FindState(service, typeof(T));
        public static IGameState GetCurrentState(this IGameStateService service) => service?.state;

    }
}
