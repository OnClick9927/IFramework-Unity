/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OnAddComponentAttribute : System.Attribute
    {
        public readonly Type type;
        public int oder;
        public OnAddComponentAttribute(Type type)
        {
            this.type = type;
        }
    }
}
