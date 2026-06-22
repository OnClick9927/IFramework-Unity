/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
namespace IFramework
{
    public static class ValueServiceEx
    {
        public static IServiceCollection UseValues(this IServiceCollection services)
        {
            ValueService service = new ValueService();
            services.UseService(service);
            return services;
        }

        private static ValueService Check(IServiceCollection services)
        {
            var service = services.GetService<ValueService>();
            if (service == null)
            {
                Log.FE("Game.UseValues First");
            }
            return service;
        }
        public static void RegisterValue(this IServiceCollection services, Type type, object instance, string name = "")
        {
            var service = Check(services);
            service?.RegisterValue(type, instance, name);
        }

        public static object GetValue(this IServiceCollection services, Type type, string name = "")
        {
            var service = Check(services);
            return service?.Get(type, name);
        }

        public static void InjectFields(this IServiceCollection services, object obj)
        {
            var service = Check(services);
            service?.Inject(obj);
        }
        public static void RegisterType<TBaseType, TType>(this IServiceCollection services) where TType : class, TBaseType, new()
        {
            var service = Check(services);
            service?.RegisterType<TBaseType, TType>();
        }
        public static void RegisterValue<T>(this IServiceCollection services, T instance, string name = "") where T : class => RegisterValue(services, typeof(T), instance, name);
        public static T GetValue<T>(this IServiceCollection services, string name = "") where T : class => GetValue(services, typeof(T), name) as T;
        public static void RegisterType<TType>(this IServiceCollection services) where TType : class, new() => RegisterType<TType, TType>(services);
    }
}
