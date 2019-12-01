/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-21
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework
{
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        protected static T instance;
        static object lockObj = new object();
        public static T Instance
        {
            get
            {
                lock (lockObj)
                    if (instance == null)
                    {
                        instance = SingletonCreator.CreateSingleton<T>();
                        SingletonPool.Set(instance);
                    }
                return instance;
            }
        }
        protected Singleton() { }

        protected virtual void OnSingletonInit() { }
        public virtual void Dispose(){ instance = null;}

        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }
    }
}
