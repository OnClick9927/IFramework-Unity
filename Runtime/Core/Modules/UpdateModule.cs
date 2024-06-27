namespace IFramework
{

    public abstract class UpdateModule: Module
    {
        private bool _enable;

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

        public void SetActive(bool enable) { this.enable = enable; }

        public override void Dispose()
        {
            enable = false;
            base.Dispose();
        }

        public void Update()
        {
            if (!enable || disposed) return;
            OnUpdate();
        }
        protected abstract void OnUpdate();
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

    }
}
