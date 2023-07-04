using System;

namespace IFramework.Singleton
{
    /// <summary>
    /// 单例属性类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonPropertyClass<T> : Unit, ISingleton where T : SingletonPropertyClass<T>
    {
        /// <summary>
        /// 实例
        /// </summary>
        protected static T instance { get { return SingletonProperty<T>.instance; } }
        /// <summary>
        /// ctor
        /// </summary>
        protected SingletonPropertyClass() { }
        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void OnSingletonInit();

        /// <summary>
        /// 注销
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (!disposed)
            {
                SingletonProperty<T>.Dispose();
            }
        }



        void ISingleton.OnSingletonInit()
        {
            OnSingletonInit();
        }
    }

}
