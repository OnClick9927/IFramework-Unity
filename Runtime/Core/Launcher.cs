/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.115
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;

namespace IFramework
{
    [AddComponentMenu("")]
    [DynamicMonoSingleton]
    class Launcher : MonoSingleton<Launcher>
    {
        private Game _game;
        public Game game
        {
            get { return _game; }
            set
            {
                if (_game == value) return;
                if(_game)
                    Destroy(_game.gameObject);
                _game = value;
            }
        }
        private static event Action onFixUpdate;
        private static event Action onUpdate;
        private static event Action onLateUpdate;
        private static event Action<bool> onApplicationFocus;
        private static event Action<bool> onApplicationPause;
        private static event Action ondisable;
        private void OnDisable() => ondisable?.Invoke();
        private void Update() => onUpdate?.Invoke();
        private void FixedUpdate() => onFixUpdate?.Invoke();
        private void LateUpdate() => onLateUpdate?.Invoke();
        private void OnApplicationFocus(bool focus) => onApplicationFocus?.Invoke(focus);
        private void OnApplicationPause(bool pause) => onApplicationPause?.Invoke(pause);

       
        public static void BindUpdate(Action action) => onUpdate += action;
        public static void UnBindUpdate(Action action) => onUpdate -= action;
        public static void BindFixedUpdate(Action action) => onFixUpdate += action;
        public static void UnBindFixedUpdate(Action action) => onFixUpdate -= action;

        public static void BindLateUpdate(Action action) => onLateUpdate += action;
        public static void UnBindLateUpdate(Action action) => onLateUpdate -= action;
        public static void BindOnApplicationFocus(Action<bool> action) => onApplicationFocus += action;
        public static void UnBindOnApplicationFocus(Action<bool> action) => onApplicationFocus -= action;
        public static void BindOnApplicationPause(Action<bool> action) => onApplicationPause += action;
        public static void UnBindOnApplicationPause(Action<bool> action) => onApplicationPause -= action;
        public static void BindDisable(Action action) => ondisable += action;
        public static void UnBindDisable(Action action) => ondisable -= action;
    }
}
