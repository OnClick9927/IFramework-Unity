/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.115
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Singleton;
using System;
using UnityEngine;

namespace IFramework
{
    [MonoSingletonPath("IFramework/Launcher")]
    public class Launcher : MonoSingleton<Launcher>
    {
        private Game _game;
        public Game game
        {
            get { return _game; }
            set
            {
                _game = value;
                if (_game != null)
                {
                    _game.Init();
                    _game.Startup();
                }
            }
        }

        public static Modules modules { get { return instance._modules; } }

        private static event Action onFixUpdate;
        private static event Action onUpdate;
        private static event Action onLateUpdate;
        private static event Action<bool> onApplicationFocus;
        private static event Action<bool> onApplicationPause;
        private static event Action ondisable;


        Modules _modules;
        private LoomModule _loom;



        private void Awake()
        {
            _modules = new Modules();
            _loom = LoomModule.CreateInstance<LoomModule>("");


        }
        private void OnDisable()
        {
            ondisable?.Invoke();
            _modules.Dispose();
            _loom.Dispose();
            _loom = null;
            _modules = null;
        }

        private void Update()
        {
            if (onUpdate != null)
            {
                onUpdate();
            }
            _loom.Update();
            _modules.Update();
        }
        private void FixedUpdate()
        {
            if (onFixUpdate != null)
            {
                onFixUpdate();
            }
        }

        private void LateUpdate()
        {
            if (onLateUpdate != null)
            {
                onLateUpdate();
            }
        }
        private void OnApplicationFocus(bool focus)
        {
            if (onApplicationFocus != null)
            {
                onApplicationFocus(focus);
            }
        }
        private void OnApplicationPause(bool pause)
        {
            if (onApplicationPause != null)
            {
                onApplicationPause(pause);
            }
        }

        public static void WaitEnvironmentFrame<T>(T action)
        {
            if (instance._loom == null) return;
            instance._loom.RunDelay<T>(action);
        }
        public static void SubscribeWaitEnvironmentFrameHandler<T>(Action<T> action)
        {
            if (instance._loom == null) return;
            instance._loom.AddDelayHandler<T>(action);
        }
        public static void UnSubscribeWaitEnvironmentFrameHandler<T>(Action<T> action)
        {
            if (instance._loom == null) return;
            instance._loom.RemoveDelayHandler<T>(action);
        }
        public static void BindUpdate(Action action)
        {
            onUpdate += action;
        }
        public static void UnBindUpdate(Action action)
        {
            onUpdate -= action;
        }
        public static void BindFixedUpdate(Action action)
        {
            onFixUpdate += action;
        }
        public static void UnBindFixedUpdate(Action action)
        {
            onFixUpdate -= action;
        }
        public static void BindLateUpdate(Action action)
        {
            onLateUpdate += action;
        }
        public static void UnBindLateUpdate(Action action)
        {
            onLateUpdate -= action;
        }
        public static void BindOnApplicationFocus(Action<bool> action)
        {
            onApplicationFocus += action;
        }
        public static void UnBindOnApplicationFocus(Action<bool> action)
        {
            onApplicationFocus -= action;
        }
        public static void BindOnApplicationPause(Action<bool> action)
        {
            onApplicationPause += action;
        }
        public static void UnBindOnApplicationPause(Action<bool> action)
        {
            onApplicationPause -= action;
        }

        public static void BindDisable(Action action)
        {
            ondisable += action;
        }
        public static void UnBindDisable(Action action)
        {
            ondisable -= action;
        }
    }
}
