using System;
using System.Reflection;

namespace IFramework.Singleton
{
    static class SingletonCreator
    {
        public static T CreateSingleton<T>() where T : class, ISingleton
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
