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
    public abstract class ServiceBase : IService
    {
        public IServiceProvider EnterService<T>(string name = "") where T : class, IService => services.EnterService<T>(name);
        public IReadOnlyList<IService> GetServices<T>() where T : class, IService => services.GetServices<T>();

        public T GetService<T>(string name = "") where T : class, IService => services.GetService<T>(name);
        public string Name { get; set; }

        IServiceCollection IService.GetServices() => services;
        protected IServiceCollection services { get; private set; }


        protected abstract void OnUse(IServiceCollection services);
        protected abstract void OnQuit(IServiceCollection services);
        protected abstract void OnEnter(IServiceCollection services);

        void IService.OnEnter(IServiceCollection services)
        {
            var value = services.Values();
            if (value != null)
                value.Inject(this);
            OnEnter(services);
        }
        void IService.OnQuit(IServiceCollection services)
        {
            OnQuit(services);
        }

        void IService.OnUse(IServiceCollection services)
        {
            this.services = services;
            OnUse(services);
        }


    }

}
