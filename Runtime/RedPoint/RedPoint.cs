/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System.Collections.Generic;

namespace IFramework.RedPoint
{
    public class RedPoint
    {
        public string parent_key;
        public string key;
        public Dictionary<string, RedPoint> children = new Dictionary<string, RedPoint>();
        private int count;
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
            var p_key = p.key;
            if (children.ContainsKey(p_key))
                return;
            children.Add(p_key, p);
        }

        public bool SetCount(int count)
        {
            if (hasValue && count == this.count) return false;
            this.count = count;
            hasValue = true;
            Tree.FreshDot(key, count);
            return true;
        }
    }

}
