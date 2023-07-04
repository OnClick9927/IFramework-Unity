using System;
using System.Collections.Generic;

namespace IFramework.Inject
{
    public partial class InjectModule
    {
        private class InjectTypeMap : InjectMap<Type>
        {
            public IEnumerable<Type> GetTypes(Type type)
            {
                foreach (var _type in map.Keys)
                {
                    if (type.IsAssignableFrom(_type))
                    {
                        foreach (var it in map[_type])
                        {
                            yield return GetByindex(it);
                        }
                    }

                }

            }
        }
    }
}
