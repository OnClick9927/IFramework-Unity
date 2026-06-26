/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;

namespace IFramework
{
    public abstract class RedDot : IDisposable
    {
        public string path { get; private set; }
        internal RedTreeService tree;
        internal void SetPath(string path)
        {
            this.path = path;
            tree.AddDot(this);
            this.FreshView(this.GetCount());
        }
        public int GetCount() => tree.GetCount(this.path);
        public abstract void FreshView(int count);
        public void Dispose()
        {
            tree.RemoveDot(this);
            FreshView(0);
            StaticPool.SetByRealType(this);
        }
    }
}
