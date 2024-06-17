namespace IFramework.Singleton
{
    /// <summary>
    /// 单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
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
                            _instance = new T();
                            _instance.OnSingletonInit();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void OnSingletonInit() { }


        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }
    }

}
