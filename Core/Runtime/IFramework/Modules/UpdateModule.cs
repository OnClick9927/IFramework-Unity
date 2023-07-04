namespace IFramework
{
    /// <summary>
    /// OnUpdate OnEnable OnDisable
    /// </summary>
    public abstract class UpdateModule: Module
    {
        private bool _enable;
        /// <summary>
        /// 开启关闭 Update
        /// </summary>
        public bool enable
        {
            get { return _enable; }
            set
            {
                if (_enable != value)
                    _enable = value;
                if (_enable)
                    OnEnable();
                else
                    OnDisable();
            }
        }
        /// <summary>
        /// 改变 enable
        /// </summary>
        /// <param name="enable"></param>
        public void SetActive(bool enable) { this.enable = enable; }
        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            enable = false;
            base.Dispose();
        }
        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            if (!enable || disposed) return;
            OnUpdate();
        }
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected abstract void OnUpdate();
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

    }
}
