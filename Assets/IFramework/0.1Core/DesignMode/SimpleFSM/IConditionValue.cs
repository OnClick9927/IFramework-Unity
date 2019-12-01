/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-07-25
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework
{
    public interface IFsmConditionValue
    {
        string Name { get; set; }
        Type ValueType { get; }
        object Value { get; set; }
    }
    public class FsmConditionValue<T> : IFsmConditionValue
    {
        public string Name { get; set; }
        public Type ValueType { get { return typeof(T); } }
        public object Value { get; set; }

    }
}
