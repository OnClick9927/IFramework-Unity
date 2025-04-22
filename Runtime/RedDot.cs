﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
namespace IFramework.RedPoint
{
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

}
