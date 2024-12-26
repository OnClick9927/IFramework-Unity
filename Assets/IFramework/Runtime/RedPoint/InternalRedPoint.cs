/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;

namespace IFramework.RedPoint
{
    public class InternalRedPoint
    {
        public string parent_key;
        public string key;
        public Dictionary<string, InternalRedPoint> children = new Dictionary<string, InternalRedPoint>();
        private int count;
        private int count_dirty;

        private bool hasValue = false;
        public InternalRedPoint(string key, string parent_key)
        {
            this.parent_key = parent_key;
            this.key = key;
        }

        public int GetCount()
        {
            return hasValue ? count : 0;
        }
        public void AddChild(InternalRedPoint p)
        {
            var key = p.key;
            if (children.ContainsKey(key))
                return;
            children.Add(key, p);
        }
        public void RemoveChild(InternalRedPoint p)
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

}
