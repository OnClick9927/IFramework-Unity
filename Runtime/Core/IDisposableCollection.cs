
using System;
using System.Collections.Generic;

namespace IFramework
{
    public static class IDisposableCollection
    {
        private static Dictionary<object, HashSet<IDisposable>> map = new();
        public static T AddTo<T>(this IDisposable self, T obj)
        {
            if (!map.TryGetValue(obj, out var list))
            {
                list = StaticPool.Get<HashSet<IDisposable>>();
                list.Clear();
                map[obj] = list;
            }
            try
            {
                list.Add(self);
            }
            catch (Exception)
            {
            }
            return obj;
        }
        public static T RemoveDisposable<T>(this T t, IDisposable disposable)
        {
            if (map.TryGetValue(t, out var list))
            {
                if (list.Remove(disposable))
                {
                    disposable.Dispose();
                }
                if (list.Count == 0)
                {
                    map.Remove(t);
                    StaticPool.Set(list);
                }
            }
            return t;
        }
        public static T ClearDisposable<T>(this T t)
        {
            if (map.Remove(t, out var list))
            {
                foreach (var item in list)
                {
                    item.Dispose();
                }
                list.Clear();
                StaticPool.Set(list);
            }
            return t;
        }
    }
}
