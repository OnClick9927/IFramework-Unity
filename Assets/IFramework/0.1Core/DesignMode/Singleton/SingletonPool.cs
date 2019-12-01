/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework
{
    public static class SingletonPool
    {
        static Dictionary<Type, ISingleton> pairs;
        static SingletonPool()
        {
            pairs = new Dictionary<Type, ISingleton>();
        }
        public static void Set<T>(T singleton) where T : ISingleton
        {
            Type type = typeof(T);
            if (!pairs.ContainsKey(type))
                pairs.Add(type, singleton);
            else
                throw new Exception("Singleton Err");
        }
        public static T Get<T>() where T : ISingleton
        {
            Type type = typeof(T);
            if (pairs.ContainsKey(type))
                return (T)pairs[type];
            return default(T);
        }
        public static void Dispose<T>() where T : ISingleton
        {
            Type type = typeof(T);
            if (pairs.ContainsKey(type))
            {
                pairs[type].Dispose();
                pairs.Remove(type);
            }
            else
                throw new Exception("Singleton Err");
        }
        public static void Dispose()
        {
            foreach (var item in pairs.Values)
            {
                item.Dispose();
            }
            pairs.Clear();
        }
    }
}
