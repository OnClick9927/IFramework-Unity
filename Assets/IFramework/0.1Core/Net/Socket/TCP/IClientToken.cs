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
    public interface IClientToken
    {
        //bool IsRunning { get; }
        void ConnectAsync(int port, string ip);
        bool ConnectSync(int port, string ip);
        bool SendAsync(BufferSegment segBuff, bool wait = true);
        int SendSync(BufferSegment segBuff);
        void DisConnect();
    }

}
