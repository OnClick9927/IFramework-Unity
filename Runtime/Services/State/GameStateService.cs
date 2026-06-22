/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework
{
    public delegate void GameStateChange(IGameState exit, IGameState enter);
    class GameStateService : ServiceBase
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

        protected override void OnQuit(IServiceCollection services)
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
