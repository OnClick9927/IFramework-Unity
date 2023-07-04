using System;

namespace IFramework
{
    /// <summary>
    /// 模块组
    /// </summary>
    public interface IModules 
    {
        /// <summary>
        /// 创建模块
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        Module CreateModule(Type type, string name = Module.defaultName, int priority = 0);
        /// <summary>
        /// 创建模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        T CreateModule<T>(string name = Module.defaultName, int priority = 0) where T : Module;
        /// <summary>
        /// 获取模块
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        Module GetModule(Type type, string name = Module.defaultName, int priority = 0);
        /// <summary>
        /// 获取模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        T GetModule<T>(string name = Module.defaultName, int priority = 0) where T : Module;
        /// <summary>
        /// 查找模块
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Module FindModule(Type type, string name = Module.defaultName);
        /// <summary>
        /// 查找模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T FindModule<T>(string name = Module.defaultName) where T : Module;
    }
}