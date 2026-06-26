/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using UnityEngine;

namespace IFramework
{
    public static class RedTreeServiceEx
    {
        public static IServiceCollection UseRedTree(this IServiceCollection services)
        {
            RedTreeService tree = new RedTreeService();
            services.Use<IRedTreeService>(tree);
            return services;
        }
        public static IRedTreeService RedTree(this IServiceProvider services) => services.GetRequiredService<IRedTreeService>();

        public static T CreateRedDot<T>(this IRedTreeService service, string path, Action<T> init) where T : RedDot, new()
        {
            var tree = service.RedTree();
            T result = StaticPool.Get<T>();
            result.tree = tree as RedTreeService;
            init?.Invoke(result);
            result.SetPath(path);
            return result;
        }
        public static RedActiveDot CreateRedActiveDot(this IRedTreeService service, string path,GameObject gameObject)
        {
            return CreateRedDot<RedActiveDot>(service, path, (e) =>
            {
                e.gameObject = gameObject;
            });
        }
    }
}
