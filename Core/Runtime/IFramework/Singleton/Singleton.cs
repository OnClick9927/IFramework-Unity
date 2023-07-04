using System;

namespace IFramework.Singleton
{
    /// <summary>
    /// 单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : Unit, ISingleton where T : Singleton<T>
    {
        //防止线程优化
        private volatile static T _instance;
        static object lockObj = new object();
        /// <summary>
        /// 实例
        /// </summary>
        public static T instance
        {
            get
            {
                //通过double check优化
                if (_instance == null)
                {
                    lock (lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = SingletonCreator.CreateSingleton<T>();
                            SingletonCollection.Set(_instance);
                        }
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// ctror
        /// </summary>
        protected Singleton() { }
        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void OnSingletonInit() { }
        /// <summary>
        /// 注销
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (!disposed)
            {
                _instance = null;
            }
        }

        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }
    }

}
