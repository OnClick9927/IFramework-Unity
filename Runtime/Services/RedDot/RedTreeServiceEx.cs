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
        public static IServiceCollection UseRedTree(this IServiceCollection collection)
        {
            RedTreeService tree = new RedTreeService();
            collection.UseService(tree);
            return collection;
        }

        public static T CreateRedDot<T>(this IServiceCollection collection, string path, Action<T> init) where T : RedDot, new()
        {
            RedTreeService tree = collection.GetService<RedTreeService>();
            T result = StaticPool.Get<T>();
            result.tree = tree;
            init?.Invoke(result);
            result.SetPath(path);
            return result;
        }
        public static RedActiveDot CreateRedActiveDot(this IServiceCollection collection, string path,GameObject gameObject)
        {
            return CreateRedDot<RedActiveDot>(collection, path, (e) =>
            {
                e.gameObject = gameObject;
            });
        }

        public static void FreshRedDots(this IServiceCollection collection)
        {
            RedTreeService tree = collection.GetService<RedTreeService>();
            tree?.FreshDots();

        }

        public static void SetRedCount(this IServiceCollection collection, string key, int count)
        {
            RedTreeService tree = collection.GetService<RedTreeService>();
            tree.SetCount(key, count);

        }

        public static int GetRedCount(this IServiceCollection collection, string key)
        {
            RedTreeService tree = collection.GetService<RedTreeService>();
            return tree.GetCount(key);
        }
        public static void ReadRedPath(this IServiceCollection collection, string key)
        {
            RedTreeService tree = collection.GetService<RedTreeService>();
            tree.ReadPath(key);
        }
        public static void ClearRedPath(this IServiceCollection collection, string key)
        {
            RedTreeService tree = collection.GetService<RedTreeService>();
            tree.ClearPath(key);
        }
        public static void ClearRedTree(this IServiceCollection collection)
        {
            RedTreeService tree = collection.GetService<RedTreeService>();
            tree.ClearAll();
        }
    }
}
