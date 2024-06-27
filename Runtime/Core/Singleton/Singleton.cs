namespace IFramework.Singleton
{

    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        private volatile static T _instance;
        static object lockObj = new object();
 
        public static T instance
        {
            get
            {
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

        protected virtual void OnSingletonInit() { }


        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }
    }

}
