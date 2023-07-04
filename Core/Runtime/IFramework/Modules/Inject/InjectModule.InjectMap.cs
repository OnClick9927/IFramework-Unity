using System;
using System.Collections.Generic;

namespace IFramework.Inject
{
    public partial class InjectModule
    {
        private class InjectMap<T>
        {
            public class Value 
            {
                public string key;
                public T value { get; set; }
            }

            protected List<Value> containers;
            protected Dictionary<Type, List<int>> map;
            public InjectMap()
            {
                containers = new List<Value>();
                map = new Dictionary<Type, List<int>>();
            }
            public void Set(Type super, string key, T t)
            {
                List<int> list;
                if (!map.TryGetValue(super, out list))
                {
                    list = new List<int>();
                    map.Add(super, list);
                }

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    int index = list[i];
                    var value = containers[index];
                    if (value.key == key)
                    {
                        value.value = t;
                        return;
                    }
                }
                list.Add(containers.Count);
                containers.Add(new Value() { key = key, value = t });
            }
            public T Get(Type super, string key)
            {
                List<int> list;
                if (map.TryGetValue(super, out list))
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        int index = list[i];
                        var value = containers[index];
                        if (value.key == key)
                        {
                            return value.value;
                        }
                    }
                }
                return default(T);
            }

            protected T GetByindex(int index)
            {
                return containers[index].value;
            }
            public void Clear()
            {
                map.Clear();
                containers.Clear();
            }

            public IEnumerable<T> Values
            {
                get
                {
                    for (int i = 0; i < containers.Count; i++)
                    {
                        yield return GetByindex(i);
                    }
                }
            }
        }
    }
}
