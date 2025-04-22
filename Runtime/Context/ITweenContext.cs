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
    public interface ITweenContext
    {
        bool isDone { get; }
        bool autoCycle { get; }
        bool paused { get; }
    }


    public interface ITweenContext<T, Target> : ITweenContext { }

    public interface ITweenGroup : ITweenContext
    {
        ITweenGroup NewContext(Func<ITweenContext> func);
    }

}