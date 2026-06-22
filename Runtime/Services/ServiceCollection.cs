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
            private List<ServiceBase> services = new List<ServiceBase>();
            public Dictionary<string, ServiceBase> map = new Dictionary<string, ServiceBase>();

            public void Add(ServiceBase service)
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
            public IReadOnlyList<ServiceBase> GetServices()
            {
                return services;
            }
            internal ServiceBase GetService(string name)
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

        public T UseService<T>(T service, string name = "") where T : ServiceBase
        {
            if (service == null) return null;
            service.Name = name;
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
                value.RegisterValue(type, service, name);
            }

     (service as IService).OnUse(this);
            services.Push(service);
            return service;
        }
        public IReadOnlyList<ServiceBase> GetServices<T>() where T : ServiceBase
        {
            var type = typeof(T);
            if (serviceMap.TryGetValue(type, out var result))
            {
                return result.GetServices();
            }
            return null;
        }
        public T GetService<T>(string name = "") where T : ServiceBase
        {
            var type = typeof(T);
            if (serviceMap.TryGetValue(type, out var result))
            {
                return result.GetService(name) as T;
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
