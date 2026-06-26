/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;

namespace IFramework
{
    public delegate void GameStateChange(IGameState exit, IGameState enter);
    class GameStateService : ServiceBase, IGameStateService
    {
        private void Update()
        {
            if (state == null) return;
            state.Update();
        }

        protected override void OnUse(IServiceCollection services)
        {
            Game.BindUpdate(Update);
        }


        protected override void OnEnter(IServiceCollection services)
        {
            var values = this.Values();
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                values.Inject(state);
                state.Init();
            }
        }
        protected override void OnQuit(IServiceCollection services)
        {
            Game.UnBindUpdate(Update);
            state = null;
            OnGameStateChange = null;
        }
        public void RemoveListenStateChange(GameStateChange call)
        {
            OnGameStateChange -= call;
        }
        public void ListenStateChange(GameStateChange call)
        {
            OnGameStateChange += call;
        }

        private IGameState _state;
        public IReadOnlyList<IGameState> states { get; private set; }

        public GameStateService(IReadOnlyList<IGameState> states)
        {
            this.states = states;
        }

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
