using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IFramework
{
    public static class Ex
    {
        /// <summary>
        /// 拼接路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="toCombinePath"></param>
        /// <returns></returns>
        public static string CombinePath(this string path, string toCombinePath)
        {
            return Path.Combine(path, toCombinePath).ToRegularPath();
        }


        /// <summary>
        /// 规范路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ToRegularPath(this string path)
        {
            path = path.Replace('\\', '/');
            return path;
        }



        /// <summary>
        /// 获取当前程序集中的类型的子类，3.5有问题
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetSubTypesInAssembly(this Type self)
        {
            if (self.IsInterface)
                return Assembly.GetExecutingAssembly()
                               .GetTypes()
                               .Where(item => item.GetInterfaces().Contains(self));
            return Assembly.GetExecutingAssembly()
                           .GetTypes()
                           .Where(item => item.IsSubclassOf(self));
        }
        /// <summary>
        /// 获取所有程序集中的类型的子类，3.5有问题
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetSubTypesInAssemblys(this Type self)
        {
            if (self.IsInterface)
                return AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(item => item.GetTypes())
                                .Where(item => item.GetInterfaces().Contains(self));
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(item => item.GetTypes())
                            .Where(item => item.IsSubclassOf(self));
        }


        /// <summary>
        /// 字符串结尾转Unix编码
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToUnixLineEndings(this string self)
        {
            return self.Replace("\r\n", "\n").Replace("\r", "\n");
        }

    }

}
