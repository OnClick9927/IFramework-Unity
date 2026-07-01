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
using UnityEngine;
namespace IFramework
{

    public abstract class Game : MonoBehaviour, IServiceCollection
    {
        public static Game Current { get { return Launcher.Instance.game; } }

        string IServiceCollection.Name => services == null ? GetType().Name : services.Name;

        private bool quited;
        private ServiceCollection services;


        private void Awake()
        {
            services = new ServiceCollection((this as IServiceCollection).Name);
            transform.SetParent(Launcher.Instance.transform);
            Launcher.Instance.game = this;
            quited = false;
            Startup();
        }




        public void Quit()
        {
            if (quited) return;
            quited = true;
            OnQuit();
            services.Quit();
            this.ClearDisposable();
        }
        protected virtual void OnQuit() { }
        protected abstract void Startup();
        private void OnDestroy() => Quit();

        public IServiceProvider EnterService<T>(string name = "") where T : class, IService => services.EnterService<T>(name);
        public T Use<T>(T service, string name = "") where T : class, IService => services.Use<T>(service, name);

        public IReadOnlyList<IService> GetServices<T>() where T : class, IService => services.GetServices<T>();
        public T GetService<T>(string name = "") where T : class, IService => services.GetService<T>(name);




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
