namespace IFramework
{

    public abstract class UpdateModule : Module
    {
        public void Update()
        {
            if (disposed) return;
            OnUpdate();
        }
        protected abstract void OnUpdate();
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

    }
}
