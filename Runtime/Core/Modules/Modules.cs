using System;
using System.Collections.Generic;

namespace IFramework
{


    public class Modules : IDisposable
    {
        private object _lock = new object();
        private Dictionary<Type, Dictionary<string, Module>> _dic;

        private GenericPriorityQueue<Module, int> _queue;
        private List<UpdateModule> _updateModules;


        public Module CreateModule(Type type, string name = Module.defaultName, int priority = 0)
        {
            var mou = Module.CreateInstance(type, name, priority);
            if (SubscribeModule(mou))
                mou.binded = true;
            return mou;
        }

        public T CreateModule<T>(string name = Module.defaultName, int priority = 0) where T : Module
        {
            return CreateModule(typeof(T), name, priority) as T;
        }



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



        public T FindModule<T>(string name = Module.defaultName) where T : Module
        {
            return FindModule(typeof(T), name) as T;
        }

        public T GetModule<T>(string name = Module.defaultName, int priority = 0) where T : Module
        {
            return GetModule(typeof(T), name, priority) as T;
        }


        public Modules()
        {
            _dic = new Dictionary<Type, Dictionary<string, Module>>();
            _queue = new GenericPriorityQueue<Module, int>(256);
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
            lock (_lock)
            {
                Type type = moudle.GetType();
                if (!_dic.ContainsKey(type))
                    _dic.Add(type, new Dictionary<string, Module>());
                var list = _dic[type];
                if (list.ContainsKey(moudle.name))
                {
                    Log.E(string.Format("Have Bind Module | Type {0}  Name {1}", type, moudle.name));
                    return false;
                }
                else
                {
                    list.Add(moudle.name, moudle);
                    if (_queue.count == _queue.capacity)
                        _queue.Resize(_queue.count * 2);
                    _queue.Enqueue(moudle, moudle.priority);
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
                Log.E(string.Format("01,Have Not Bind Module | Type {0}  Name {1}", type, moudle.name));
                return false;
            }
            else
            {
                var list = _dic[type];

                if (!list.ContainsKey(moudle.name))
                {
                    Log.E(string.Format("02,Have Not Bind Module | Type {0}  Name {1}", type, moudle.name));
                    return false;
                }
                else
                {
                    _dic[type].Remove(moudle.name);
                    if (_queue.Contains(moudle))
                    {
                        _queue.Remove(moudle);
                        SyncUpdateList();
                    }
                    return true;
                }
            }

        }

        public void Dispose()
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
