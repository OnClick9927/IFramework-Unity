namespace IFramework
{
    /// <summary>
    /// 基类
    /// </summary>
    public abstract class Unit 
    {
        private bool _disposed;
        /// <summary>
        /// 是否已经释放
        /// </summary>
        public bool disposed { get { return _disposed; } }

        /// <summary>
        /// 释放时
        /// </summary>
        protected abstract void OnDispose();
        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Dispose()
        {
            if (_disposed) return;
            OnDispose();
            _disposed = true;
        }

    }
}
