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
    public enum TweenContextState
    {
        Allocate,
        Run,
        Pause,
        Sleep,
    }
    public interface ITweenContext
    {
        string id { get; }
        bool isDone { get; }
        bool autoCycle { get; }
        bool paused { get; }

        TweenContextState state {  get; }
    }


    public interface ITweenContext<T, Target> : ITweenContext { }

    public interface ITweenGroup : ITweenContext
    {
        ITweenGroup NewContext(Func<ITweenContext> func);
    }

}