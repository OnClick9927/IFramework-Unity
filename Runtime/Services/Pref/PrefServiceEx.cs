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

            services.UseService(service);
            return services;
        }

        public static void SetPrefContext<T>(this IServiceCollection services, PrefContext<T> context) where T : class, new()
        {
            PrefService service = services.GetService<PrefService>();
            service.SetPrefContext(context);
            services.RegisterValue(context);

        }
        public static void ClearPref(this IServiceCollection services)
        {
            PrefService service = services.GetService<PrefService>();
            service.ClearPref();
        }
        public static void SaveAllPref(this IServiceCollection services)
        {
            PrefService service = services.GetService<PrefService>();
            service.SaveAllPref();
        }
        public static object LoadPref(this IServiceCollection services, Type type, string key)
        {
            PrefService service = services.GetService<PrefService>();
            var pref = service.LoadPref(type, key);
            return pref;
        }
        public static T LoadPref<T>(this IServiceCollection services, string key) where T : class, new() => LoadPref(services, typeof(T), key) as T;
        public static T LoadPref<T>(this IServiceCollection services) where T : class, new() => LoadPref<T>(services, defaultKey);
        public static void SavePref<T>(this IServiceCollection services, string key, T pref) where T : class
        {
            if (string.IsNullOrEmpty(key)) return;
            if (pref == null) return;
            PrefService service = services.GetService<PrefService>();
            service.SavePref(key, pref);
        }
        public static void SavePref<T>(this IServiceCollection services, T pref) where T : class => SavePref<T>(services, defaultKey, pref);
     
        
        
        public static void SavePref<T>(this IServiceCollection services, PrefContext<T> context) where T : class, new() => SavePref<T>(services, context.key, context.Value);
        public static PrefContext<T> FindPrefContext<T>(this IServiceCollection services) where T : class, new()
        {
            PrefService service = services.GetService<PrefService>();
            return service.FindPrefContext<T>();
        }
    }

}



