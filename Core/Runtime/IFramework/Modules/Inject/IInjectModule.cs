using System;
using System.Collections.Generic;

namespace IFramework.Inject
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInjectModule
    {
        /// <summary>
        /// 清除
        /// </summary>
        void Clear();

        /// <summary>
        /// 反射注入
        /// </summary>
        /// <param name="obj"></param>
        void Inject(object obj);

        /// <summary>
        /// 注入所有
        /// </summary>
        void InjectInstances();

        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="name"></param>
        void Subscribe<Type>(string name = null);
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="BaseType"></typeparam>
        /// <typeparam name="Type"></typeparam>
        /// <param name="name"></param>
        void Subscribe<BaseType, Type>(string name = null) where Type : BaseType;
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="name"></param>
        void Subscribe(Type source, Type target, string name = null);
        /// <summary>
        /// 注册实例
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="inject"></param>
        void SubscribeInstance<Type>(Type instance, string name = null, bool inject = true) where Type : class;

        /// <summary>
        /// 注册实例
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="inject"></param>
        /// <exception cref="Exception"></exception>
        void SubscribeInstance(Type baseType, object instance, string name = null, bool inject = true);
        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        T GetValue<T>(string name = null, params object[] args) where T : class;
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="name"></param>
        /// <param name="constructorArgs"></param>
        /// <returns></returns>
        object GetValue(Type baseType, string name = null, params object[] constructorArgs);
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<object> GetValues(Type type);
        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <returns></returns>
        IEnumerable<Type> GetValues<Type>();
    }
}