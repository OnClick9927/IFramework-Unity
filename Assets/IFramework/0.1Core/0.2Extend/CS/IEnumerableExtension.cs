/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace IFramework
{
	public static partial class IEnumerableExtension
	{
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> selfArray, Action<T> action)
        {
            if (action == null) throw new ArgumentException();
            foreach (var item in selfArray)
            {
                action(item);
            }
            return selfArray;
        }

        public static T[] ForEach<T>(this T[] selfArray, Action<T> action)
        {
            Array.ForEach(selfArray, action);
            return selfArray;
        }
        public static T[] ForEach<T>(this T[] selfArray, Action<int, T> action)
        {
            for (var i = 0; i < selfArray.Length; i++)
            {
                action(i, selfArray[i]);
            }
            return selfArray;

        }

        public static List<T> ReverseForEach<T>(this List<T> self, Action<T> action)
        {
            if (action == null) throw new ArgumentException();

            for (var i = self.Count - 1; i >= 0; --i)
                action(self[i]);
            return self;
        }
        public static void ForEach<T>(this List<T> self, Action<int, T> action)
        {
            for (var i = 0; i < self.Count; i++)
            {
                action(i, self[i]);
            }
        }
        public static void CopyTo<T>(this List<T> self, List<T> to, int begin = 0, int end = -1)
        {
            if (begin < 0) begin = 0;
            var endIndex = Math.Min(self.Count, to.Count) - 1;
            if (end != -1 && end < endIndex) endIndex = end;
            for (var i = begin; i < end; i++)
            {
                to[i] = self[i];
            }
        }
        public static T Dequeue<T>(this List<T> self)
        {
            if (self == null || self.Count <= 0) throw new Exception("Null List");
            T t = self[0];
            self.Remove(t);
            return t;
        }
        public static void Enqueue<T>(this List<T> self, T t)
        {
            if (self == null) throw new Exception("Null List");
            self.Add(t);
        }
        public static void Push<T>(this List<T> self, T t)
        {
            if (self == null) throw new Exception("Null List");
            self.Add(t);
        }
        public static T Pop<T>(this List<T> self)
        {
            if (self == null || self.Count <= 0) throw new Exception("Null List");
            T t = self[self.Count - 1];
            self.Remove(t);
            return t;
        }
        public static T QueuePeek<T>(this List<T> list)
        {
            if (list == null || list.Count <= 0) throw new Exception("Null List");
            return list[0];
        }
        public static T StackPeek<T>(this List<T> list)
        {
            if (list == null || list.Count <= 0) throw new Exception("Null List");
            return list[list.Count - 1];
        }


        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, params Dictionary<TKey, TValue>[] dictionaries)
        {
            return dictionaries.Aggregate(dictionary,
                (current, self) => current.Union(self).ToDictionary(kv => kv.Key, kv => kv.Value));
        }
        public static void ForEach<K, V>(this Dictionary<K, V> self, Action<K, V> action)
        {
            var dictE = self.GetEnumerator();
            while (dictE.MoveNext())
            {
                var current = dictE.Current;
                action(current.Key, current.Value);
            }

            dictE.Dispose();
        }
        public static void AddRange<K, V>(this Dictionary<K, V> self, Dictionary<K, V> addInDict, bool isOverride = false)
        {
            var dictE = addInDict.GetEnumerator();

            while (dictE.MoveNext())
            {
                var current = dictE.Current;
                if (self.ContainsKey(current.Key))
                {
                    if (isOverride)
                        self[current.Key] = current.Value;
                    continue;
                }

                self.Add(current.Key, current.Value);
            }

            dictE.Dispose();
        }
    }
}
