/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-21
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Reflection;

namespace IFramework
{
    internal static class SingletonCreator
    {
        internal static T CreateSingleton<T>() where T : class, ISingleton
        {
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
            if (ctor == null) throw new Exception("Non-Public Constructor() not found! in " + typeof(T));
            var retInstance = ctor.Invoke(null) as T;
            retInstance.OnSingletonInit();
            return retInstance;
        }
    }

}
