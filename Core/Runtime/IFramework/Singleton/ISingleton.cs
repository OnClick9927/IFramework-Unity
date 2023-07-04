using System;

namespace IFramework.Singleton
{
    /// <summary>
    /// 单例
    /// </summary>
    public interface ISingleton : IDisposable
    {
        /// <summary>
        /// 单例初始化
        /// </summary>
        void OnSingletonInit();
    }

}
