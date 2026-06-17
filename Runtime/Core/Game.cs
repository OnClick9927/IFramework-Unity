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
using System.Linq;
using UnityEngine;
namespace IFramework
{
    interface IGameService
    {
        string name { get; }
        void OnQuit(Game game);
        void OnUse(Game game);
    }

    public abstract class GameServiceBase : IGameService
    {
        public string name { get; internal set; }
        protected abstract void OnUse(Game game);
        protected abstract void OnQuit(Game game);

        void IGameService.OnQuit(Game game)
        {
            OnQuit(game);
        }

        void IGameService.OnUse(Game game)
        {
            OnUse(game);
        }
    }
    partial class Game
    {
        public static void BindUpdate(Action action) => Launcher.BindUpdate(action);
        public static void UnBindUpdate(Action action) => Launcher.UnBindUpdate(action);
        public static void BindFixedUpdate(Action action) => Launcher.BindFixedUpdate(action);
        public static void UnBindFixedUpdate(Action action) => Launcher.UnBindFixedUpdate(action);

        public static void BindLateUpdate(Action action) => Launcher.BindLateUpdate(action);
        public static void UnBindLateUpdate(Action action) => Launcher.UnBindLateUpdate(action);
        public static void BindOnApplicationFocus(Action<bool> action) => Launcher.BindOnApplicationFocus(action);
        public static void UnBindOnApplicationFocus(Action<bool> action) => Launcher.UnBindOnApplicationFocus(action);
        public static void BindOnApplicationPause(Action<bool> action) => Launcher.BindOnApplicationPause(action);
        public static void UnBindOnApplicationPause(Action<bool> action) => Launcher.UnBindOnApplicationPause(action);
        public static void BindDisable(Action action) => Launcher.BindDisable(action);
        public static void UnBindDisable(Action action) => Launcher.UnBindDisable(action);
    }
    public abstract partial class Game : MonoBehaviour
    {
        private class ServiceSeg
        {
            public Type type;
            private List<GameServiceBase> services = new List<GameServiceBase>();
            public Dictionary<string, GameServiceBase> map = new Dictionary<string, GameServiceBase>();

            public void Add(GameServiceBase service)
            {
                services.Add(service);
                var name = service.name;
                map.Add(name, service);
            }
            public IReadOnlyList<GameServiceBase> GetServices()
            {
                return services;
            }
            internal GameServiceBase GetService(string name)
            {
                if (string.IsNullOrEmpty(name) && services.Count != 0)
                    return services[0];
                return map.TryGetValue(name, out var service) ? service : null;
            }
        }
        public static Game Current { get { return Launcher.Instance.game; } }
        private bool quited;
        private Stack<IGameService> services = new Stack<IGameService>();
        private Dictionary<Type, ServiceSeg> serviceMap = new Dictionary<Type, ServiceSeg>();


        private void Awake()
        {
            transform.SetParent(Launcher.Instance.transform);
            Launcher.Instance.game = this;
            quited = false;
            Startup();
        }

        public GameServiceBase UseService<T>(T service, string name = "", bool register = true) where T : GameServiceBase
        {
            service.name = name;
            (service as IGameService).OnUse(this);
            services.Push(service);
            var type = service.GetType();
            if (!serviceMap.TryGetValue(type, out var result))
            {
                result = new();
                serviceMap.Add(type, result);
            }
            result.Add(service);
            var value = GetService<ValueService>();
            if (value != null)
            {
                value.Inject(service);
                if (register)
                    value.RegisterInstance(type, service);
            }
            return service;
        }
        public IReadOnlyList<GameServiceBase> GetServices<T>() where T : GameServiceBase
        {
            var type = typeof(T);
            if (serviceMap.TryGetValue(type, out var result))
            {
                return result.GetServices();
            }
            return null;
        }
        public T GetService<T>(string name = "") where T : GameServiceBase
        {
            var type = typeof(T);
            if (serviceMap.TryGetValue(type, out var result))
            {
                return result.GetService(name) as T;
            }
            return null;
        }


        public void Quit()
        {
            if (quited) return;
            quited = true;
            OnQuit();
            var count = services.Count;
            for (int i = 0; i < count; i++)
            {
                var service = services.Pop();
                service.OnQuit(this);
            }
            serviceMap.Clear();
        }
        protected virtual void OnQuit() { }
        protected abstract void Startup();
        private void OnDestroy() => Quit();

    }
}
