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
    class ServiceCollection : IServiceCollection
    {
        private class ServiceSeg
        {
            private List<IService> services = new List<IService>();
            public Dictionary<string, IService> map = new Dictionary<string, IService>();

            public void Add(IService service)
            {
                var name = service.Name;
                if (!map.TryAdd(name, service))
                {
                    Log.FE($"Same Name Service  {name}  {service.GetType()}");
                }
                else
                {
                    services.Add(service);
                }

            }
            public IReadOnlyList<IService> GetServices()
            {
                return services;
            }
            internal IService GetService(string name)
            {
                if (string.IsNullOrEmpty(name))
                    return services.Count > 0 ? services[0] : null;
                return map.TryGetValue(name, out var service) ? service : null;
            }
        }
        private Stack<IService> services = new Stack<IService>();
        private Dictionary<Type, ServiceSeg> serviceMap = new Dictionary<Type, ServiceSeg>();

        private string _name;
        public ServiceCollection(string name)
        {
            _name = name;
        }

        public string Name => _name;

        public IServiceProvider EnterService<T>(string name = "") where T : class, IService
        {
            var result = this.GetRequiredService<T>(name);
            result?.OnEnter(this);
            return this;
        }
        public BaseType Use<BaseType>(BaseType service, string name = "") where BaseType : class, IService
        {
            if (service == null) return default;
            service.Name = name;
            var type = typeof(BaseType);
            if (!serviceMap.TryGetValue(type, out var result))
            {
                result = new();
                serviceMap.Add(type, result);
            }

            result.Add(service);
            var value = this.GetService<IValueService>();
            if (value != null)
            {
                //value.Inject(service);
                value.Register<BaseType>(service, name);
            }
            service.OnUse(this);
            services.Push(service);
            return service;
        }
        public IReadOnlyList<IService> GetServices<T>() where T : class, IService
        {
            var type = typeof(T);
            if (serviceMap.TryGetValue(type, out var result))
            {
                return result.GetServices();
            }
            return null;
        }
        public T GetService<T>(string name = "") where T : class, IService
        {
            var type = typeof(T);
            if (serviceMap.TryGetValue(type, out var result))
            {
                var service = result.GetService(name);
                if (service is T _T)
                {
                    return _T;
                }
            }
            return null;
        }

        internal void Quit()
        {
            var count = services.Count;
            for (int i = 0; i < count; i++)
            {
                var service = services.Pop();
                service.OnQuit(this);
            }
            serviceMap.Clear();
        }


    }

}
