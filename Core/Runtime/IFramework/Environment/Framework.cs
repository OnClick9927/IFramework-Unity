using System;
using System.Collections.Generic;

namespace IFramework
{
    /// <summary>
    /// 框架入口
    /// </summary>
    public static class Framework
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        private static Dictionary<int, IEnvironment> envs = new Dictionary<int, IEnvironment>();
        private static object _lock = new object();

        public static IEnvironment current { get { return Environment.current; } }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        /// <summary>
        /// 实例化环境
        /// </summary>
        /// <param name=" envType">环境类型</param>
        /// <returns>环境</returns>
        public static IEnvironment CreateEnv(EnvironmentType envType)
        {
            lock (_lock)
            {
                IEnvironment env;
                if (envs.TryGetValue((int)envType, out env))
                {
                    throw new Exception(string.Format("The EnvironmentType {0} is not null ", envType));
                }
                else
                {
                    env = new Environment(envType);
                    envs.Add((int)envType, env);
                    return env;
                }
            }
        }
        /// <summary>
        /// 根据序号获取环境
        /// </summary>
        /// <param name=" envType">环境类型</param>
        /// <returns></returns>
        public static IEnvironment GetEnv(EnvironmentType envType)
        {
            lock (_lock)
            {
                IEnvironment env;
                if (envs.TryGetValue((int)envType, out env))
                {
                    return env;
                }
                else
                {
                    throw new Exception(string.Format("The EnvironmentType {0} Error Not Find ,Please Check ", envType));
                }
            }
        }
        /// <summary>
        /// 释放环境
        /// </summary>
        /// <param name="envType"></param>
        public static void DisposeEnv(EnvironmentType envType)
        {
            var env = GetEnv(envType);
            if (env != null)
            {
                env.Dispose();
                lock (_lock)
                {
                    envs.Remove((int)envType);
                }
            }
        }

        /// <summary>
        /// 绑顶 方法 到一个环境的 Update
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="env">环境</param>
        public static void BindEnvUpdate(this Action action, IEnvironment env)
        {
            env.BindUpdate(action);
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Update
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="env">环境</param>
        public static void UnBindEnvUpdate(this Action action, IEnvironment env)
        {
            env.UnBindUpdate(action);
        }
        /// <summary>
        /// 绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="env">环境</param>
        public static void BindEnvDispose(this Action action, IEnvironment env)
        {
            env.BindDispose(action);
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name="env">环境</param>
        public static void UnBindEnvDispose(this Action action, IEnvironment env)
        {
            env.UnBindDispose(action);
        }

        /// <summary>
        /// 绑顶 方法 到一个环境的 Update
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name=" envType"></param>
        public static void BindEnvUpdate(this Action action, EnvironmentType envType)
        {
            action.BindEnvUpdate(GetEnv(envType));
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Update
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name=" envType"></param>
        public static void UnBindEnvUpdate(this Action action, EnvironmentType envType)
        {
            action.UnBindEnvUpdate(GetEnv(envType));
        }
        /// <summary>
        /// 绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name=" envType"></param>
        public static void BindEnvDispose(this Action action, EnvironmentType envType)
        {
            action.BindEnvDispose(GetEnv(envType));
        }
        /// <summary>
        /// 解除绑顶 方法 到一个环境的 Dispose
        /// </summary>
        /// <param name="action">方法</param>
        /// <param name=" envType"></param>
        public static void UnBindEnvDispose(this Action action, EnvironmentType envType)
        {
            action.UnBindEnvDispose(GetEnv(envType));
        }
    }
}
