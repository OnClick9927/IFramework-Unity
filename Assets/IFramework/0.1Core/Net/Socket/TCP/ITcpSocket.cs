/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Net;
using System.Net.Sockets;

namespace IFramework.Net
{
    public interface ITcpSocket
    {
        Socket Sock { get; }
        bool IsConnected { get; }
        EndPoint EndPoint { get; }
        int RecTimeout { get; }
        int SendTimeout { get; }
        int ConnTimeout { get; }
        int BufferSize { get; }
    }

}
