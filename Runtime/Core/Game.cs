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

    public abstract class IGameService
    {
        public string name { get; internal set; }
        public abstract void OnUse(Game game);
        public abstract void OnQuit(Game game);
    }

    public abstract class Game : MonoBehaviour
    {
        public Modules modules => _modules;
        public static Game Current { get { return Launcher.Instance.game; } }
        private bool quited;
        Modules _modules;
        private Stack<IGameService> services = new Stack<IGameService>();
        private Dictionary<Type, List<IGameService>> serviceMap = new Dictionary<Type, List<IGameService>>();


        private void Awake()
        {
            _modules = new Modules();
            transform.SetParent(Launcher.Instance.transform);
            Launcher.Instance.game = this;
            quited = false;
            BindUpdate(_modules.Update);
            //this.UseValue();
            Startup();
        }
        public void UseService(IGameService context, string name)
        {
            context.name = name;
            UseService(context);
        }

        public void UseService(IGameService context)
        {
            context.OnUse(this);
            services.Push(context);
            var type = context.GetType();
            if (!serviceMap.TryGetValue(type, out var result))
            {
                result = new List<IGameService>();
                serviceMap.Add(type, result);
            }
            result.Add(context);
        }
        public IReadOnlyList<IGameService> GetServices<T>() where T : IGameService
        {
            var type = typeof(T);
            if (serviceMap.TryGetValue(type, out var result))
            {
                return result;
            }
            return null;
        }

        public T GetService<T>(string name="") where T : IGameService
        {
            var result = GetServices<T>();
            if (result != null)
            {
                if (string.IsNullOrEmpty(name))
                    return result.FirstOrDefault() as T;
                return result.FirstOrDefault(x=>x.name==name) as T;
            }
            return null;
        }


        public void Quit()
        {
            if (quited) return;
            quited = true;
            OnQuit();
            UnBindUpdate(_modules.Update);
            ((IDisposable)_modules).Dispose();
            var count = services.Count;
            for (int i = 0; i < count; i++)
            {
                var service = services.Pop();
                service.OnQuit(this);
            }
            _modules = null;
            serviceMap.Clear();
        }
        protected virtual void OnQuit() { }
        protected abstract void Startup();





        private void OnDestroy() => Quit();
















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
}
