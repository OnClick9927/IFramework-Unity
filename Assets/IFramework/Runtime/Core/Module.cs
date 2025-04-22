using System;
using System.Collections.Generic;

namespace IFramework
{
    public struct ModulePriority
    {
        public const int Custom = 1000;
        private int _value;

        public ModulePriority(int value) => _value = value;

        public int value { get { return _value; } set { _value = value; } }


        public static ModulePriority FromValue(int value) => new ModulePriority(value);

        public static implicit operator int(ModulePriority value) => value.value;

        public static implicit operator ModulePriority(int value) => new ModulePriority(value);

        public static ModulePriority operator +(ModulePriority a, ModulePriority b) => new ModulePriority(a.value + b.value);

        public static ModulePriority operator -(ModulePriority a, ModulePriority b) => new ModulePriority(a.value - b.value);
    }

    public abstract class Module : IPriorityQueueNode<int>, IDisposable
    {

        public const string defaultName = "default";
        private bool _disposed;
        protected bool disposed { get { return _disposed; } }
        private bool _binded;
        private int _priority;

        int IPriorityQueueNode<int>.priority { get => _priority; set => _priority = value; }
        int IPriorityQueueNode<int>.position { get; set; }
        long IPriorityQueueNode<int>.insertPosition { get; set; }

        internal int priority { get { return _priority; } }
        internal bool binded { get { return _binded; } set { _binded = value; } }
        public string name { get; private set; }




        protected virtual ModulePriority OnGetDefaultPriority() => ModulePriority.Custom;
        protected abstract void Awake();
        protected abstract void OnDispose();
        public void Dispose()
        {
            if (_disposed) return;
            OnDispose();
            _disposed = true;
        }

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
                Log.FE(string.Format("Type: {0} Non Public Ctor With 0 para Not Find", type));

            return moudle;
        }


        public static T CreateInstance<T>(string name = defaultName, int priority = 0) where T : Module => CreateInstance(typeof(T), name, priority) as T;



    }

    public abstract class UpdateModule : Module
    {
        public void Update()
        {
            if (disposed) return;
            OnUpdate();
        }
        protected abstract void OnUpdate();

    }
    public class Modules : IDisposable
    {
        //private object _lock = new object();
        private Dictionary<Type, Dictionary<string, Module>> _dic;

        private PriorityQueue<Module, int> _queue;
        private List<UpdateModule> _updateModules;


        public Module CreateModule(Type type, string name = Module.defaultName, int priority = 0)
        {
            var mou = Module.CreateInstance(type, name, priority);
            if (SubscribeModule(mou))
                mou.binded = true;
            return mou;
        }

        public T CreateModule<T>(string name = Module.defaultName, int priority = 0) where T : Module => CreateModule(typeof(T), name, priority) as T;



        public Module FindModule(Type type, string name = Module.defaultName)
        {
            if (string.IsNullOrEmpty(name))
                name = type.Name;
            if (!_dic.ContainsKey(type)) return null;
            if (!_dic[type].ContainsKey(name)) return null;
            var module = _dic[type][name];
            return module;

        }

        public Module GetModule(Type type, string name = Module.defaultName, int priority = 0)
        {
            var tmp = FindModule(type, name);
            if (tmp == null)
            {
                tmp = CreateModule(type, name, priority);
            }
            return tmp;
        }



        public T FindModule<T>(string name = Module.defaultName) where T : Module => FindModule(typeof(T), name) as T;

        public T GetModule<T>(string name = Module.defaultName, int priority = 0) where T : Module => GetModule(typeof(T), name, priority) as T;


        internal Modules()
        {
            _dic = new Dictionary<Type, Dictionary<string, Module>>();
            _queue = new PriorityQueue<Module, int>(256);
            _updateModules = new List<UpdateModule>();
        }



        internal void Update()
        {
            for (int i = 0; i < _updateModules.Count; i++)
            {
                _updateModules[i].Update();
            }
        }
        private void SyncUpdateList()
        {
            _updateModules.Clear();
            foreach (var item in _queue)
            {
                if (item is UpdateModule)
                {
                    _updateModules.Add(item as UpdateModule);
                }
            }
        }

        private bool SubscribeModule(Module moudle)
        {
            //lock (_lock)
            {
                Type type = moudle.GetType();
                if (!_dic.ContainsKey(type))
                    _dic.Add(type, new Dictionary<string, Module>());
                var list = _dic[type];
                if (list.ContainsKey(moudle.name))
                {
                    Log.FE(string.Format("Have Bind Module | Type {0}  Name {1}", type, moudle.name));
                    return false;
                }
                else
                {
                    list.Add(moudle.name, moudle);
                    if (_queue.count == _queue.capacity)
                        _queue.Resize(_queue.count * 2);
                    _queue.Enqueue(moudle, moudle.priority);
                    if (moudle is UpdateModule)
                        SyncUpdateList();
                    return true;
                }
            }


        }
        private bool UnSubscribeBindModule(Module moudle)
        {
            if (!moudle.binded) return false;
            moudle.binded = false;
            Type type = moudle.GetType();
            if (!_dic.ContainsKey(type))
            {
                Log.FE(string.Format("01,Have Not Bind Module | Type {0}  Name {1}", type, moudle.name));
                return false;
            }
            else
            {
                var list = _dic[type];

                if (!list.ContainsKey(moudle.name))
                {
                    Log.FE(string.Format("02,Have Not Bind Module | Type {0}  Name {1}", type, moudle.name));
                    return false;
                }
                else
                {
                    _dic[type].Remove(moudle.name);
                    if (_queue.Contains(moudle))
                    {
                        _queue.Remove(moudle);
                        if (moudle is UpdateModule)
                            SyncUpdateList();
                    }
                    return true;
                }
            }

        }

        void IDisposable.Dispose()
        {
            {
                int count = _queue.count;
                Stack<Module> _modules = new Stack<Module>();
                for (int i = 0; i < count; i++)
                {
                    var item = _queue.Dequeue();

                    _modules.Push(item);

                }

                for (int i = 0; i < count; i++)
                {
                    var item = _modules.Pop();
                    UnSubscribeBindModule(item);
                    item.Dispose();
                }
                _updateModules.Clear();
                _queue = null;
                _dic.Clear();
                _dic = null;
            }
        }
    }

}
