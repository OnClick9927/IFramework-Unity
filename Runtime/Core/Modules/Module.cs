using System;

namespace IFramework
{

    public abstract class Module : IGenericPriorityQueueNode<int>,IDisposable
    {
 
        public const string defaultName = "default";


        private bool _binded;
        private int _priority;
 
        int IGenericPriorityQueueNode<int>.priority { get => _priority; set => _priority = value; }
        int IGenericPriorityQueueNode<int>.position { get; set; }
        long IGenericPriorityQueueNode<int>.insertPosition { get; set; }

        public int priority { get { return _priority; } }


        public bool binded { get { return _binded; } internal set { _binded = value; } }


        public string name { get; set; }




        protected abstract void Awake();

 
        protected Module() { }

        public static Module CreateInstance(Type type, string name = defaultName, int priority = 0)
        {
            Module moudle = Activator.CreateInstance(type) as Module;
            if (moudle != null)
            {
                moudle._binded = false;
                moudle.name = name;
                moudle._priority = moudle.OnGetDefaultPriority().value + priority;
                moudle.Awake();
                if (moudle is UpdateModule)
                {
                    (moudle as UpdateModule).enable = true;
                }
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

        public bool disposed { get { return _disposed; } }

        protected abstract void OnDispose();

        public virtual void Dispose()
        {
            if (_disposed) return;
            OnDispose();
            _disposed = true;
        }
    }
}
