/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.51
 *UnityVersion:   2018.4.24f1
 *Date:           2020-09-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

#pragma warning disable
namespace IFramework
{
    partial class RootWindow
    {
        public abstract class UserOptionTab
        {
            public abstract string Name { get; }
            public abstract void OnGUI(Rect position);
            public virtual void OnEnable() { }
            public virtual void OnDisable() { }
        }

    }

}
