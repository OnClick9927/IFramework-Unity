using System;
using System.Collections.Generic;

namespace IFramework.Inject
{
    public partial class InjectModule
    {
        private class InjectInstanceMap : InjectMap<object>
        {
            public IEnumerable<object> GetInstances(Type type)
            {
                List<int> list;
                if (map.TryGetValue(type, out list))
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        int index = list[i];
                        var value = containers[index];
                        yield return value.value;
                    }
                }
            }
        }
    }
}
