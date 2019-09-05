/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-22
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework
{
    public interface IEventArgs { }
    public interface IEventArgs<T>: IEventArgs { Type EventType { get; } }

}
