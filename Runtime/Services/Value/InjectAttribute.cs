/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
namespace IFramework
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InjectAttribute : System.Attribute
    {
        public InjectAttribute(string name = "")
        {
            this.name = name;
        }

        public string name { get; private set; }

    }
}
