/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.Net
{
    public interface IServerToken
    {
        int BufferSize { get; }
        int MaxConCount { get; }
        int CurConCount { get; }
        bool IsRunning { get; }
        bool Start(int port, string ip = "0.0.0.0");
        void Stop();
    }

}
