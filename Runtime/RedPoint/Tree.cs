/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace IFramework.RedPoint
{
    public interface ITreeViewer
    {
        void FreshView(List<RedPoint> root);
    }
    public class Tree
    {
        private static ITreeViewer viewer;

        public static void SetViewer(ITreeViewer viewer)
        {
#if UNITY_EDITOR
            Tree.viewer = viewer;
            if (viewer == null) return;
            Tree.viewer.FreshView(Tree.root);
#endif
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
        internal static void FreshDot(string path, int count)
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

        private static void FreshParent(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            var point = Find(key);
            int sum = 0;
            foreach (var item in point.children.Values)
            {
                sum += item.GetCount();
            }
            if (point.SetCount(sum))
            {
                FreshParent(point.parent_key);
            }
        }
        public static void SetCount(string key, int count)
        {
            var point = Find(key);
            if (point == null || point.children.Count > 0) return;
            if (point.SetCount(count))
            {
                FreshParent(point.parent_key);
            }
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
            SetViewer(Tree.viewer);
        }
        public static void ClearPath(string key)
        {
            var point = Find(key);
            if (point == null) return;
            key_map.Remove(key);
            foreach (var item in point.children.Keys)
            {
                ClearPath(item);
            }
#if UNITY_EDITOR
            if (root.RemoveAll(x => x.key == key) > 0)
            {
                SetViewer(Tree.viewer);
            }
#endif
        }
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
#endif
        public static void ClearAll()
        {
            red_dot_map.Clear();
            key_map.Clear();
            root.Clear();
            SetViewer(Tree.viewer);
        }
    }

}