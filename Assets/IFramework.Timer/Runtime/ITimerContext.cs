/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework
{
    public delegate bool TimerFunc(float time, float delta);
    public delegate void TimerAction(float time, float delta);


    public interface ITimerContext
    {
        string id { get; }
        bool isDone { get; }
        bool canceled { get; }

    }

}
