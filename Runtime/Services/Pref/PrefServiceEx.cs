using System;
using UnityEngine;

namespace IFramework
{
    public static class PrefServiceEx
    {
        public static string defaultKey = $"__{SystemInfo.deviceName}__";
        public static IServiceCollection UsePref(this IServiceCollection services, IPrefConverter converter, IPrefLoader loader)
        {
            PrefService service = new PrefService(converter, loader, services.Name);

            services.Use<IPrefService>(service);
            return services;
        }
        public static IPrefService Pref(this IServiceProvider services) => services.GetRequiredService<IPrefService>();


     
        public static T Load<T>(this IPrefService service, string key) where T : class, new() => service.Load(typeof(T), key) as T;
        public static T Load<T>(this IPrefService service) where T : class, new() => Load<T>(service, defaultKey);

        public static void Save<T>(this IPrefService service, string key, T pref) where T : class
        {
            if (string.IsNullOrEmpty(key)) return;
            if (pref == null) return;
            service.Save(key, pref);
        }
        public static void Save<T>(this IPrefService service, T pref) where T : class => service.Save<T>(defaultKey, pref);
        public static void Save<T>(this IPrefService service, PrefContext<T> context) where T : class, new() => service.Save<T>(context.key, context.Value);
    }

}



