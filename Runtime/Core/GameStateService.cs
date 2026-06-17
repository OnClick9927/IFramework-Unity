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
    public delegate void GameStateChange(IGameState exit, IGameState enter);
    public static class GameStateServiceEx
    {
        public static Game UseState(this Game game, IReadOnlyList<IGameState> states, IGameState first)
        {

            var service = new GameStateService();

            game.UseService(service);
            if (states == null) return game;
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                game.RegisterValue(state.GetType(), state);
            }
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                game.InjectValues(state);
                state.Init();
            }
            if (first != null)
                SwitchState(game, first);
            return game;
        }
        public static bool SwitchState(this Game game, Type type)
        {
            var _state = FindState(game, type);
            if (_state == null) return false;
            var service = game.GetService<GameStateService>();
            service.state = _state;
            return true;
        }
        public static bool SwitchState<T>(this Game game) => SwitchState(game, typeof(T));
        public static bool SwitchState(this Game game, IGameState state) => SwitchState(game, state.GetType());
        public static IGameState FindState(this Game game, Type type) => game.GetValue(type) as IGameState;
        public static IGameState FindState<T>(this Game game) where T : IGameState => FindState(game, typeof(T));

        public static IGameState GetCurrentState(this Game game)
        {
            var service = game.GetService<GameStateService>();
            return service?.state;
        }

        public static void ListenStateChange(this Game game, GameStateChange call)
        {
            var service = game.GetService<GameStateService>();
            if (service != null)
            {
                service.OnGameStateChange += call;
            }
        }

    }
    public interface IGameState : IInjectAble, IEventsOwner
    {
        void OnExit(IGameState enter);
        void OnEnter(IGameState exit);
        void Update();
        void Init();
    }
    class GameStateService : GameServiceBase
    {
        private void Update()
        {
            if (state == null) return;
            state.Update();
        }

        protected override void OnUse(Game game)
        {
            Game.BindUpdate(Update);
        }

        protected override void OnQuit(Game game)
        {
            Game.UnBindUpdate(Update);
            state = null;
            OnGameStateChange = null;
        }

        private IGameState _state;
        public IGameState state
        {
            get => _state; set
            {
                if (value == _state) return;
                _state?.DisposeEvents();
                _state?.OnExit(value);
                var exit = _state;
                _state = value;
                _state?.OnEnter(exit);
                OnGameStateChange?.Invoke(exit, value);
            }
        }
        public event GameStateChange OnGameStateChange;
    }
}
