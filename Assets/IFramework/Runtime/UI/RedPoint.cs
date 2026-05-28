/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IFramework
{
    class RedPoint
    {
        public string parent_key;
        public string key;
        public Dictionary<string, RedPoint> children = new Dictionary<string, RedPoint>();
        private int count;
        private int count_dirty;

        private bool hasValue = false;
        public RedPoint(string key, string parent_key)
        {
            this.parent_key = parent_key;
            this.key = key;
        }

        public int GetCount()
        {
            return hasValue ? count : 0;
        }
        public void AddChild(RedPoint p)
        {
            var key = p.key;
            if (children.ContainsKey(key))
                return;
            children.Add(key, p);
        }
        public void RemoveChild(RedPoint p)
        {
            var key = p.key;
            if (!children.ContainsKey(key))
                return;
            children.Remove(key);
        }

        public bool SetCount(int count)
        {
            if (hasValue && count == this.count) return false;
            this.count = count;
            hasValue = true;
            return true;
        }
        public int GetDirtyCount()
        {
            return count_dirty;
        }
        public bool SetDirty(int count)
        {
            if (count == this.GetCount()) return false;
            this.count_dirty = count;
            return true;
        }
    }
    internal interface ITreeViewer
    {
        void FreshView(List<RedPoint> root);
    }
    public class RedTree
    {
        private static ITreeViewer viewer;

        internal static void SetViewer(ITreeViewer viewer)
        {
#if UNITY_EDITOR
            RedTree.viewer = viewer;
            if (viewer == null) return;
            RedTree.viewer.FreshView(RedTree.root);
#endif
        }
        public static int GetDotCount(string key)
        {
            List<RedDot> result;
            if (red_dot_map.TryGetValue(key, out result))
                return result.Count;
            return 0;
        }
        private static Dictionary<string, List<RedDot>> red_dot_map = new Dictionary<string, List<RedDot>>();
        private static Dictionary<string, RedPoint> key_map = new Dictionary<string, RedPoint>();

        private static List<RedPoint> root = new List<RedPoint>();
        internal static void AddDot(RedDot dot)
        {
            var path = dot.path;
            if (!red_dot_map.ContainsKey(path))
                red_dot_map.Add(path, new List<RedDot>());
            red_dot_map[path].Add(dot);
        }
        internal static void RemoveDot(RedDot dot)
        {
            var path = dot.path;
            if (!red_dot_map.ContainsKey(path))
                return;
            if (!red_dot_map[path].Contains(dot)) return;
            red_dot_map[path].Remove(dot);
        }
        private static void FreshDot(string path, int count)
        {
            if (!red_dot_map.ContainsKey(path))
                return;
            foreach (var item in red_dot_map[path])
            {
                item.FreshView(count);
            }
        }
        public static char separator = '/';

        private static RedPoint Find(string key)
        {
            if (key_map.ContainsKey(key))
                return key_map[key];
            return null;
        }

        private static RedPoint AddPoint(string parentKey, string key)
        {
            var _new = Find(key);
            if (_new == null)
            {
                _new = new RedPoint(key, parentKey);
                key_map.Add(key, _new);
            }
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(parentKey))
            {
                if (root.Find(x => x.key == key) == null)
                    root.Add(_new);
            }
#endif
            return _new;
        }


        public static void SetCount(string key, int count)
        {
            var point = Find(key);
            if (point == null) return;
            if (point.SetDirty(count) && point.children.Count == 0)
                dirty.Enqueue(point);

        }

        private static Queue<RedPoint> dirty = new Queue<RedPoint>();
        private static Queue<RedPoint> dirty_P = new Queue<RedPoint>();

        private static void FreshParent(string key, Dictionary<string, int> map)
        {
            if (string.IsNullOrEmpty(key)) return;
            var point = Find(key);
            int sum = 0;
            foreach (var item in point.children.Values)
            {
                if (Find(item.key) != null)
                    sum += item.GetCount();
            }
            if (point.SetCount(sum))
            {
                map[point.key] = sum;
                FreshParent(point.parent_key, map);
            }
        }
        static Dictionary<string, int> map = new Dictionary<string, int>();

        public static void FreshDots()
        {
            if (dirty.Count == 0 && dirty_P.Count == 0) return;
            map.Clear();
            while (dirty.Count > 0)
            {
                var point = dirty.Dequeue();

                if (Find(point.key) == null) continue;
                var count = point.GetDirtyCount();
                if (point.SetCount(count))
                {
                    map[point.key] = count;
                    FreshParent(point.parent_key, map);
                }
            }
            while (dirty_P.Count > 0)
            {
                var point = dirty_P.Dequeue();
                FreshParent(point.key, map);

            }

            foreach (var item in map)
            {
                FreshDot(item.Key, item.Value);
            }
            SetViewer(RedTree.viewer);

        }


        public static int GetCount(string key)
        {
            var point = Find(key);
            return point == null ? 0 : point.GetCount();
        }
        public static void ReadPath(string key)
        {
            var columns = key.Split(separator);
            RedPoint last = null;
            for (int j = 0; j < columns.Length; j++)
            {
                var _pkey = string.Join(separator.ToString(), columns, 0, j);
                var _key = string.Join(separator.ToString(), columns, 0, j + 1);
                var point = AddPoint(_pkey, _key);
                if (last != null)
                    last.AddChild(point);

                last = point;
            }
            SetViewer(RedTree.viewer);
        }
        public static void ClearPath(string key)
        {
            var point = Find(key);
            if (point == null) return;
            var p_point = Find(point.parent_key);
            if (p_point != null)
            {
                p_point.RemoveChild(point);
                dirty_P.Enqueue(p_point);
            }
            key_map.Remove(key);
            foreach (var item in point.children.Keys)
                ClearPath(item);

#if UNITY_EDITOR
            if (root.RemoveAll(x => x.key == key) > 0)
            {
                SetViewer(RedTree.viewer);
            }
#endif
        }
#if UNITY_EDITOR
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        public static void ClearAll()
        {
            dirty.Clear();
            dirty_P.Clear();
            red_dot_map.Clear();
            key_map.Clear();
            root.Clear();
            SetViewer(RedTree.viewer);
        }
    }

    public abstract class RedDot : IDisposable
    {
        public string path;

        public void SetPath(string path)
        {
            this.path = path;
            RedTree.AddDot(this);
            this.FreshView(this.GetCount());
        }
        public int GetCount() => RedTree.GetCount(this.path);
        public abstract void FreshView(int count);
        public void Dispose()
        {
            RedTree.RemoveDot(this);
            FreshView(0);
        }
    }
    public class RedActiveDot : RedDot
    {
        private GameObject gameObject;

        public RedActiveDot(string path, GameObject gameObject)
        {
            this.gameObject = gameObject;
            this.SetPath(path);
        }

        public override void FreshView(int count)
        {
            this.gameObject.SetActive(count > 0);
        }
    }
}
