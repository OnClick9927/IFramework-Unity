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
            services.Use<IValueService>(service);
            service.Register(services.GetType(), services, string.Empty);
            return services;
        }
        public static IValueService Values(this IServiceProvider services)
        {
            return services.GetRequiredService<IValueService>();
        }

        public static void Register<T>(this IValueService services, T instance, string name = "") where T : class
            => services.Register(typeof(T), instance, name);
        public static T Get<T>(this IValueService services, string name = "") where T : class
            => services.Get(typeof(T), name) as T;
        public static void RegisterType<TType>(this IValueService services) where TType : class, new() => services.Register<TType, TType>();
    }
}
