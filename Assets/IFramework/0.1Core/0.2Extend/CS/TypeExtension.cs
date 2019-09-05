/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace IFramework
{
    public static partial class TypeExtension
    {
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
        public static IEnumerable<Type> GetSubTypesInAssemblys(this Type self)
        {
            if (self.IsInterface)
                return AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(item => item.GetTypes())
                                .Where(item =>item.GetInterfaces().Contains(self));
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(item => item.GetTypes())
                            .Where(item => item.IsSubclassOf(self));
        }
        public static object CreatInstance(this Type self)
        {
            return self.IsValueType ? Activator.CreateInstance(self) : null;
        }
        public static bool IsSubClassOfInterface(this Type self, Type Interface)
        {
            return !self.IsInterface && self.GetInterfaces().Contains(Interface);
        }
        //                                                       typeof(IList<>)......
        public static bool IsSubclassOfGeneric(this Type self, Type genericType)
        {
#if NETFX_CORE
                if (!genericTypeDefinition.GetTypeInfo().IsGenericTypeDefinition)
#else
            if (!genericType.IsGenericTypeDefinition)
#endif
                return false;

#if NETFX_CORE
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals(genericTypeDefinition))
#else
            if (self.IsGenericType && self.GetGenericTypeDefinition().Equals(genericType))
#endif
                return true;

#if NETFX_CORE
                Type baseType = type.GetTypeInfo().BaseType;
#else
            Type baseType = self.BaseType;
#endif
            if (baseType != null && baseType != typeof(object))
            {
                if (IsSubclassOfGeneric(baseType, genericType))
                    return true;
            }

            foreach (Type t in self.GetInterfaces())
            {
                if (IsSubclassOfGeneric(t, genericType))
                    return true;
            }

            return false;
        }

        public static IList<Type> GetTypeTree(this Type t)
        {
            var types = new List<Type>();
            while (t.BaseType != null)
            {
                types.Add(t);
                t = t.BaseType;
            }

            return types;
        }


    }
}