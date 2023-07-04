using System;
using System.Collections.Generic;

namespace IFramework.Singleton
{
    /// <summary>
    /// 单例合集
    /// </summary>
    public static class SingletonCollection
    {
        static Dictionary<Type, ISingleton> pairs;
        static SingletonCollection()
        {
            pairs = new Dictionary<Type, ISingleton>();
           // Framework.onDispose += Dispose;
        }
        /// <summary>
        /// 注入单例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="singleton"></param>
        public static void Set<T>(T singleton) where T : ISingleton
        {
            Type type = typeof(T);
            if (!pairs.ContainsKey(type))
                pairs.Add(type, singleton);
            else
                throw new Exception("Singleton Err");
        }
        /// <summary>
        /// 注销一个单例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Dispose<T>() where T : ISingleton
        {
            Type type = typeof(T);
            if (pairs.ContainsKey(type))
            {
                pairs[type].Dispose();
                pairs.Remove(type);
            }
            else
                throw new Exception("SingletonPool dispose Err "+typeof(T));
        }
        /// <summary>
        /// 注销所有单例
        /// </summary>
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
