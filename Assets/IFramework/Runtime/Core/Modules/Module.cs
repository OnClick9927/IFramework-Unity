using System;

namespace IFramework
{

    public abstract class Module : IGenericPriorityQueueNode<int>, IDisposable
    {

        public const string defaultName = "default";

        private bool _binded;
        private int _priority;

        int IGenericPriorityQueueNode<int>.priority { get => _priority; set => _priority = value; }
        int IGenericPriorityQueueNode<int>.position { get; set; }
        long IGenericPriorityQueueNode<int>.insertPosition { get; set; }

        internal int priority { get { return _priority; } }


        internal bool binded { get { return _binded; }  set { _binded = value; } }


        public string name { get; set; }




        protected abstract void Awake();


        public static Module CreateInstance(Type type, string name = defaultName, int priority = 0)
        {
            Module moudle = Activator.CreateInstance(type) as Module;
            if (moudle != null)
            {
                moudle._binded = false;
                moudle.name = name;
                moudle._priority = moudle.OnGetDefaultPriority().value + priority;
                moudle.Awake();
            }
            else
                Log.E(string.Format("Type: {0} Non Public Ctor With 0 para Not Find", type));

            return moudle;
        }

        protected virtual ModulePriority OnGetDefaultPriority()
        {
            return ModulePriority.Custom;
        }

        public static T CreateInstance<T>(string name = defaultName, int priority = 0) where T : Module
        {
            return CreateInstance(typeof(T), name, priority) as T;
        }
        private bool _disposed;

        protected bool disposed { get { return _disposed; } }

        protected abstract void OnDispose();

        public void Dispose()
        {
            if (_disposed) return;
            OnDispose();
            _disposed = true;
        }
    }
}
